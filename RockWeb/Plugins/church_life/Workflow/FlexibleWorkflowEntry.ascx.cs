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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Rock.Security;
using Rock.Workflow;
using Rock.Field;
using Newtonsoft.Json;
using Rock.Lava;

namespace RockWeb.Plugins.church_life.WorkFlow
{
    /// <summary>
    /// Used to enter information for a workflow form entry action.
    /// </summary>
    [DisplayName( "Flexible Workflow Entry" )]
    [Category( "Life.Church > WorkFlow" )]
    [Description( "Used to enter information for a workflow form entry action." )]

    #region Block Attributes

    [WorkflowTypeField(
        "Workflow Type",
        Description = "Type of workflow to start.",
        Key = AttributeKey.WorkflowType,
        Order = 0)]
    [BooleanField(
        "Show Summary View",
        Description = "If workflow has been completed, should the summary view be displayed?",
        Key = AttributeKey.ShowSummaryView,
        Order = 1 )]
    [CodeEditorField(
        "Block Title Template",
        Description = "Lava template for determining the title of the block. If not specified, the name of the Workflow Type will be shown.",
        Key = AttributeKey.BlockTitleTemplate,
        EditorMode = CodeEditorMode.Lava,
        EditorTheme = CodeEditorTheme.Rock,
        EditorHeight = 100,
        IsRequired = false,
        Order = 2)]
    [TextField(
        "Block Title Icon CSS Class",
        Description = "The CSS class for the icon displayed in the block title. If not specified, the icon for the Workflow Type will be shown.",
        Key = AttributeKey.BlockTitleIconCssClass,
        IsRequired = false,
        Order = 3 )]
    [CodeEditorField(
        "Field Configuration",
        Description = "JSON configuration for the field payload.",
        Key = AttributeKey.WorkflowFieldConfiguration,
        EditorMode = CodeEditorMode.JavaScript,
        EditorTheme = CodeEditorTheme.Rock,
        EditorHeight = 500,
        IsRequired = true,
        DefaultValue = @"{
    ""Fields"": [
        {
            ""FieldName"": ""Q1"",
            ""AttributeKey"": ""Q1"",
            ""Prompt"": ""This is the text for question 1"",
            ""Helptext"": ""This is the helptext"",
            ""Required"": true,
            ""RequiredErrorText"": ""Please respond to question 1"",
            ""PostbackOnChange"": true,
            ""FieldType"": ""Radio"",
            ""FieldConfiguration"": {
                ""Rows"": 1,
                ""Style"": """",
                ""Options"": [
                    ""Agree"",
                    ""Disagree""
                ]
            },
            ""RevealCondition"": ""{% if FormState.CurrentPage == 1 %}true{% else %}false{% endif %}"",
            ""Order"": 0,
            ""ResponseValue"": """"
        },
        {
            ""FieldName"": ""Q1e"",
            ""AttributeKey"": ""Q1e"",
            ""Prompt"": ""Please explain why you disagree to question 1"",
            ""Helptext"": ""This is the helptext"",
            ""Required"": true,
            ""RequiredErrorText"": ""Please explain your response to question 1"",
            ""PostbackOnChange"": false,
            ""FieldType"": ""Text"",
            ""FieldConfiguration"": {
                ""Rows"": 3
            },
            ""RevealCondition"": ""{% unless FormState.CurrentPage == 1 and Q1.ResponseValue == 'Disagree' %}false{% else %}true{% endunless %}"",
            ""Order"": 1,
            ""ResponseValue"": """"
        },
        {
            ""FieldName"": ""Q2"",
            ""AttributeKey"": ""Q2"",
            ""Prompt"": ""This is the text for question 2"",
            ""Helptext"": ""This is the helptext"",
            ""Required"": true,
            ""RequiredErrorText"": ""Please respond to question 2"",
            ""PostbackOnChange"": true,
            ""FieldType"": ""Radio"",
            ""FieldConfiguration"": {
                ""Rows"": 1,
                ""Options"": [
                    ""Agree"",
                    ""Disagree""
                ]
            },
            ""RevealCondition"": ""{% if FormState.CurrentPage == 2 %}true{% else %}false{% endif %}"",
            ""Order"": 0,
            ""ResponseValue"": """"
        },
        {
            ""FieldName"": ""Q2e"",
            ""AttributeKey"": ""Q2e"",
            ""Prompt"": ""Please explain your response to question 2"",
            ""Helptext"": ""This is the helptext"",
            ""Required"": true,
            ""RequiredErrorText"": ""Please respond to question 2"",
            ""PostbackOnChange"": false,
            ""FieldType"": ""Text"",
            ""FieldConfiguration"": {
                ""Rows"": 3
            },
            ""RevealCondition"": ""{% unless FormState.CurrentPage == 2 and Q2.ResponseValue == 'Disagree' %}false{% else %}true{% endunless %}"",
            ""Order"": 1,
            ""ResponseValue"": """"
        },
        {
            ""FieldName"": ""Date"",
            ""AttributeKey"": ""Date"",
            ""Prompt"": ""Please enter a date"",
            ""Helptext"": ""The Date of the thing"",
            ""Required"": true,
            ""RequiredErrorText"": ""The Date is Required"",
            ""PostbackOnChange"": false,
            ""FieldType"": ""Date"",
            ""FieldConfiguration"": {
            },
            ""RevealCondition"": ""{% unless FormState.CurrentPage == 3 %}false{% else %}true{% endunless %}"",
            ""Order"": 0,
            ""ResponseValue"": """"
        }
    ],
    ""Actions"": [
        {
            ""ActionName"": ""Next"",
            ""ButtonText"": ""Next"",
            ""HtmlButtonValueId"": 242,
            ""RevealCondition"": ""{% if FormState.CurrentPage == 1 %}true{% else %}false{% endif %}"",
            ""DestinationPage"": 2,
            ""ActivityActivate"": [
                72,
                73
            ],
            ""ActivityComplete"": [
                71
            ],
            ""StartWorkflow"": false,
            ""GoToSuccessPage"": true,
            ""Order"": 0
        },
        {
            ""ActionName"": ""Submit"",
            ""ButtonText"": ""Submit"",
            ""HtmlButtonValueId"": 242,
            ""RevealCondition"": ""{% if FormState.CurrentPage == 2 %}true{% else %}false{% endif %}"",
            ""DestinationPage"": 3,
            ""ActivityActivate"": [
                72,
                73
            ],
            ""ActivityComplete"": [
                71
            ],
            ""StartWorkflow"": true,
            ""GoToSuccessPage"": true,
            ""Order"": 0
        }
    ],
    ""CurrentPage"": 1
}",
        Order = 4)]
    [LinkedPage("Success Page", "The page that the user will be direted to when they click an action that uses the success page", true, Order = 5) ]

