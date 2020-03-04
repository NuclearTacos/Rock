<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GroupTypeListLC.ascx.cs" Inherits="RockWeb.Plugins.Groups.GroupTypeListLC" %>

<asp:UpdatePanel ID="upGroupType" runat="server">
    <ContentTemplate>
        
        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-sitemap"></i> Group Type List</h1>
            </div>
            <div class="panel-body">

                <div class="grid grid-panel">
                    <Rock:GridFilter ID="rFilter" runat="server" OnDisplayFilterValue="rFilter_DisplayFilterValue">
                        <Rock:DefinedValuePicker ID="dvpPurpose" runat="server" Label="Purpose" />
                        <Rock:RockDropDownList ID="ddlIsSystem" runat="server" Label="System Group Type">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>
                        <Rock:RockDropDownList ID="ddlShowInNavigation" runat="server" Label="Shown in Navigation">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                       <Rock:DefinedValuePicker ID="dvpInheritedGroupType" runat="server" Label="Inherited Group Type" />

                        <Rock:DefinedValuePicker ID="dvpLocationSelectionModes" runat="server" Label="Location Selection Modes" />

                       <Rock:RockDropDownList ID="ddlAllowMultipleLocations" runat="server" Label="Multiple Locations">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                         <Rock:RockDropDownList ID="ddlEnableLocationSchedules" runat="server" Label="Location Schedules">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                        <Rock:RockDropDownList ID="ddlAllowSpecificGroupMemberAttributes" runat="server" Label="Specific Group Member Attributes">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                         <Rock:RockDropDownList ID="ddlEnableSpecificGroupRequirements" runat="server" Label="Enable Specific Group Requirements">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                         <Rock:RockDropDownList ID="ddlEnableGroupHistory" runat="server" Label="Enable Group History">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                        <Rock:RockDropDownList ID="ddlTakesAttendance" runat="server" Label="Takes Attendance">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                         <Rock:RockDropDownList ID="ddlWeekendService" runat="server" Label="Weekend Service">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                         <Rock:RockDropDownList ID="ddlGroupAttendanceRequiresLocation" runat="server" Label="RequiresLocation">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                        <Rock:DefinedValuePicker ID="dvpScheduleTypes" runat="server" Label="Schedule Types" />

                        <Rock:RockDropDownList ID="ddlGroupAttendanceRequiresSchedule" runat="server" Label="RequiresSchedule">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                        <Rock:DefinedValuePicker ID="dvpAttendanceRule" runat="server" Label="Attendance Rule" />

                        <Rock:DefinedValuePicker ID="dvpAttendancePrintTo" runat="server" Label="Print To" />

                        <Rock:RockDropDownList ID="ddlShowInGroupList" runat="server" Label="Show in Group List">
                            <asp:ListItem Value="" Text=" " />
                            <asp:ListItem Value="Yes" Text="Yes" />
                            <asp:ListItem Value="No" Text="No" />
                        </Rock:RockDropDownList>

                    </Rock:GridFilter>
                    <Rock:ModalAlert ID="mdGridWarning" runat="server" />
                    <Rock:Grid ID="gGroupType" runat="server" RowItemText="Group Type" OnRowSelected="gGroupType_Edit" TooltipField="Description">
                        <Columns>
                            <Rock:ReorderField />
                            <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                            <Rock:RockBoundField DataField="Purpose" HeaderText="Purpose" SortExpression="Purpose" />
                            <Rock:RockBoundField DataField="GroupsCount" HeaderText="Group Count" SortExpression="GroupsCount" />
                            <Rock:BoolField DataField="ShowInNavigation" HeaderText="Show in Navigation" SortExpression="ShowInNavigation" />
                            <Rock:BoolField DataField="IsSystem" HeaderText="System" SortExpression="IsSystem" />

                            <Rock:RockBoundField DataField="InheritedGroupType" HeaderText="Inherited Group Type" SortExpression="InheritedGroupType" />
                            <Rock:RockBoundField Datafield="LocationSelectionMode" HeaderText="Location Selection Modes" SortExpression="LocationSelectionModes" />
                            <Rock:BoolField DataField="AllowMultipleLocations" HeaderText="Allow Multiple Locations" SortExpression="AllowMultipleLocations" />
                            <Rock:BoolField DataField="EnableLocationSchedules" HeaderText="Enable Location Schedules" SortExpression="EnableLocationSchedules" />
                            <Rock:BoolField DataField="AllowSpecificGroupMemberAttributes" HeaderText="Allow Specific Group Member Attributes" SortExpression="AllowSpecificGroupMemberAttributes" />
                            <Rock:BoolField DataField="EnableSpecificGroupRequirements" HeaderText="Enable Specific Group Requirements" SortExpression="EnableSpecificGroupRequirements" />
                            <Rock:BoolField DataField="EnableGroupHistory" HeaderText="Enable Group History" SortExpression="EnableGroupHistory" />
                            <Rock:BoolField DataField="TakesAttendance" HeaderText="Takes Attendance" SortExpression="TakesAttendance" />
                            <Rock:BoolField DataField="AttendanceCountsAsWeekendService" HeaderText="Weekend Service" SortExpression="WeekendService" />
                            <Rock:BoolField DataField="GroupAttendanceRequiresLocation" HeaderText="Requires Location" SortExpression="GroupAttendanceRequiresLocation" />
                            <Rock:RockBoundField Datafield="AllowedScheduleTypes" HeaderText="Group Schedule Options" SortExpression="AllowedScheduleTypes" />
                            <Rock:BoolField DataField="GroupAttendanceRequiresSchedule" HeaderText="Requires Schedule" SortExpression="GroupAttendanceRequiresSchedule" />
                            <Rock:RockBoundField Datafield="AttendanceRule" HeaderText="Check-in Rule" SortExpression="AttendanceRule" />
                            <Rock:RockBoundField Datafield="AttendancePrintTo" HeaderText="Print Using" SortExpression="AttendancePrintTo" />
                            <Rock:BoolField DataField="ShowInGroupList" HeaderText="Show in Group List" SortExpression="ShowInGroupList" />


                            <Rock:SecurityField TitleField="Name" />
                            <Rock:DeleteField OnClick="gGroupType_Delete" />
                        </Columns>
                    </Rock:Grid>
                </div>

            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
