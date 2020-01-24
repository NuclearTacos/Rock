<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FlexibleWorkflowEntry.ascx.cs" Inherits="RockWeb.Plugins.church_life.WorkFlow.FlexibleWorkflowEntry" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <div class="row">
            <div id="divForm" runat="server" class="col-md-6">

                <div class="panel panel-block">

                    <div class="panel-heading">
                        <h1 class="panel-title">
                            <asp:Literal ID="lIconHtml" runat="server" ><i class="fa fa-gear"></i></asp:Literal>
                            <asp:Literal ID="lTitle" runat="server" >Workflow Entry</asp:Literal>
                        </h1>
                        <div class="panel-labels">
                            <Rock:HighlightLabel ID="hlblWorkflowId" runat="server" LabelType="Info" />
                            <Rock:HighlightLabel ID="hlblDateAdded" runat="server" LabelType="Default" />
                        </div>
                    </div>
                    <div class="panel-body">

                        <asp:Literal ID="lSummary" runat="server" Visible="false" />

                        <asp:Panel ID="pnlForm" CssClass="workflow-entry-panel" runat="server">

                            <asp:ValidationSummary ID="vsDetails" runat="server" HeaderText="Please correct the following:" CssClass="alert alert-validation" />

                            <asp:Literal ID="lheadingText" runat="server" />

                            <%--<asp:PlaceHolder ID="phAttributes" runat="server" EnableViewState ="true" ViewStateMode="Enabled"/>--%>
                            <asp:PlaceHolder ID="phAttributes" runat="server" />

                            <%--<asp:Repeater ID="rptFields" runat="server" >
                                <ItemTemplate>
                                    <div class="row padding-v-lg">
                                        <div class="col-sm-3 margin-b-lg">
                                            <asp:Literal ID="lFamily" runat="server" />
                                        </div>
                                        <div class="col-sm-9">
                                            <asp:Repeater ID="rptFamilyPeople" runat="server" >
                                                <ItemTemplate>
                                                    <div class="row margin-b-md">
                                                        <asp:Literal ID="lPerson" runat="server" />
                                                        <asp:Literal ID="lPhones" runat="server" />
                                                        <asp:Literal ID="lEverythingElse" runat="server" />
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>--%>
            
                            <asp:Literal ID="lFootingText" runat="server" />

                            <div class="actions">
                                <asp:PlaceHolder ID="phActions" runat="server" />
                            </div>

                        </asp:Panel>

                        <Rock:NotificationBox ID="nbMessage" runat="server" Dismissable="true" CssClass="margin-t-lg" />

                    </div>

                </div>

            </div>

            <div id="divNotes" runat="server" class="col-md-6" visible ="false">

                <Rock:NoteContainer ID="ncWorkflowNotes" runat="server" NoteLabel="Note" 
                    ShowHeading="true" Title="Notes" TitleIconCssClass="fa fa-comment"
                    DisplayType="Full" UsePersonIcon="false" ShowAlertCheckBox="true"
                    ShowPrivateCheckBox="false" ShowSecurityButton="false"
                    AllowAnonymousEntry="false" AddAlwaysVisible="false"
                    SortDirection="Descending" Visible="false"/>
               

            </div>

        </div>

    </ContentTemplate>
</asp:UpdatePanel>