    #endregion


    public partial class FlexibleWorkflowEntry : Rock.Web.UI.RockBlock, IPostBackEventHandler
    {
        #region Attribute Keys

        /// <summary>
        /// Keys to use for Block Attributes
        /// </summary>
        private static class AttributeKey
        {
            public const string WorkflowType = "WorkflowType";
            public const string ShowSummaryView = "ShowSummaryView";
            public const string BlockTitleTemplate = "BlockTitleTemplate";
            public const string BlockTitleIconCssClass = "BlockTitleIconCssClass";
            public const string WorkflowFieldConfiguration = "WorkflowFieldConfiguration";
        }

        #endregion Attribute Keys


        #region PCOOs
        public class Field: ILiquidizable
        {
            [Rock.Data.LavaIgnore]
            public object this[object key]
            {
                get
                {
                    //return "Lava+Field";
                    switch (key.ToStringSafe())
                    {
                        case "FieldName": return FieldName;
                        case "AttributeKey": return AttributeKey;
                        case "Prompt": return Prompt;
                        case "HelpText": return HelpText;
                        case "Required": return Required;
                        case "RequiredErrorText": return RequiredErrorText;
                        case "FieldConfiguration": return FieldConfiguration[key];
                        case "RevealCondition": return RevealCondition;
                        case "Order": return Order;
                        case "ResponseValue": return ResponseValue;
                        default: return "Lava+Field";
                    }
                }
            }

            public string FieldName { get; set; }
            public string AttributeKey { get; set; }
            public string Prompt { get; set; }
            public string HelpText { get; set; }
            public bool Required { get; set; }
            public string RequiredErrorText { get; set; }
            public bool PostbackOnChange { get; set; }
            public string FieldType { get; set; }
            public FieldConfiguration FieldConfiguration { get; set; }
            public string RevealCondition { get; set; }
            public int Order { get; set; }
            public string ResponseValue { get; set; }

            [Rock.Data.LavaIgnore]
            public List<string> AvailableKeys
            {
                get
                {
                    var availableKeys = new List<string> { "FieldName", "AttributeKey", "Prompt", "HelpText", "Required", "RequiredErrorText", "PostbackOnChange", "FieldType", "FieldConfiguration", "RevealCondition", "Order", "ResponseValue" };
                    if (this.FieldConfiguration != null)
                    {
                        availableKeys.AddRange(this.FieldConfiguration.AvailableKeys);
                    }
                    return availableKeys;
                }
            }

            public bool ContainsKey(object key)
            {
                if (this.AvailableKeys.Contains(key.ToStringSafe()))
                {
                    return true;
                }
                return false;
            }

            public object ToLiquid()
            {
                return this;
            }
        }

        public class Action: ILiquidizable
        {
            [Rock.Data.LavaIgnore]
            public object this[object key]
            {
                get
                {
                    switch (key.ToStringSafe())
                    {
                        // return the values for all of the properties
                        case "ActionName": return ActionName;
                        case "ButtonText": return ButtonText;
                        case "ButtonType": return HtmlButtonValueId;
                        case "RevealCondition": return RevealCondition;
                        case "DestinationPage": return DestinationPage;
                        case "ActivityActivate": return ActivityActivate;
                        case "ActivityComplete": return ActivityComplete;
                        case "StartWorkflow": return StartWorkflow;
                        case "GoToSuccessPage": return GoToSuccessPage;
                        case "Order": return Order;
                        default: return ButtonText;
                    }
                }
            }

            //public IList<Field> Fields { get; set; }
            public string ActionName { get; set; }
            public string ButtonText { get; set; }
            public int HtmlButtonValueId { get; set; }
            public string RevealCondition { get; set; }
            public int DestinationPage { get; set; }
            public int[] ActivityActivate { get; set; } // Not implemented
            public int[] ActivityComplete { get; set; } // Not implemented
            public bool StartWorkflow { get; set; }
            public bool GoToSuccessPage { get; set; }
            public int Order { get; set; }


            [Rock.Data.LavaIgnore]
            public List<string> AvailableKeys
            {
                get
                {
                    var availableKeys = new List<string> { "ButtonText", "ActionName", "HtmlButtonValueId", "RevealCondition", "DestinationPage", "ActivityActivate", "StartWorkflow", "GoToSuccessPage", "Order" };
                    return availableKeys;
                }
            }

            public bool ContainsKey(object key)
            {
                if (this.AvailableKeys.Contains(key.ToStringSafe()))
                {
                    return true;
                }
                return false;
            }

            public object ToLiquid()
            {
                return this;
            }
        }

        public class FormState: ILiquidizable
        {
            [Rock.Data.LavaIgnore]
            public object this[object key]
            {
                get
                {
                    switch (key.ToStringSafe())
                    {
                        case "Fields": return Fields;
                        case "Actions": return Actions;
                        case "CurrentPage": return CurrentPage;
                        default: return "Lava+Field";
                    }
                }
            }

