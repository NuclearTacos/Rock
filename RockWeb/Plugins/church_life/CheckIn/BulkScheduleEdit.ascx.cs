﻿// <copyright>
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
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RockWeb.Plugins.CheckIn
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName( "Bulk Schedule Edit - LC" )]
    [Category( "Life.Church > Check-in" )]
    [Description( "Helps to build schedules used for check-in." )]
    public partial class BulkScheduleEdit : RockBlock, ICustomGridOptions
    {
        private int? _groupTypeId = null;

        const string SCHEDULE_GROUPS_DEFINED_TYPE = "a3ac7606-3c0a-4812-b462-4121a781419a";

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            _groupTypeId = this.PageParameter( "groupTypeId" ).AsIntegerOrNull();
            btnCancel.Visible = _groupTypeId.HasValue;

            gDefinedValueSchedule.DataKeyNames = new string[] { "DefinedValueId" };
            gDefinedValueSchedule.Actions.ShowAdd = false;
            gDefinedValueSchedule.IsDeleteEnabled = false;
            gDefinedValueSchedule.GridRebind += gDefinedValueSchedule_GridRebind;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
               
                BindFilter();
                BindGrid();
            }

            if( pkrParentLocation.SelectedValue == "0" )
            {
                gDefinedValueSchedule.Visible = false;
                btnSave.Visible = false;
            }
            else
            {
                gDefinedValueSchedule.Visible = true;
                btnSave.Visible = true;
            }

            base.OnLoad( e );
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlGroupType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        //protected void ddlGroupType_SelectedIndexChanged( object sender, EventArgs e )
        //{
        //   SetBlockUserPreference( "Group Type", ddlGroupType.SelectedValueAsId().ToString() );
        //   BindGrid();
        //}

        /// <summary>
        /// Handles the SelectItem event of the pkrParentLocation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void pkrParentLocation_SelectItem( object sender, EventArgs e )
        {
            SetBlockUserPreference( "Parent Location", pkrParentLocation.SelectedValueAsId().ToString() );
            BindGrid();
        }

        /// <summary>
        /// Handles the SelectItem event of the pCategory control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void pCategory_SelectItem( object sender, EventArgs e )
        {
            SetBlockUserPreference( "Category", pCategory.SelectedValueAsId().ToString() );
            BindGrid();
        }

        /// <summary>
        /// Handles the GridRebind event of the ggDefinedValueSchedule control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void gDefinedValueSchedule_GridRebind( object sender, EventArgs e )
        {
            BindGrid();
        }

        /// <summary>
        /// Handles the RowDataBound event of the ggDefinedValueSchedule control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GridViewRowEventArgs"/> instance containing the event data.</param>
        protected void gDefinedValueSchedule_RowDataBound( object sender, GridViewRowEventArgs e )
        {
            // add tool-tip to header columns
            if ( e.Row.RowType == DataControlRowType.Header )
            {
                var scheduleService = new ScheduleService( new RockContext() );

                foreach ( var cell in e.Row.Cells.OfType<DataControlFieldCell>() )
                {
                    if ( cell.ContainingField is CheckBoxEditableField )
                    {
                        CheckBoxEditableField checkBoxEditableField = cell.ContainingField as CheckBoxEditableField;
                        int scheduleId = int.Parse( checkBoxEditableField.DataField.Replace( "scheduleField_", string.Empty ) );

                        var schedule = scheduleService.Get( scheduleId );
                        if ( schedule != null )
                        {
                            cell.Attributes["title"] = schedule.ToString();
                        }
                    }
                }
            }
            else
            {
                if ( e.Row.DataItem != null )
                {
                    var dataRow = e.Row.DataItem as System.Data.DataRowView;
                    Literal lGroupName = e.Row.FindControl( "lGroupName" ) as Literal;
                    if ( lGroupName != null )
                    {
                        //lGroupName.Text = string.Format( "{0}<br /><small>{1}</small>", dataRow["GroupName"] as string, dataRow["GroupPath"] as string );
                        lGroupName.Text = dataRow["GroupName"] as string;
                    }

                    Literal lLocationName = e.Row.FindControl( "lLocationName" ) as Literal;
                    if ( lLocationName != null )
                    {
                        //lLocationName.Text = string.Format( "{0}<br /><small>{1}</small>", dataRow["LocationName"] as string, dataRow["LocationPath"] as string );
                        lLocationName.Text = string.Empty;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            var rockContext = new RockContext();

            GroupLocationService groupLocationService = new GroupLocationService( rockContext );
            ScheduleService scheduleService = new ScheduleService( rockContext );

            //Copied from BindGrid.
            var groupTypeService = new GroupTypeService( rockContext );
            var definedValueService = new DefinedValueService( rockContext );
            var definedTypeId = DefinedTypeCache.Get( SCHEDULE_GROUPS_DEFINED_TYPE ).Id;
            var definedValueQry = definedValueService.Queryable().Where( dv => dv.DefinedTypeId == definedTypeId ).ToList();

            //var qryList = definedValueQry
            //   .Select( dv =>
            //  new
            //  {
            //      DefinedValueId = dv.Id,
            //       //dv.Location,
            //       //GroupId = dv.GroupId,
            //       GroupName = dv.Value, //GroupName = dv.Group.Name,
            //       ScheduleIdList = new List<int>(), //dv.Schedules.Select(s => s.Id),
            //                                         //GroupTypeId = dv.Group.GroupTypeId
            //   } ).ToList();


            //TODO: Refactor into own method; copied from BindGrid.
            int parentLocationId = pkrParentLocation.SelectedValueAsInt() ?? Rock.Constants.All.Id;

            //TODO: Refactor into own method; copied from BindGrid.
            var locationFilteredQuery = groupLocationService.Queryable().Where( gl => gl.LocationId == parentLocationId );
            if( parentLocationId != Rock.Constants.All.Id )
            {
                locationFilteredQuery = locationFilteredQuery.Where( gl => gl.LocationId == parentLocationId );
            }

            var filteredGroups = locationFilteredQuery.Select( gl => gl.Group ).ToList();
            filteredGroups.ForEach( g => g.LoadAttributes() );



            var gridViewRows = gDefinedValueSchedule.Rows;
            //var test = gDefinedValueSchedule.DataSourceAsDataTable;

            //int test3 = 0
            //foreach( DataRow test2 in test.Rows ) test3 = test2["DefinedValueId"];

            foreach ( GridViewRow row in gridViewRows.OfType<GridViewRow>() )
            {
                int rowDefinedValueId = int.Parse( gDefinedValueSchedule.DataKeys[row.RowIndex].Value as string );
                //var rowDefinedValueId = 795;    // WHEN HITTING SAVE, ONLY GROUPS THAT ARE LifeKids Weekend WILL BE AFFECTED.
                //                                // They will get changed for each row, so they will retain the state of the last Row.

                //var groupLocationId = parentLocationId; // This needs to be removed.
                var test = filteredGroups.Where( g => g.Attributes.ContainsKey( "SchedulingGroup" ) );
                //var rowFilteredGroups = filteredGroups.Where( g => g.GetAttributeValue( "SchedulingGroup" ).Contains( DefinedValueCache.Get( rowDefinedValueId ).Guid.ToString() ) ).ToList();
                var rowFilteredGroups = filteredGroups.Where( g => g.Attributes.ContainsKey( "SchedulingGroup" ) && g.GetAttributeValue( "SchedulingGroup" ).Contains( DefinedValueCache.Get( rowDefinedValueId ).Guid.ToString() ) ).ToList();

                List<GroupLocation> rowGLs = rowFilteredGroups.Select( g => g.GroupLocations.Where( gl => gl.LocationId == parentLocationId ).FirstOrDefault() ).ToList();

                GroupLocation groupLocation = groupLocationService.Get( parentLocationId );
                if ( groupLocation != null )
                {
                    foreach( var fieldCell in row.Cells.OfType<DataControlFieldCell>() )
                    {
                        CheckBoxEditableField checkBoxTemplateField = fieldCell.ContainingField as CheckBoxEditableField;
                        if( checkBoxTemplateField != null )
                        {
                            CheckBox checkBox = fieldCell.Controls[0] as CheckBox;
                            string dataField = ( fieldCell.ContainingField as CheckBoxEditableField ).DataField;
                            int scheduleId = int.Parse( dataField.Replace( "scheduleField_", string.Empty ) );

                            // update gDefinedValueSchedule depending on if the Schedule is Checked or not
                            if( checkBox.Checked )
                            {
                                //WorkNote: When the checkbox is checked, look if the represented GroupLocations
                                //          include the schedule.  If they don't include it, add it.
                                foreach(var location in rowGLs)
                                {
                                    if( !location.Schedules.Any( a => a.Id == scheduleId ) )
                                    {
                                        var schedule = scheduleService.Get( scheduleId );
                                        location.Schedules.Add( schedule );
                                    }
                                }
                                // This schedule is selected, so if gDefinedValueSchedule doesn't already have this schedule, add it
                                 //if( !groupLocation.Schedules.Any( a => a.Id == scheduleId ) )
                                 //{
                                      //var schedule = scheduleService.Get( scheduleId );
                                      //groupLocation.Schedules.Add( schedule );
                                 //}
                            }
                            else
                            {
                                //WorkNote: When the checkbox isn't checked, remove the Schedule from any GroupLocations

                                // This schedule is not selected, so if gDefinedValueSchedule has this schedule, delete it
                                foreach(var location in rowGLs)
                                {
                                    if( location.Schedules.Any( a => a.Id == scheduleId ) )
                                    {
                                        location.Schedules.Remove( location.Schedules.FirstOrDefault( a => a.Id == scheduleId ) );
                                    }
                                }
                               
                            }
                        }
                    }
                }
            }

            rockContext.SaveChanges();

            Rock.CheckIn.KioskDevice.Clear();

            if ( _groupTypeId.HasValue )
            {
                NavigateToParentPage( new Dictionary<string, string> { { "CheckinTypeId", _groupTypeId.ToString() } } );
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnCancel_Click( object sender, EventArgs e )
        {
            if ( _groupTypeId.HasValue ) { 
                NavigateToParentPage( new Dictionary<string, string> { { "CheckinTypeId", _groupTypeId.ToString() } } );
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds any needed data to the Grid Filter also using the user's stored
        /// preferences.
        /// </summary>
        private void BindFilter()
        {
            //ddlGroupType.Visible = false;
            //ddlGroupType.Items.Clear();
            //ddlGroupType.Items.Add( Rock.Constants.All.ListItem );

            var rockContext = new RockContext();

            foreach( var groupType in GetTopGroupTypes( rockContext ) )
            //{
            //  ddlGroupType.Items.Add( new ListItem( groupType.Name, groupType.Id.ToString() ) );
            //}
            //ddlGroupType.SetValue( GetBlockUserPreference( "Group Type" ) );

            // hide the GroupType filter if this page has a groupTypeId parameter
            if ( _groupTypeId.HasValue )
            {
                pnlGroupType.Visible = false;
            }
            //int catagorynum = pCategory.SelectedValues.Count();
            int? categoryId = GetBlockUserPreference( "Category" ).AsIntegerOrNull();
            if ( !categoryId.HasValue )
            {
                var categoryCache = CategoryCache.Get( Rock.SystemGuid.Category.SCHEDULE_SERVICE_TIMES.AsGuid() );
                categoryId = categoryCache != null ? categoryCache.Id : (int?)null;
            }

            pCategory.EntityTypeId = EntityTypeCache.GetId( typeof( Rock.Model.Schedule ) ) ?? 0;
            if ( categoryId.HasValue )
            {
                pCategory.SetValue( new CategoryService( rockContext ).Get( categoryId.Value ) );
            }
            else
            {
                pCategory.SetValue( null );
            }

            pkrParentLocation.SetValue( GetBlockUserPreference( "Parent Location" ).AsIntegerOrNull() );
            
        }


        /// <summary>
        /// Binds the grid.
        /// </summary>
        protected void BindGrid()
        {
            AddScheduleColumns();

            var rockContext = new RockContext();

            var definedValueService = new DefinedValueService(rockContext);
            var groupTypeService = new GroupTypeService(rockContext);
            var groupService = new GroupService(rockContext);
            var groupLocationService = new GroupLocationService(rockContext);

            var groupPaths = new List<GroupTypePath>();
            var definedTypeId = DefinedTypeCache.Get( SCHEDULE_GROUPS_DEFINED_TYPE ).Id;
            //var groupLocationQry = definedValueService.Queryable().Where(gl => gl.Group.IsActive && !gl.Group.IsArchived);
            var definedValueQry = definedValueService.Queryable().Where( dv => dv.DefinedTypeId == definedTypeId ).ToList();


            // if this page has a PageParam for groupTypeId use that to limit which groupTypeId to see. Otherwise, use the groupTypeId specified in the filter
            //if (_groupTypeId.HasValue)
            //{
            //    groupTypeId = _groupTypeId.Value;
            //}
            //else
            //{
            //    groupTypeId = ddlGroupType.SelectedValueAsInt() ?? Rock.Constants.All.Id;
            //}

            //if (groupTypeId != Rock.Constants.All.Id)
            //{
            //    var descendantGroupTypeIds = groupTypeService.GetAllAssociatedDescendents(groupTypeId).Select(a => a.Id);

            //    // filter to groups that either are of the GroupType or are of a GroupType that has the selected GroupType as a parent (ancestor)
            //    definedValueQry = definedValueQry.Where(a => a.Group.GroupType.Id == groupTypeId || descendantGroupTypeIds.Contains(a.Group.GroupTypeId));

            //    groupPaths = groupTypeService.GetAllAssociatedDescendentsPath(groupTypeId).ToList();
            //}
            //else
            //{
            //    List<int> descendantGroupTypeIds = new List<int>();
            //    foreach (GroupType groupType in GetTopGroupTypes(rockContext))
            //    {
            //        descendantGroupTypeIds.Add(groupType.Id);

            //        groupPaths.AddRange(groupTypeService.GetAllAssociatedDescendentsPath(groupType.Id).ToList());
            //        foreach (var childGroupType in groupTypeService.GetChildGroupTypes(groupType.Id))
            //        {
            //            descendantGroupTypeIds.Add(childGroupType.Id);
            //            descendantGroupTypeIds.AddRange(groupTypeService.GetAllAssociatedDescendents(childGroupType.Id).Select(a => a.Id).ToList());
            //        }
            //    }

            //    definedValueQry = definedValueQry.Where(a => descendantGroupTypeIds.Contains(a.Group.GroupTypeId));
            //}

            //if (gDefinedValueSchedule.SortProperty != null)
            //{
            //    definedValueQry = definedValueQry.Sort(gDefinedValueSchedule.SortProperty);
            //}
            //else
            //{
            //    definedValueQry = definedValueQry.OrderBy(a => a.Group.Name).ThenBy(a => a.Location.Name);
            //}
           int parentLocationId = pkrParentLocation.SelectedValueAsInt() ?? Rock.Constants.All.Id;
           // var selectedGroups = groupLocationService.Queryable().Where( gl => gl.LocationId == parentLocationId );

           // selectedGroups = selectedGroups.Where( av => av.AttributeValues.ContainsKey( "DefinedValue" ) );

            var qryList = definedValueQry
                .Select( dv =>
               new
               {
                   DefinedValueId = dv.Id,
                   //dv.Location,
                   //GroupId = dv.GroupId,
                   GroupName = dv.Value, //GroupName = dv.Group.Name,
                   ScheduleIdList = new List<int>(), //dv.Schedules.Select(s => s.Id),
                   //GroupTypeId = dv.Group.GroupTypeId
               }).ToList();

            var locationService = new LocationService(rockContext);
            //if (parentLocationId != Rock.Constants.All.Id)
            //{
            //    var currentAndDescendantLocationIds = new List<int>();
            //    currentAndDescendantLocationIds.Add(parentLocationId);
            //    currentAndDescendantLocationIds.AddRange(locationService.GetAllDescendents(parentLocationId).Select(a => a.Id));

            //    qryList = qryList.Where(a => currentAndDescendantLocationIds.Contains(a.Location.Id)).ToList();
            //}

            // put stuff in a DataTable so we can dynamically have columns for each Schedule
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("DefinedValueId");
            //dataTable.Columns.Add("GroupId");
            dataTable.Columns.Add("GroupName");
            //dataTable.Columns.Add("GroupPath");
            //dataTable.Columns.Add("LocationName");
            //dataTable.Columns.Add("LocationPath");
            foreach (var field in gDefinedValueSchedule.Columns.OfType<CheckBoxEditableField>())
            {
                dataTable.Columns.Add(field.DataField, typeof(bool));
            }

            var locationPaths = new Dictionary<int, string>();

            //List<int> descendantGroupTypeIds = new List<int>();
            //if( groupTypeId != Rock.Constants.All.Id )
            //{
            //    //foreach( GroupType groupType in GetTopGroupTypes( rockContext ) )
            //    //{
            //    //    descendantGroupTypeIds.Add( groupType.Id );

            //    //    groupPaths.AddRange( groupTypeService.GetAllAssociatedDescendentsPath( groupType.Id ).ToList() );
            //    //    foreach( var childGroupType in groupTypeService.GetChildGroupTypes( groupType.Id ) )
            //    //    {
            //    //        descendantGroupTypeIds.Add( childGroupType.Id );
            //    //        descendantGroupTypeIds.AddRange( groupTypeService.GetAllAssociatedDescendents( childGroupType.Id ).Select( a => a.Id ).ToList() );
            //    //    }
            //    //}

            //    // filter to groups that either are of the GroupType or are of a GroupType that has the selected GroupType as a parent (ancestor)
            //    //definedValueQry = definedValueQry.Where( a => a.Group.GroupType.Id == groupTypeId || descendantGroupTypeIds.Contains( a.Group.GroupTypeId ) );

            //    //groupPaths = groupTypeService.GetAllAssociatedDescendentsPath( groupTypeId ).ToList();
            //}

            // Find all Groups that meet at the filtered location
            var locationFilteredQuery = groupLocationService.Queryable().Where( gl =>  gl.LocationId == parentLocationId );
            if ( parentLocationId != Rock.Constants.All.Id )
            {
                locationFilteredQuery = locationFilteredQuery.Where( gl => gl.LocationId == parentLocationId );
            }

            // Unnecessary since we will only affect groups that match the selected Defined Value for the Row
            //var checkInTemplateId = DefinedValueCache.GetId( Rock.SystemGuid.DefinedValue.GROUPTYPE_PURPOSE_CHECKIN_FILTER.AsGuid() );
            //locationFilteredQuery = locationFilteredQuery.Where( gl => gl.Group.GroupType.GroupTypePurposeValueId == checkInTemplateId );

            var filteredGroups = locationFilteredQuery.Select( gl => gl.Group ).ToList();
            filteredGroups.ForEach( g => g.LoadAttributes() );

            foreach (var row in qryList)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["DefinedValueId"] = row.DefinedValueId;
                dataRow["GroupName"] = row.GroupName;
                
                var rowFilteredGroups = filteredGroups.Where( g => g.GetAttributeValues( "SchedulingGroup" ).Contains( DefinedValueCache.Get( row.DefinedValueId ).Guid.ToString() ) ).ToList();

                foreach (var field in gDefinedValueSchedule.Columns.OfType<CheckBoxEditableField>())
                {
                    int scheduleId = int.Parse( field.DataField.Replace( "scheduleField_", string.Empty ) );
                    var filteredGroupsWithSchedule = rowFilteredGroups.Select(
                                g => g.GroupLocations.Where(
                                    gl => gl.LocationId == parentLocationId
                                ).FirstOrDefault()
                            ).Where(
                                gl => gl.Schedules.Any( gls => gls.Id == scheduleId )
                            ).Select(
                                gl => gl.Group
                            );

                    bool scheduleSelected = filteredGroupsWithSchedule.Count() == rowFilteredGroups.Count && rowFilteredGroups.Count > 0;

                    //dataRow[field.DataField] = row.ScheduleIdList.Any(a => a == scheduleId);
                    dataRow[field.DataField] = scheduleSelected;
                }

                dataTable.Rows.Add(dataRow);
            }

            gDefinedValueSchedule.EntityTypeId = EntityTypeCache.Get<DefinedType>().Id;
            gDefinedValueSchedule.DataSource = dataTable;
            gDefinedValueSchedule.DataBind();
        }


        /// <summary>
        /// Adds the schedule columns.
        /// </summary>
        private void AddScheduleColumns()
        {
            ScheduleService scheduleService = new ScheduleService( new RockContext() );

            // limit Schedules to ones that are Active and have a CheckInStartOffsetMinutes
            var scheduleQry = scheduleService.Queryable().Where( a => a.IsActive && a.CheckInStartOffsetMinutes != null );

            // limit Schedules to the Category from the Filter
            List<int> scheduleCategoryIds = pCategory.SelectedValuesAsInt().ToList(); //?? Rock.Constants.All.Id;
            if ( !scheduleCategoryIds.Contains( Rock.Constants.All.Id ) )
            {
                scheduleQry = scheduleQry.Where( s => scheduleCategoryIds.Contains( s.CategoryId ?? 0 ) );

                //foreach (var category in scheduleCategoryIds)
                //{
                //    scheduleQry = scheduleQry.Where( a => a.CategoryId == category );

                //}
                //scheduleQry = scheduleQry.Where( a => a.CategoryId == scheduleCategoryId );

            }
            else
            {
                // NULL (or 0) means Shared, so specifically filter so to show only Schedules with CategoryId NULL
                scheduleQry = scheduleQry.Where( a => a.CategoryId == null );
            }

            var checkBoxEditableFields = gDefinedValueSchedule.Columns.OfType<CheckBoxEditableField>().ToList();
            foreach ( var field in checkBoxEditableFields )
            {
                gDefinedValueSchedule.Columns.Remove( field );
            }

            // clear out any existing schedule columns and add the ones that match the current filter setting
            var scheduleList = scheduleQry.ToList();
            //var scheduleList2 = scheduleList.ToList().OrderBy( a => a.Name ).ToList();

            var occurrenceDate = RockDateTime.Now.SundayDate().AddDays( 1 );
            var sortedScheduleList = scheduleList.
                OrderBy( a => a.CategoryId )
                .ThenBy( a => a.GetNextStartDateTime( occurrenceDate ) )
                .ThenBy( a => a.Name )
                .ThenBy( a => a.Id )
                .ToList(); ;

            foreach ( var item in sortedScheduleList )
            {
                string dataFieldName = string.Format( "scheduleField_{0}", item.Id );

                CheckBoxEditableField field = new CheckBoxEditableField { HeaderText = item.Name + "<br /><a href='#' style='display: inline' class='fa fa-square-o js-sched-select-all'></a>", DataField = dataFieldName };
                field.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                gDefinedValueSchedule.Columns.Add(field);
            }

            if ( !scheduleList.Any() )
            {
                nbNotification.Text = nbNotification.Text = String.Format( "<p><strong>Warning</strong></p>No schedules found. Consider <a class='alert-link' href='{0}'>adding a schedule</a> or a different schedule category.", ResolveUrl( "~/Schedules" ) );
                nbNotification.Visible = true;
            }
            else
            {
                nbNotification.Visible = false;
            }
        }

        private List<GroupType> GetTopGroupTypes( RockContext rockContext )
        {
            var groupTypes = new List<GroupType>();

            // populate the GroupType DropDownList only with GroupTypes with GroupTypePurpose of Check-in Template
            // or with group types that allow multiple locations/schedules and support named locations
            int groupTypePurposeCheckInTemplateId = DefinedValueCache.Get( new Guid( Rock.SystemGuid.DefinedValue.GROUPTYPE_PURPOSE_CHECKIN_TEMPLATE ) ).Id;
            GroupTypeService groupTypeService = new GroupTypeService( rockContext );

            // First find all the group types that have a purpose of 'Check-in Template'
            var checkInGroupTypeIds = groupTypeService.Queryable()
                .Where( t =>
                    t.GroupTypePurposeValueId.HasValue &&
                    t.GroupTypePurposeValueId.Value == groupTypePurposeCheckInTemplateId )
                .Select( t => t.Id )
                .ToList();

            // Now find all their descendants (so we can exclude them in a sec)
            var descendentGroupTypeIds = new List<int>();
            foreach ( int id in checkInGroupTypeIds )
            {
                descendentGroupTypeIds.AddRange( groupTypeService.GetAllAssociatedDescendents( id ).Select( a => a.Id ).ToList() );
            }

            // Now query again for all the types that have a purpose of 'Check-in Template' or support check-in outside of being a descendant of the template
            var groupTypeList = groupTypeService.Queryable()
                .Where( a =>
                    checkInGroupTypeIds.Contains( a.Id ) ||
                    (
                        !descendentGroupTypeIds.Contains( a.Id ) &&
                        a.AllowMultipleLocations &&
                        a.EnableLocationSchedules.HasValue &&
                        a.EnableLocationSchedules.Value &&
                        a.LocationTypes.Any()
                    ) )
                .OrderBy( a => a.Order )
                .ThenBy( a => a.Name )
                .ToList();

            foreach ( var groupType in groupTypeList )
            {
                // Make sure the group type supports named locations (we can't query on this in the above qry)
                if ( groupType.GroupTypePurposeValueId == groupTypePurposeCheckInTemplateId ||
                    ( groupType.LocationSelectionMode & GroupLocationPickerMode.Named ) == GroupLocationPickerMode.Named )
                {
                    groupTypes.Add( groupType );
                }
            }

            return groupTypes;
        }

        #endregion

    }
}