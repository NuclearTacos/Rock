// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//

using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Quartz;
using Rock;
using Rock.Data;
using Rock.Model;

namespace church.life.Jobs
{
    /// <summary>
    /// Job to update Group Members' Group Requirement statuses for requirements that are calculated from SQL
    /// </summary>
    [DisallowConcurrentExecution]
    public class CalculateGroupRequirementsBulkSqlOnly : IJob
    {

        enum RequirementStatus
        {
            Pass = 0,
            Warning = 1,
            Fail = 2
        };

        /// <summary> 
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public CalculateGroupRequirementsBulkSqlOnly()
        {
        }

        public void Execute( IJobExecutionContext context )
        {
            var rockContext1 = new RockContext();
            var groupRequirementTypeService1 = new GroupRequirementTypeService( rockContext1 );

            // we only need to consider group requirements that are based on a DataView or SQL
            var groupRequirementTypeQry = groupRequirementTypeService1.Queryable()
                .Where( a => a.RequirementCheckType == RequirementCheckType.Sql )
                .AsNoTracking();

            foreach (var groupRequirementType in groupRequirementTypeQry.ToList())
            {
                // Get GroupMemberRequirements that exist for the GroupMember, GroupRequirementType
                string existingGroupMemberRequirements =
                    $@"Select gm.PersonId, gmr.*
                    From [GroupMember] gm
	                    Inner Join [Group] g on g.Id = gm.GroupId and g.GroupTypeId not in ( 10, 11, 12 )
	                    Left Join [GroupRequirement] gr on ( gr.GroupId = g.Id or gr.GroupTypeId = g.GroupTypeId ) and gr.GroupRoleId = gm.GroupRoleId
	                    Left Join [GroupMemberRequirement] gmr on gmr.GroupMemberId = gm.Id and gmr.GroupRequirementId = gr.Id
                    Where gr.GroupRequirementTypeId = {groupRequirementType.Id}
	                    and gmr.Id is not null
                        -- and gm.IsArchived = 0
                        and g.IsArchived = 0";

                // Get GroupMembers that should have a Requirement of that type, but do not
                string groupMembersWithoutRequirements =
                    $@"Select gr.Id as 'GroupRequirementId', gm.*
                    From [GroupMember] gm
	                    Inner Join [Group] g on g.Id = gm.GroupId and g.GroupTypeId not in ( 10, 11, 12 )
	                    Left Join [GroupRequirement] gr on ( gr.GroupId = g.Id or gr.GroupTypeId = g.GroupTypeId ) and gr.GroupRoleId = gm.GroupRoleId
	                    Left Join [GroupMemberRequirement] gmr on gmr.GroupMemberId = gm.Id and gmr.GroupRequirementId = gr.Id
                    Where gr.GroupRequirementTypeId = {groupRequirementType.Id}
	                    and gmr.Id is null
                        -- and gm.IsArchived = 0
                        and g.IsArchived = 0";

                // Retrieve SQL expressions for Pass and Warning
                string sqlPassExpression = groupRequirementType.SqlExpression.ResolveMergeFields( new Dictionary<string, object>(), new Person() );
                string sqlWarningExpression = groupRequirementType.WarningSqlExpression.ResolveMergeFields( new Dictionary<string, object>(), new Person() );

                UpdatePassPeople( existingGroupMemberRequirements, groupMembersWithoutRequirements, sqlPassExpression, sqlWarningExpression );
                UpdateWarningPeople( existingGroupMemberRequirements, groupMembersWithoutRequirements, sqlPassExpression, sqlWarningExpression );
                UpdateFailPeople( existingGroupMemberRequirements, groupMembersWithoutRequirements, sqlPassExpression );
            }
        }

        private void UpdatePassPeople( string existingGroupMemberRequirements, string groupMembersWithoutRequirements, string sqlPassExpression, string sqlWarningExpression )
        {
            // Update existingGroupMemberRequirements to be GroupMemberRequirements of People that Pass *without* Warning
            existingGroupMemberRequirements =
                $@"Select egmr.*
                From (
                    {existingGroupMemberRequirements}
	                ) egmr
	            Where egmr.PersonId in ( Select d.Id From (
                        {sqlPassExpression}
                    ) d )
                    and egmr.PersonId not in ( Select d.Id From (
                        {sqlWarningExpression}
                    ) d )";

            // Update groupMembersWithoutRequirements to be GroupMembers of People that Pass *without* Warning
            groupMembersWithoutRequirements =
                $@"Select gmwr.*
                From (
	                {groupMembersWithoutRequirements}
	                ) gmwr
	            Where gmwr.PersonId in ( Select d.Id From (
                        {sqlPassExpression}
                    ) d )
                    and gmwr.PersonId not in ( Select d.Id From (
                        {sqlWarningExpression}
                    ) d )";

            UpdateGroupRequirements( existingGroupMemberRequirements, RequirementStatus.Pass );
            InsertGroupMemberRequirements( groupMembersWithoutRequirements, RequirementStatus.Pass );
        }