            public IList<Field> Fields { get; set; }
            public IList<Action> Actions { get; set; }
            public int CurrentPage { get; set; }

            [Rock.Data.LavaIgnore]
            public List<string> AvailableKeys
            {
                get
                {
                    var availableKeys = new List<string> { "Fields", "Actions", "CurrentPage" };
                    if (this.Fields.First().AvailableKeys != null)
                    {
                        availableKeys.AddRange( this.Fields.First().AvailableKeys );
                    }
                    if (this.Actions.First().AvailableKeys != null)
                    {
                        availableKeys.AddRange(this.Fields.First().AvailableKeys);
                    }
                    return availableKeys;
                }
            }

            public bool ContainsKey(object key)
            {
                if ( this.AvailableKeys.Contains(key.ToStringSafe()))
                {
                    return true;
                }
                return false;
            }

            public object ToLiquid()
            {
                return this;
            }
        }

        public class FieldConfiguration: ILiquidizable
        {
            [Rock.Data.LavaIgnore]
            public object this[object key]
            {
                get
                {
                    //return "Lava+Field";
                    switch (key.ToStringSafe())
                    {
                        case "Rows": return Rows;
                        case "Style": return Style;
                        case "Options": return Options;
                        default: return "Lava+Field";
                    }
                }
            }

            public int Rows { get; set; }
            public string Style { get; set; }
            public IList<string> Options { get; set; }

            [Rock.Data.LavaIgnore]
            public List<string> AvailableKeys
            {
                get
                {
                    var availableKeys = new List<string> { "Rows", "Options" };
                    return availableKeys;
                }
            }

            public bool ContainsKey(object key)
            {
                if (this.AvailableKeys.Contains(key.ToStringSafe()))
                {
                    return true;
                }
                return false;
            }

            public object ToLiquid()
            {
                return this;
            }
        }

        // Not suitable for Lava
        public class AddressValue
        {
            public string Street1 { get; set; }
            public string Street2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string PostalCode { get; set; }
        }
        #endregion

        #region Fields

        private RockContext _rockContext = null;
        private WorkflowService _workflowService = null;

        private WorkflowTypeCache _workflowType = null;
        private WorkflowActionTypeCache _actionType = null;
        private Workflow _workflow = null;
        private WorkflowActivity _activity = null;
        private WorkflowAction _action = null;

