// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_secc.Administration
{
    /// <summary>
    /// Block to execute a sql command and display the result (if any).
    /// </summary>
    [DisplayName( "Group Type Changer" )]
    [Category( "SECC > Administration" )]
    [Description( "Tool to change the group type of a group." )]

    public partial class GroupTypeChanger : RockBlock
    {

        Group selectedGroup;
        RockContext rockContext;
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            nbSuccess.Visible = false;
            var groupId = PageParameter( "GroupId" ).AsInteger();
            if ( groupId != 0 )
            {
                rockContext = new RockContext();
                selectedGroup = new GroupService( rockContext ).Get( groupId );
                if ( selectedGroup != null )
                {
                    ltName.Text = string.Format( "<span style='font-size:1.5em;'>{0}</span>", selectedGroup.Name );
                    ltGroupTypeName.Text = string.Format( "<span style='font-size:1.5em;'>{0}</span>", selectedGroup.GroupType.Name );
                }
            }

            if ( !Page.IsPostBack )
            {
                BindGroupTypeDropDown();
                BindComparisonDropDown();

                dvBulkGroupDV.EntityTypeId = EntityTypeCache.GetId( typeof( Rock.Model.Group ) );
            }
            else
            {
                var groupTypeId = ddlGroupTypes.SelectedValue.AsInteger();
                if ( groupTypeId == 0 || selectedGroup == null )
                {
                    return;
                }
                var newGroupType = new GroupTypeService( new RockContext() ).Get( groupTypeId );
                BindRoles( newGroupType, selectedGroup.GroupType.Roles );
                DisplayAttributes();
            }

        }

        private void BindComparisonDropDown()
        {
            //ddlBulkComparison.Items.Add( new ListItem(
            //        text: "Equal To",
            //        value: ComparisonOptions.EqualTo.ToStringSafe()
            //    ) );
            //ddlBulkComparison.Items.Add( new ListItem(
            //        text: "Starts With",
            //        value: ComparisonOptions.StartsWith.ToStringSafe()
            //    ) );
            //ddlBulkComparison.Items.Add( new ListItem(
            //        text: "Ends With",
            //        value: ComparisonOptions.EndsWith.ToStringSafe()
            //    ) );
            //ddlBulkComparison.Items.Add( new ListItem(
            //        text: "Contains",
            //        value: ComparisonOptions.Contains.ToStringSafe()
            //    ) );
        }

        private void BindGroupTypeDropDown()
        {
            var groupTypes = new GroupTypeService( new RockContext() ).Queryable()
                .Select( gt => new
                {
                    Id = gt.Id,
                    Name = gt.Name
                } ).ToList();

            groupTypes.Insert( 0, new { Id = 0, Name = "" } );

            ddlGroupTypes.DataSource = groupTypes;
            ddlGroupTypes.DataBind();
        }

        protected void ddlGroupTypes_SelectedIndexChanged( object sender, EventArgs e )
        {
            var groupTypeId = ddlGroupTypes.SelectedValue.AsInteger();
            if ( groupTypeId != 0 && selectedGroup != null )
            {
                btnSave.Visible = true;
                pnlBulk.Visible = true;
                var newGroupType = new GroupTypeService( rockContext ).Get( groupTypeId );

                BindRoles( newGroupType, selectedGroup.GroupType.Roles );
                DisplayAttributes();

            }
            else
            {
                pnlAttributes.Visible = false;
                pnlRoles.Visible = false;
                pnlBulk.Visible = false;
                btnSave.Visible = false;
            }
        }

        private void BindRoles( GroupType newGroupType, ICollection<GroupTypeRole> roles )
        {
            if ( roles.Any() && newGroupType != null )
            {
                pnlRoles.Visible = true;
                phRoles.Controls.Clear();
                foreach ( var role in roles )
                {
                    RockDropDownList ddlRole = new RockDropDownList()
                    {
                        DataTextField = "Name",
                        DataValueField = "Id",
                        Label = role.Name,
                        ID = role.Id.ToString() + "_ddlRole"
                    };
                    BindRoleDropDown( newGroupType, ddlRole );
                    phRoles.Controls.Add( ddlRole );
                }
            }
            else
            {
                pnlRoles.Visible = false;
            }
        }

        private void DisplayAttributes()
        {
            phAttributes.Controls.Clear();

            var newGroupTypeId = ddlGroupTypes.SelectedValue.AsInteger();

            if ( newGroupTypeId == 0 )
            {
                pnlAttributes.Visible = false;
                return;
            }

            var attributeService = new AttributeService( rockContext );

            var groupMemberEntityId = new EntityTypeService( rockContext ).Get( Rock.SystemGuid.EntityType.GROUP_MEMBER.AsGuid() ).Id;
            var stringGroupTypeId = selectedGroup.GroupTypeId.ToString();

            var attributes = attributeService.Queryable()
                .Where( a =>
                    a.EntityTypeQualifierColumn == "GroupTypeId"
                    && a.EntityTypeQualifierValue == stringGroupTypeId
                    && a.EntityTypeId == groupMemberEntityId
                    ).ToList();
            if ( attributes.Any() )
            {
                var newGroupTypeIdString = newGroupTypeId.ToString();
                var selectableAttributes = attributeService.Queryable()
                    .Where( a =>
                        a.EntityTypeQualifierColumn == "GroupTypeId"
                        && a.EntityTypeQualifierValue == newGroupTypeIdString
                        && a.EntityTypeId == groupMemberEntityId
                        )
                    .ToList()
                    .Select( a => new
                    {
                        Id = a.Id.ToString(),
                        Name = a.Name + " [" + a.Key + "]"
                    } )
                    .ToList();

                var groupIdString = selectedGroup.Id.ToString();
                var groupAttributes = attributeService.Queryable()
                    .Where( a =>
                        a.EntityTypeQualifierColumn == "GroupId"
                        && a.EntityTypeQualifierValue == groupIdString
                        && a.EntityTypeId == groupMemberEntityId
                        )
                    .ToList()
                    .Select( a => new
                    {
                        Id = a.Id.ToString(),
                        Name = a.Name + " [" + a.Key + "]"
                    } )
                    .ToList();

                selectableAttributes.AddRange( groupAttributes );


                pnlAttributes.Visible = true;
                foreach ( var attribute in attributes )
                {
                    RockDropDownList ddlAttribute = new RockDropDownList()
                    {
                        ID = attribute.Id.ToString() + "_ddlAttribute",
                        Label = attribute.Name,
                        DataValueField = "Id",
                        DataTextField = "Name"
                    };
                    ddlAttribute.DataSource = selectableAttributes;
                    ddlAttribute.DataBind();
                    phAttributes.Controls.Add( ddlAttribute );
                }
            }
            else
            {
                pnlAttributes.Visible = false;
            }
        }

        private void BindRoleDropDown( GroupType newGroupType, RockDropDownList ddlRole )
        {
            ddlRole.DataSource = newGroupType.Roles;
            ddlRole.DataBind();
        }

        protected void btnSave_Click( object sender, EventArgs e )
        {
            //Get the old groupTypeId before we change it
            var stringGroupTypeId = selectedGroup.GroupTypeId.ToString();

            //TODO: Chris's magic
            List<Group> groupsToBeChanged = new List<Group>();

            var dataViewId = 0;
            int.TryParse( dvBulkGroupDV.ItemId, out dataViewId );

            List<string> dvErrorMessage = new List<string>();
            var rockContext = new RockContext();
            if( dataViewId > 0 )
            {
                var dataViewService = new DataViewService( rockContext );

                var dvQuery = ( IQueryable<Group> ) dataViewService.Get( dataViewId ).GetQuery( null, rockContext, null, out dvErrorMessage );

                groupsToBeChanged.AddRange( dvQuery.ToList() );
            }
            else
            {
                groupsToBeChanged.Add( selectedGroup );
            }

            if( dvErrorMessage.Count == 0 )
            {
                foreach( var group in groupsToBeChanged )
                {
                    var groupMembers = group.Members;
                    foreach( var role in group.GroupType.Roles ) // For Role in New Role
                    {
                        var ddlRole = ( RockDropDownList ) phRoles.FindControl( role.Id.ToString() + "_ddlRole" );
                        var roleMembers = groupMembers.Where( gm => gm.GroupRoleId == role.Id );
                        foreach( var member in roleMembers )
                        {
                            member.GroupRoleId = ddlRole.SelectedValue.AsInteger();
                        }
                    }

                    //Map attributes
                    var attributeService = new AttributeService( rockContext );
                    var attributeValueService = new AttributeValueService( rockContext );

                    var groupMemberEntityId = new EntityTypeService( rockContext ).Get( Rock.SystemGuid.EntityType.GROUP_MEMBER.AsGuid() ).Id;

                    var attributes = attributeService.Queryable()
                        .Where( a =>
                            a.EntityTypeQualifierColumn == "GroupTypeId"
                            && a.EntityTypeQualifierValue == stringGroupTypeId
                            && a.EntityTypeId == groupMemberEntityId
                            ).ToList();
                    foreach( var attribute in attributes )
                    {
                        var ddlAttribute = ( RockDropDownList ) phAttributes.FindControl( attribute.Id.ToString() + "_ddlAttribute" );
                        if( ddlAttribute != null )
                        {
                            var newAttributeId = ddlAttribute.SelectedValue.AsInteger();
                            if( newAttributeId != 0 )
                            {
                                foreach( var member in groupMembers )
                                {
                                    var attributeEntity = attributeValueService.Queryable()
                                        .Where( av => av.EntityId == member.Id && av.AttributeId == attribute.Id )
                                        .FirstOrDefault();
                                    if( attributeEntity != null )
                                    {
                                        attributeEntity.AttributeId = newAttributeId;
                                    }
                                }
                            }
                        }
                    }

                    var groupTypeService = new GroupTypeService( rockContext );
                    group.GroupType = groupTypeService.Get( ddlGroupTypes.SelectedValue.AsInteger() );
                }
                rockContext.SaveChanges();
                nbSuccess.Visible = true;
            }
            else
            {
                nbSuccess.Visible = false;
                //TODO: show the error message somehow
            }
        }

        protected void cbEnableBulk_CheckedChanged( object sender, EventArgs e )
        {
            //nbParentLevels.Visible = cbEnableBulk.Checked;
            //ddlBulkComparison.Visible = cbEnableBulk.Checked;
            //tbName.Visible = cbEnableBulk.Checked;
        }
    }

    #region Enumerations

    public enum ComparisonOptions
    {
        EqualTo = 0,

        StartsWith = 1,

        EndsWith = 2,

        Contains = 3
    }

    #endregion
}