        private void UpdateWarningPeople( string existingGroupMemberRequirements, string groupMembersWithoutRequirements, string sqlPassExpression, string sqlWarningExpression )
        {
            // Update existingGroupMemberRequirements to be GroupMemberRequirements of People that Pass *without* Warning
            existingGroupMemberRequirements =
                $@"Select egmr.*
                From (
                    {existingGroupMemberRequirements}
	                ) egmr
                Where egmr.PersonId in ( Select d.Id From (
                        {sqlPassExpression}
                    ) d )
                    and egmr.PersonId in ( Select d.Id From (
                        {sqlWarningExpression}
                    ) d )";

            // Update groupMembersWithoutRequirements to be GroupMembers of People that Pass *without* Warning
            groupMembersWithoutRequirements =
                $@"Select gmwr.*
                From (
	                {groupMembersWithoutRequirements}
	                ) gmwr
                Where gmwr.PersonId in ( Select d.Id From (
                        {sqlPassExpression}
                    ) d )
                    and gmwr.PersonId in ( Select d.Id From (
                        {sqlWarningExpression}
                    ) d )";

            UpdateGroupRequirements( existingGroupMemberRequirements, RequirementStatus.Warning );
            InsertGroupMemberRequirements( groupMembersWithoutRequirements, RequirementStatus.Warning );
        }

        private void UpdateFailPeople( string existingGroupMemberRequirements, string groupMembersWithoutRequirements, string sqlPassExpression )
        {
            // Update existingGroupMemberRequirements to be GroupMemberRequirements of People that Pass *without* Warning
            existingGroupMemberRequirements =
                $@"Select egmr.*
                From (
                    {existingGroupMemberRequirements}
	                ) egmr
	            Where egmr.PersonId not in (
                    Select d.Id From (
                        {sqlPassExpression}
                    ) d
	            )";

            // Update groupMembersWithoutRequirements to be GroupMembers of People that Pass *without* Warning
            groupMembersWithoutRequirements =
                $@"Select gmwr.*
                From (
	                {groupMembersWithoutRequirements}
	                ) gmwr
	            Where gmwr.PersonId not in ( Select d.Id From (
                        {sqlPassExpression}
                    ) d )";

            UpdateGroupRequirements( existingGroupMemberRequirements, RequirementStatus.Fail );
            InsertGroupMemberRequirements( groupMembersWithoutRequirements, RequirementStatus.Fail );
        }

        private void UpdateGroupRequirements( string existingGroupMemberRequirements, RequirementStatus requirementStatus )
        {
            string sGetDate = "GetDate()";
            string sNull = "null";

            string sMetDate = "";
            string sWarningDate = "";
            string sFailDate = "";

            switch (requirementStatus)
            {
                case RequirementStatus.Pass:
                    sMetDate = sGetDate;
                    sWarningDate = sNull;
                    sFailDate = sNull;
                    break;

                case RequirementStatus.Warning:
                    sMetDate = sGetDate;
                    sWarningDate = sGetDate;
                    sFailDate = sNull;
                    break;

                case RequirementStatus.Fail:
                    sMetDate = sNull;
                    sWarningDate = sNull;
                    sFailDate = sGetDate;
                    break;
            }

            var updateEGMR =
                $@"Update gmr
                Set
	                gmr.RequirementMetDateTime = {sMetDate}
                    , gmr.LastRequirementCheckDateTime = GetDate()
                    , gmr.RequirementWarningDateTime = {sWarningDate}
                    , gmr.RequirementFailDateTime = {sFailDate}
                    , gmr.ModifiedDateTime = GetDate()
                    , gmr.ModifiedByPersonAliasId = 10
                From [GroupMemberRequirement] gmr
	                Join (
                        {existingGroupMemberRequirements}
                    ) x on x.Id = gmr.Id
                Where x.Id = gmr.Id";

            DbService.ExecuteCommand( updateEGMR );
        }

        private void InsertGroupMemberRequirements( string groupMembersWithoutRequirements, RequirementStatus requirementStatus )
        {
            string sGetDate = "GetDate()";
            string sNull = "null";

            string sMetDate = "";
            string sWarningDate = "";
            string sFailDate = "";

            switch (requirementStatus)
            {
                case RequirementStatus.Pass:
                    sMetDate = sGetDate;
                    sWarningDate = sNull;
                    sFailDate = sNull;
                    break;

                case RequirementStatus.Warning:
                    sMetDate = sGetDate;
                    sWarningDate = sGetDate;
                    sFailDate = sNull;
                    break;

                case RequirementStatus.Fail:
                    sMetDate = sNull;
                    sWarningDate = sNull;
                    sFailDate = sGetDate;
                    break;
            }

            var insertGMWR =
                $@"INSERT INTO [GroupMemberRequirement]
                (
                    GroupMemberId
                  , GroupRequirementId
                  , RequirementMetDateTime
                  , LastRequirementCheckDateTime
                  , CreatedDateTime
                  , ModifiedDateTime
                  , CreatedByPersonAliasId
                  , ModifiedByPersonAliasId
                  , Guid
                  , ForeignKey
                  , RequirementFailDateTime
                  , RequirementWarningDateTime
                  , ForeignGuid
                  , ForeignId
                )
                Select
                    x.Id		--	GroupMemberId
                    , x.GroupRequirementId		--  , GroupRequirementId
                    , {sMetDate}		--  , RequirementMetDateTime
                    , GetDate()		--  , LastRequirementCheckDateTime
                    , GetDate()		--  , CreatedDateTime
                    , GetDate()		--  , ModifiedDateTime
                    , 10		--  , CreatedByPersonAliasId
                    , 10		--  , ModifiedByPersonAliasId
                    , NewId()		--  , Guid
                    , null		--  , ForeignKey
                    , {sFailDate}		--  , RequirementFailDateTime
                    , {sWarningDate}    --  , RequirementWarningDateTime
                    , null		--  , ForeignGuid
                    , null		--  , ForeignId
                From (
                    {groupMembersWithoutRequirements}
                ) x";

            DbService.ExecuteCommand( insertGMWR );
        }
    }
}