        private FormState _formState = null;
        private List<Control> _formControls = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the workflow type identifier.
        /// </summary>
        /// <value>
        /// The workflow type identifier.
        /// </value>
        public int? WorkflowTypeId
        {
            get { return ViewState["WorkflowTypeId"] as int?; }
            set { ViewState["WorkflowTypeId"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow type was set by attribute.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [configured type]; otherwise, <c>false</c>.
        /// </value>
        public bool ConfiguredType
        {
            get { return ViewState["ConfiguredType"] as bool? ?? false; }
            set { ViewState["ConfiguredType"] = value; }
        }

        /// <summary>
        /// Gets or sets the workflow identifier.
        /// </summary>
        /// <value>
        /// The workflow identifier.
        /// </value>
        public int? WorkflowId
        {
            get { return ViewState["WorkflowId"] as int?; }
            set { ViewState["WorkflowId"] = value; }
        }

        /// <summary>
        /// Gets or sets the action type identifier.
        /// </summary>
        /// <value>
        /// The action type identifier.
        /// </value>
        public int? ActionTypeId
        {
            get { return ViewState["ActionTypeId"] as int?; }
            set { ViewState["ActionTypeId"] = value; }
        }

        #endregion

        #region Base Control Methods

        //  overrides of the base RockBlock methods (i.e. OnInit, OnLoad)

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            // Load the formState from viewstate and decode from JSON.
            var formState = ( string ) ViewState["FormState"];
            _formState = formState.FromJsonOrNull<FormState>();

            BuildForm(false);

            //if (HydrateObjects())
            //{
            //    BuildForm(false);
            //}
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );

            _formControls = new List<Control>();

            SetBlockTitle();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            nbMessage.Visible = false;

            if ( !Page.IsPostBack )
            {
                try
                {
                    var configJson = GetAttributeValue( AttributeKey.WorkflowFieldConfiguration );
                    _formState = configJson.FromJsonOrNull<FormState>();


                }
                catch { }

                {
                    //ProcessActionRequest();
                    UpdateFieldsList();
                    BuildForm( true );
                }
            }
            else
            {
                UpdateFieldsList();
                BuildForm(true);
            }
        }

        /// <summary>
        /// Saves the state of the view.
        /// </summary>
        /// <returns></returns>
        protected override object SaveViewState()
        {
            // Encode the form state in JSON since the actual object is
            // probably not marked as ISerializable.
            ViewState["FormState"] = _formState.ToJson();

            return base.SaveViewState();
        }

        private void UpdateFieldsList()
        {
            foreach ( var field in _formState.Fields)
            {
                var control = phAttributes.FindControl( field.FieldName );
                try
                {
                    field.ResponseValue = ( ( CampusPicker ) control ).SelectedCampusId.ToString() ?? "";
                }
                catch
                {
                    try
                    {
                        var addressControl = ( ( AddressControl ) control );
                        var addressValues = new AddressValue
                        {
                            Street1 = addressControl.Street1,
                            Street2 = addressControl.Street2,
                            City = addressControl.City,
                            State = addressControl.State,
                            Country = addressControl.Country,
                            PostalCode = addressControl.PostalCode
                        };
                        field.ResponseValue = addressValues.ToJson();
                    }
                    catch
                    { 
                        try
                        {
                            field.ResponseValue = ( ( RockTextBox ) control ).Text ?? "";
                        }
                        catch
                        {
                            try
                            {
                                field.ResponseValue = ( ( RockRadioButtonList ) control ).SelectedValue ?? "";
                            }
                            catch
                            {
                                try
                                {
                                    field.ResponseValue = ( ( SSNBox ) control ).TextEncrypted ?? "";
                                }
                                catch
                                {
                                    try
                                    {
                                        field.ResponseValue = ( ( RockDropDownList ) control ).SelectedValue ?? "";
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            var cbControl = ( ( RockCheckBoxList ) control );
                                            field.ResponseValue = cbControl.SelectedValues.ToJson() ?? "";
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                field.ResponseValue = ( ( PhoneNumberBox ) control ).Number ?? "";
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns breadcrumbs specific to the block that should be added to navigation
        /// based on the current page reference.  This function is called during the page's
        /// oninit to load any initial breadcrumbs.
        /// </summary>
        /// <param name="pageReference">The <see cref="Rock.Web.PageReference" />.</param>
        /// <returns>
        /// A <see cref="System.Collections.Generic.List{BreadCrumb}" /> of block related <see cref="Rock.Web.UI.BreadCrumb">BreadCrumbs</see>.
        /// </returns>
        public override List<BreadCrumb> GetBreadCrumbs( Rock.Web.PageReference pageReference )
        {
            var breadCrumbs = new List<BreadCrumb>();

            LoadWorkflowType();

            if ( _workflowType != null && !ConfiguredType )
            {
                breadCrumbs.Add( new BreadCrumb( _workflowType.Name, pageReference ) );
            }

            return breadCrumbs;
        }

        protected override void Render( HtmlTextWriter writer )
        {
            base.Render( writer );
        }
        #endregion

        #region Events

        // handlers called by the controls on your block

        /// <summary>
        /// Handles the BlockUpdated event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            SetBlockTitle();
        }

        /// <summary>
        /// When implemented by a class, enables a server control to process an event raised when a form is posted to the server.
        /// </summary>
        /// <param name="eventArgument">A <see cref="T:System.String" /> that represents an optional event argument to be passed to the event handler.</param>
        public void RaisePostBackEvent( string eventArgument )
        {
            GetFormValues();
            CompleteFormAction( eventArgument );
        }

        #endregion

        #region Methods

        private bool HydrateObjects()
        {
            LoadWorkflowType();

            // Set the note type if this is first request
            if ( !Page.IsPostBack )
            {
                var entityType = EntityTypeCache.Get( typeof( Rock.Model.Workflow ) );
                var noteTypes = NoteTypeCache.GetByEntity( entityType.Id, string.Empty, string.Empty );
                ncWorkflowNotes.NoteOptions.SetNoteTypes( noteTypes );
            }

            if ( _workflowType == null )
            {
                ShowNotes( false );
                ShowMessage( NotificationBoxType.Danger, "Configuration Error", "Workflow type was not configured or specified correctly." );
                return false;
            }

            if ( !_workflowType.IsAuthorized( Authorization.VIEW, CurrentPerson ) )
            {
                ShowNotes( false );
                ShowMessage( NotificationBoxType.Warning, "Sorry", "You are not authorized to view this type of workflow." );
                return false;
            }

            if ( !(_workflowType.IsActive ?? true) )
            {
                ShowNotes( false );
                ShowMessage( NotificationBoxType.Warning, "Sorry", "This type of workflow is not active." );
                return false;
            }

            // If operating against an existing workflow, get the workflow and load attributes
            if ( !WorkflowId.HasValue )
            {
                WorkflowId = PageParameter( "WorkflowId" ).AsIntegerOrNull();
                if ( !WorkflowId.HasValue )
                {
                    Guid guid = PageParameter( "WorkflowGuid" ).AsGuid();
                    if ( !guid.IsEmpty() )
                    {
                        _workflow = _workflowService.Queryable()
                            .Where( w => w.Guid.Equals( guid ) && w.WorkflowTypeId == _workflowType.Id )
                            .FirstOrDefault();
                        if ( _workflow != null )
                        {
                            WorkflowId = _workflow.Id;
                        }
                    }
                }
            }

            if ( WorkflowId.HasValue )
            {
                if ( _workflow == null )
                {
                    _workflow = _workflowService.Queryable()
                        .Where( w => w.Id == WorkflowId.Value && w.WorkflowTypeId == _workflowType.Id )
                        .FirstOrDefault();
                }
                if ( _workflow != null )
                {
                    //hlblWorkflowId.Text = _workflow.WorkflowId;

                    _workflow.LoadAttributes();
                    foreach ( var activity in _workflow.Activities )
                    {
                        activity.LoadAttributes();
                    }
                }

            }

            // If an existing workflow was not specified, activate a new instance of workflow and start processing
            if ( _workflow == null )
            {
                string workflowName = PageParameter( "WorkflowName" );
                if ( string.IsNullOrWhiteSpace(workflowName))
                {
                    workflowName = "New " + _workflowType.WorkTerm;
                }

                _workflow = Rock.Model.Workflow.Activate( _workflowType, workflowName);
                if ( _workflow != null )
                {
                    // If a PersonId or GroupId parameter was included, load the corresponding
                    // object and pass that to the actions for processing
                    object entity = null;
                    int? personId = PageParameter( "PersonId" ).AsIntegerOrNull();
                    if ( personId.HasValue )
                    {
                        entity = new PersonService( _rockContext ).Get( personId.Value );
                    }
                    else
                    {
                        int? groupId = PageParameter( "GroupId" ).AsIntegerOrNull();
                        if ( groupId.HasValue )
                        {
                            entity = new GroupService( _rockContext ).Get( groupId.Value );
                        }
                    }

                    // Loop through all the query string parameters and try to set any workflow
                    // attributes that might have the same key
                    foreach ( var param in RockPage.PageParameters() )
                    {
                        if ( param.Value != null && param.Value.ToString().IsNotNullOrWhiteSpace() )
                        {
                            _workflow.SetAttributeValue( param.Key, param.Value.ToString() );
                        }
                    }

                    List<string> errorMessages;
                    if ( !_workflowService.Process( _workflow, entity, out errorMessages ) )
                    {
                        ShowNotes( false );
                        ShowMessage( NotificationBoxType.Danger, "Workflow Processing Error(s):",
                            "<ul><li>" + errorMessages.AsDelimited( "</li><li>" ) + "</li></ul>" );
                        return false;
                    }
                    if ( _workflow.Id != 0 )
                    {
                        WorkflowId = _workflow.Id;
                    }
                }
            }

            if ( _workflow == null )
            {
                ShowNotes( false );
                ShowMessage( NotificationBoxType.Danger, "Workflow Activation Error", "Workflow could not be activated." );
                return false;
            }

            var canEdit = UserCanEdit || _workflow.IsAuthorized( Authorization.EDIT, CurrentPerson );

            if ( _workflow.IsActive )
            {
                if ( ActionTypeId.HasValue )
                {
                    foreach ( var activity in _workflow.ActiveActivities )
                    {
                        _action = activity.ActiveActions.Where( a => a.ActionTypeId == ActionTypeId.Value ).FirstOrDefault();
                        if ( _action != null )
                        {
                            _activity = activity;
                            _activity.LoadAttributes();

                            _actionType = _action.ActionTypeCache;
                            ActionTypeId = _actionType.Id;
                            return true; 
                        }
                    }
                }

                // Find first active action form
                int personId = CurrentPerson != null ? CurrentPerson.Id : 0;
                int? actionId = PageParameter( "ActionId" ).AsIntegerOrNull();
                foreach ( var activity in _workflow.Activities
                    .Where( a =>
                        a.IsActive &&
                        ( !actionId.HasValue || a.Actions.Any( ac => ac.Id == actionId.Value ) ) &&
                        (
                            ( canEdit ) ||
                            ( !a.AssignedGroupId.HasValue && !a.AssignedPersonAliasId.HasValue ) ||
                            ( a.AssignedPersonAlias != null && a.AssignedPersonAlias.PersonId == personId ) ||
                            ( a.AssignedGroup != null && a.AssignedGroup.Members.Any( m => m.PersonId == personId ) )
                        )
                    )
                    .ToList()
                    .OrderBy( a => a.ActivityTypeCache.Order ) )
                {
                    if ( canEdit || ( activity.ActivityTypeCache.IsAuthorized( Authorization.VIEW, CurrentPerson ) ) )
                    {
                        foreach ( var action in activity.ActiveActions
                            .Where( a => ( !actionId.HasValue || a.Id == actionId.Value ) ) )
                        {
                            if ( action.ActionTypeCache.WorkflowForm != null && action.IsCriteriaValid )
                            {
                                _activity = activity;
                                _activity.LoadAttributes();

                                _action = action;
                                _actionType = _action.ActionTypeCache;
                                ActionTypeId = _actionType.Id;
                                return true;
                            }
                        }
                    }
                }

                lSummary.Text = string.Empty;

            }
            else
            {
                if ( GetAttributeValue( AttributeKey.ShowSummaryView ).AsBoolean() && !string.IsNullOrWhiteSpace( _workflowType.SummaryViewText ) )
                {
                    var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson );
                    mergeFields.Add( "Action", _action );
                    mergeFields.Add( "Activity", _activity );
                    mergeFields.Add( "Workflow", _workflow );

                    lSummary.Text = _workflowType.SummaryViewText.ResolveMergeFields( mergeFields, CurrentPerson );
                    lSummary.Visible = true;
                }
            }

            if ( lSummary.Text.IsNullOrWhiteSpace() )
            {
                if ( _workflowType.NoActionMessage.IsNullOrWhiteSpace() )
                {
                    ShowMessage( NotificationBoxType.Warning, string.Empty, "The selected workflow is not in a state that requires you to enter information." );
                }
                else
                {
                    var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson );
                    mergeFields.Add( "Action", _action );
                    mergeFields.Add( "Activity", _activity );
                    mergeFields.Add( "Workflow", _workflow );
                    ShowMessage( NotificationBoxType.Warning, string.Empty, _workflowType.NoActionMessage.ResolveMergeFields( mergeFields, CurrentPerson ) );
                }
            }

            ShowNotes( false );

            return false;
        }

        private void LoadWorkflowType()
        {
            if ( _rockContext == null )
            {
                _rockContext = new RockContext();
            }

            if ( _workflowService == null )
            {
                _workflowService = new WorkflowService( _rockContext );
            }

            // Get the workflow type id (initial page request)
            if ( !WorkflowTypeId.HasValue )
            {
                // Get workflow type set by attribute value
                Guid workflowTypeguid = GetAttributeValue( AttributeKey.WorkflowType ).AsGuid();
                if ( !workflowTypeguid.IsEmpty() )
                {
                    _workflowType = WorkflowTypeCache.Get( workflowTypeguid );
                }

                // If an attribute value was not provided, check for query/route value
                if ( _workflowType != null )
                {
                    WorkflowTypeId = _workflowType.Id;
                    ConfiguredType = true;
                }
                else
                {
                    WorkflowTypeId = PageParameter( "WorkflowTypeId" ).AsIntegerOrNull();
                    ConfiguredType = false;
                }
            }

            // Get the workflow type 
            if ( _workflowType == null && WorkflowTypeId.HasValue )
            {
                _workflowType = WorkflowTypeCache.Get( WorkflowTypeId.Value );
            }
        }

        private void ProcessActionRequest()
        {
            string action = PageParameter( "Command" );
            if ( !string.IsNullOrWhiteSpace( action ) )
            {
                CompleteFormAction( action );
            }
        }

        private void BuildForm( bool setValues )
        {
            var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson );
            mergeFields.Add("Action", _action);
            mergeFields.Add("Activity", _activity);
            mergeFields.Add("Workflow", _workflow);

            mergeFields.Add( "FormState", _formState );
            foreach ( var field in _formState.Fields )
            {
                mergeFields.Add( field.FieldName, field );
            }

            //var form = _actionType.WorkflowForm;

            //if (setValues)
            //{
            //    lheadingText.Text = form.Header.ResolveMergeFields(mergeFields);
            //    lFootingText.Text = form.Footer.ResolveMergeFields(mergeFields);
            //}

            //if (_workflow != null && _workflow.CreatedDateTime.HasValue)
            //{
            //    hlblDateAdded.Text = String.Format("Added: {0}", _workflow.CreatedDateTime.Value.ToShortDateString());
            //}
            //else
            //{
            //    hlblDateAdded.Visible = false;
            //}

            phAttributes.Controls.Clear();
             _formControls.Clear();
 

            // Begin foreach to populate fields on the form
            foreach (Field field in _formState.Fields.OrderBy( f => f.Order ) )
            {
                var fieldIsVisible = (field.RevealCondition.ToString().ResolveMergeFields(mergeFields) == "true");
                switch ( field.FieldType.ToLower() )
                {
                    case "literal":
                        var fieldLiteral = new LiteralControl
                        {
                            ID = field.FieldName,
                            Text = field.Prompt.ResolveMergeFields(mergeFields),
                            Visible = fieldIsVisible
                        };
                        phAttributes.Controls.Add(fieldLiteral);

                        _formControls.Add(fieldLiteral);
                        break;

                    case "text":
                        var fieldText = new RockTextBox
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup, 
                            Text = field.ResponseValue,
                            TextMode = field.FieldConfiguration.Rows > 1  ? TextBoxMode.MultiLine : TextBoxMode.SingleLine,
                            Rows = field.FieldConfiguration.Rows,
                            AutoPostBack = field.PostbackOnChange,
                            Visible = fieldIsVisible
                        };
                        phAttributes.Controls.Add(fieldText);

                        _formControls.Add(fieldText);
                        break;

                    case "email":
                        var fieldEmail = new EmailBox
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup, 
                            Text = field.ResponseValue,
                            AutoPostBack = field.PostbackOnChange,
                            Visible = fieldIsVisible
                        };
                        phAttributes.Controls.Add(fieldEmail);

                        _formControls.Add(fieldEmail);
                        break;

                    case "phonenumber":
                        var fieldPhoneNumber = new PhoneNumberBox
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup, 
                            Text = field.ResponseValue,
                            CountryCode = "1",
                            AutoPostBack = field.PostbackOnChange,
                            Visible = fieldIsVisible
                        };
                        phAttributes.Controls.Add(fieldPhoneNumber);

                        _formControls.Add(fieldPhoneNumber);
                        break;

                    case "ssn":
                        var fieldSsn = new SSNBox
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup, 
                            TextEncrypted = field.ResponseValue,
                            //AutoPostBack = field.PostbackOnChange,
                            Visible = fieldIsVisible
                        };
                        phAttributes.Controls.Add(fieldSsn);

                        _formControls.Add(fieldSsn);
                        break;

                    case "date":
                        var fieldDate = new DatePicker
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup,
                            Text = field.ResponseValue,
                            Rows = field.FieldConfiguration.Rows,
                            AutoPostBack = field.PostbackOnChange,
                            Visible = fieldIsVisible
                        };
                        phAttributes.Controls.Add(fieldDate);

                        _formControls.Add(fieldDate);
                        break;

                    case "address":
                        var addressValues = field.ResponseValue.FromJsonOrNull<AddressValue>();
                        var fieldAddress = new AddressControl
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup, 
                            Visible = fieldIsVisible,

                            Street1 = addressValues != null ? addressValues.Street1 : "",
                            Street2 = addressValues != null ? addressValues.Street2 : "",
                            City = addressValues != null ? addressValues.City : "",
                            State = addressValues != null ? addressValues.State : "",
                            Country = addressValues != null ? addressValues.Country : "US",
                            PostalCode = addressValues != null ? addressValues.PostalCode : ""
                        };
                        phAttributes.Controls.Add(fieldAddress);

                        _formControls.Add(fieldAddress);
                        break;

                    case "radio":
                        var fieldRadio = new RockRadioButtonList
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup,
                            RepeatColumns = field.FieldConfiguration.Rows,
                            AutoPostBack = field.PostbackOnChange,
                            Visible = (field.RevealCondition.ToString().ResolveMergeFields(mergeFields) == "true")
                        };
                        fieldRadio.Items.Clear();
                        phAttributes.Controls.Add(fieldRadio);

                        foreach ( var option in field.FieldConfiguration.Options)
                        {
                            fieldRadio.Items.Add( option );
                        }
                        fieldRadio.SetValue( field.ResponseValue );

                        _formControls.Add(fieldRadio);
                        break;

                    case "dropdown":
                        var fieldDropDown = new RockDropDownList
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup,
                            AutoPostBack = field.PostbackOnChange,
                            Visible = (field.RevealCondition.ToString().ResolveMergeFields(mergeFields) == "true")
                        };
                        fieldDropDown.Items.Clear();
                        phAttributes.Controls.Add(fieldDropDown);

                        fieldDropDown.Items.Add( "" );
                        foreach ( var option in field.FieldConfiguration.Options)
                        {
                            fieldDropDown.Items.Add( option );
                        }
                        fieldDropDown.SetValue( field.ResponseValue );

                        _formControls.Add(fieldDropDown);
                        break;

                    case "campus":
                        int selectedCampusId = 0;
                        int.TryParse(field.ResponseValue, out selectedCampusId);

                        var fieldCampus = new CampusPicker
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup,
                            AutoPostBack = field.PostbackOnChange,
                            IncludeInactive = false,
                            Visible = (field.RevealCondition.ToString().ResolveMergeFields(mergeFields) == "true"),
                            SelectedCampusId = selectedCampusId, 
                        };
                        //fieldCampus.Items.Clear();
                        phAttributes.Controls.Add(fieldCampus);

                        _formControls.Add(fieldCampus);
                        break;

                    case "multibox":
                        var fieldMultiBox = new RockCheckBoxList
                        {
                            ID = field.FieldName,
                            Label = field.Prompt.ResolveMergeFields(mergeFields),
                            Help = field.HelpText,
                            Required = field.Required && fieldIsVisible,
                            RequiredErrorMessage = field.RequiredErrorText,
                            ValidationGroup = BlockValidationGroup,
                            AutoPostBack = field.PostbackOnChange,
                            Visible = (field.RevealCondition.ToString().ResolveMergeFields(mergeFields) == "true")
                        };
                        fieldMultiBox.Items.Clear();
                        fieldMultiBox.SelectedValues.Clear();
                        phAttributes.Controls.Add(fieldMultiBox);

                        foreach ( var option in field.FieldConfiguration.Options)
                        {
                            fieldMultiBox.Items.Add( option );
                        }

                        var selectedValues = field.ResponseValue.FromJsonOrNull<List<string>>();
                        if ( selectedValues != null && selectedValues.Count > 0)
                        {
                            foreach ( var item in fieldMultiBox.Items )
                            {
                                fieldMultiBox.Items.FindByText( item.ToString() ).Selected = ( selectedValues.Contains( item.ToString() ) );
                            }
                        }

                        _formControls.Add(fieldMultiBox);
                        break;
                }
            }

            ShowNotes(false);

            phActions.Controls.Clear();

            var visibleActions = _formState.Actions.Where(a => a.RevealCondition.ResolveMergeFields(mergeFields).ToLower() == "true").OrderBy(a => a.Order);
            foreach (var action in visibleActions )
            {
                string buttonHtml = string.Empty;
                var buttonDefinedValue = DefinedValueCache.Get( action.HtmlButtonValueId );
                if (buttonDefinedValue != null)
                {
                    buttonHtml = buttonDefinedValue.GetAttributeValue("ButtonHTML");
                }

                if (string.IsNullOrWhiteSpace(buttonHtml))
                {
                    buttonHtml = "<a href=\"{{ ButtonLink }}\" onclick=\"{{ ButtonClick }}\" class='btn btn-primary' data-loading-text='<i class=\"fa fa-refresh fa-spin\"></i> {{ ButtonText }}'>{{ ButtonText }}</a>";
                }

                var buttonMergeFields = new Dictionary<string, object>();
                buttonMergeFields.Add("ButtonText", action.ButtonText.EncodeHtml());
                buttonMergeFields.Add("ButtonClick",
                        string.Format("if ( Page_ClientValidate('{0}') ) {{ $(this).button('loading'); return true; }} else {{ return false; }}",
                        BlockValidationGroup));
                buttonMergeFields.Add("ButtonLink", Page.ClientScript.GetPostBackClientHyperlink(this, action.ActionName));

                buttonHtml = buttonHtml.ResolveMergeFields(buttonMergeFields);
                phActions.Controls.Add(new LiteralControl(buttonHtml));
                phActions.Controls.Add(new LiteralControl(" "));
            }

            //TODO: this is out of date, update to match the logic in the workflow start stuff.  Better yet, move into a method.
            // Set the Attribute Values in the Workflow to meet the current value of the _formState
            //if (_workflow != null)
            //{
            //    foreach (var field in _formState.Fields)
            //    {
            //        _workflow.SetAttributeValue(field.AttributeKey, field.ResponseValue);
            //    }
            //}
        }

        private void ShowNotes(bool visible)
        {
            divNotes.Visible = visible;

            if ( visible )
            {
                divForm.RemoveCssClass( "col-md-12" );
                divForm.AddCssClass( "col-md-6" );
            }
            else
            {
                divForm.AddCssClass( "col-md-12" );
                divForm.RemoveCssClass( "col-md-6" );
            }
        }

        private void GetFormValues()
        {
            if ( _workflow != null && _actionType != null )
            {
                var form = _actionType.WorkflowForm;

                var values = new Dictionary<int, string>();
                foreach ( var formAttribute in form.FormAttributes.OrderBy( a => a.Order ) )
                {
                    if ( formAttribute.IsVisible && !formAttribute.IsReadOnly )
                    {
                        var attribute = AttributeCache.Get( formAttribute.AttributeId );
                        var control = phAttributes.FindControl( string.Format( "attribute_field_{0}", formAttribute.AttributeId ) );

                        if ( attribute != null && control != null)
                        {
                            Rock.Attribute.IHasAttributes item = null;
                            if ( attribute.EntityTypeId == _workflow.TypeId )
                            {
                                item = _workflow;
                            }
                            else if ( attribute.EntityTypeId == _activity.TypeId )
                            {
                                item = _activity;
                            }

                            if ( item != null )
                            {
                                item.SetAttributeValue( attribute.Key, attribute.FieldType.Field.GetEditValue( attribute.GetControl( control ), attribute.QualifierValues ) );
                            }
                        }
                    }
                }
            }
        }

        private void CompleteFormAction( string formAction )
        {
            Action action = null;
            if ( !string.IsNullOrWhiteSpace( formAction ) )
            {
                action = _formState.Actions.Where( a => a.ActionName == formAction ).FirstOrDefault();
                if ( action.StartWorkflow && _workflow == null )
                {
                    // If you don't yet have the Workflow, and the Action should start it, start it.
                    _workflow = Rock.Model.Workflow.Activate( _workflowType, _workflowType.Name );
                }

                if ( _workflow != null )
                {
                    foreach (var field in _formState.Fields)
                    {
                        if ( field.FieldType.ToLower() == "address" )
                        {
                            var addressValues = field.ResponseValue.FromJsonOrNull<AddressValue>();
                            var rockContext = new RockContext();
                            var locationService = new LocationService( rockContext );

                            // Find if there's a matching Location record to the one being entered.
                            var location = locationService.Queryable().Where( l =>
                                l.Street1 == addressValues.Street1
                                && l.Street2 == addressValues.Street2
                                && l.City == addressValues.City
                                && l.State == addressValues.State
                                && l.PostalCode == addressValues.PostalCode
                                && l.Country == addressValues.Country
                            ).FirstOrDefault();

                            if ( location != null )
                            {
                                // If the location value was found, then use that for the Workflow.
                                _workflow.SetAttributeValue( field.AttributeKey, location.Guid );
                            }
                            else
                            {
                                // If the Location was NOT found, then create a Location record, and use that for the Workflow.
                                var newLocationGuid = Guid.NewGuid();
                                var newLocation = new Location
                                {
                                    Street1 = addressValues.Street1,
                                    Street2 = addressValues.Street2,
                                    City = addressValues.City,
                                    State = addressValues.State,
                                    PostalCode = addressValues.PostalCode,
                                    Country = addressValues.Country,
                                    Guid = newLocationGuid
                                };

                                locationService.Add( newLocation );
                                rockContext.SaveChanges();

                                _workflow.SetAttributeValue( field.AttributeKey, newLocationGuid);
                            }
                        }
                        else if ( field.FieldType.ToLower() == "multibox" )
                        {
                            var multiSelectString = string.Empty;
                            field.ResponseValue.FromJsonOrNull<List<string>>().ForEach( rv => multiSelectString = rv + "," + multiSelectString );
                            multiSelectString = multiSelectString.ReplaceLastOccurrence( ",", "" );

                            _workflow.SetAttributeValue( field.AttributeKey, multiSelectString );
                        }
                        else if (field.FieldType.ToLower() == "campus")
                        {
                            var campusId = 0;
                            int.TryParse( field.ResponseValue, out campusId );
                            if ( campusId > 0 )
                            {
                                var campusGuid = CampusCache.Get( campusId ).Guid.ToStringSafe();
                                _workflow.SetAttributeValue( field.AttributeKey, campusGuid ?? "" );
                            }
                        }
                        else
                        {
                            _workflow.SetAttributeValue( field.AttributeKey, field.ResponseValue );
                        }

                        _workflow.SetAttributeValue("FormState", _formState.ToJson() );
                    }
                    List<string> errorMessages;
                    _workflowService.Process(_workflow, out errorMessages);
                }

                _formState.CurrentPage = action.DestinationPage;

                var successPageGuid = GetAttributeValue( "SuccessPage" ).AsGuid();
                if ( action.GoToSuccessPage && !successPageGuid.IsEmpty() )
                {
                    NavigateToPage( successPageGuid, new Dictionary<string, string>() );
                }
            }

            if ( !string.IsNullOrWhiteSpace( formAction ) && _workflow != null )
            {
                 var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson );
                mergeFields.Add( "Workflow", _workflow );
                
                Guid activityTypeGuid = Guid.Empty;
                //string responseText = "Your information has been submitted successfully.";

            }
            BuildForm(false);
        }

        private void ShowMessage( NotificationBoxType type, string title, string message, bool hideForm = true )
        {
            nbMessage.NotificationBoxType = type;
            nbMessage.Title = title;
            nbMessage.Text = message;
            nbMessage.Visible = true;
            nbMessage.Dismissable = false;

            if ( hideForm )
            {
                pnlForm.Visible = false;
            }

        }

        /// <summary>
        /// Set the properties of the block title bar.
        /// </summary>
        private void SetBlockTitle()
        {
            // If the block title is specified by a configuration setting, use it.
            var blockTitle = GetAttributeValue( AttributeKey.BlockTitleTemplate );

            if ( !string.IsNullOrWhiteSpace( blockTitle ) )
            {
                // Resolve the block title using the specified Lava template.
                var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( RockPage, CurrentPerson );

                mergeFields.Add( "WorkflowType", _workflowType );

                // Add the WorkflowType as the default Item.
                mergeFields.Add( "Item", _workflowType );

                blockTitle = blockTitle.ResolveMergeFields( mergeFields );
            }

            // If the block title is not configured, use the Workflow Type if it is available.
            if ( string.IsNullOrWhiteSpace( blockTitle ) )
            {
                if ( _workflowType != null )
                {
                    blockTitle = string.Format( "{0} Entry", _workflowType.WorkTerm );
                }
                else
                {
                    blockTitle = "Workflow Entry";
                }
            }

            //lTitle.Text = blockTitle;

            // Set the Page Title to the Workflow Type name, unless the Workflow Type has been specified by a configuration setting.
            if ( _workflowType != null && !ConfiguredType )
            {
                RockPage.PageTitle = _workflowType.Name;
            }

            // Set the Block Icon.
            var blockTitleIconCssClass = GetAttributeValue( AttributeKey.BlockTitleIconCssClass );

            if ( string.IsNullOrWhiteSpace( blockTitleIconCssClass ) )
            {
                if ( _workflowType != null )
                {
                    blockTitleIconCssClass = _workflowType.IconCssClass;
                }
            }

            if ( !string.IsNullOrWhiteSpace( blockTitleIconCssClass ) )
            {
                //lIconHtml.Text = string.Format( "<i class='{0}' ></i>", blockTitleIconCssClass );

                // If the Page Icon is not configured, set it to the same icon as the block.
                if ( string.IsNullOrWhiteSpace( RockPage.PageIcon ) )
                {
                    RockPage.PageIcon = blockTitleIconCssClass;
                }
            }
        }

        #endregion

    }

}
