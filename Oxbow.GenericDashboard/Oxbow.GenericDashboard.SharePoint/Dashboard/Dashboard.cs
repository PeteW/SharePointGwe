using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Oxbow.GenericDashboard.SharePoint.CONTROLTEMPLATES;

namespace Oxbow.GenericDashboard.SharePoint.Dashboard
{
    public class VisualEditorPart : EditorPart
    {
        private const string _configWebPartPath = @"~/_CONTROLTEMPLATES/WebPartConfig.ascx";
        private WebPartConfig _control;
        protected override void CreateChildControls()
        {
            _control = (WebPartConfig)Page.LoadControl(_configWebPartPath);
            Controls.Add(_control);
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();
            ((Dashboard)WebPartToEdit).ConfigXml = _control.GetConfigXml();
            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            _control.SetConfigXml(((Dashboard)WebPartToEdit).ConfigXml);
        }
    }

    [ToolboxItemAttribute(false)]
    public class Dashboard : WebPart, IWebEditable
    {
        private bool _error = false;
        private string _myProperty = null;
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Oxbow.GenericDashboard.SharePoint/Dashboard/DashboardUserControl.ascx";

        object IWebEditable.WebBrowsableObject
        {
            get { return this; }
        }

        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            var collection = new List<EditorPart>();
            var editor = new VisualEditorPart();
            editor.ID = ID + "_editor";
            collection.Add(editor);
            return new EditorPartCollection(collection);
        }

        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(true)]
        [WebDisplayName("ConfigXml")]
        [WebDescription("ConfigXml")]
        public string ConfigXml { get; set; }

        public Dashboard()
        {
            this.ExportMode = WebPartExportMode.All;
        }

        /// <summary>
        /// Create all your controls here for rendering.
        /// Try to avoid using the RenderWebPart() method.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (!_error)
            {
                try
                {
                    base.CreateChildControls();
                    var control = (DashboardUserControl)Page.LoadControl(_ascxPath);
                    control.SetConfigXml(ConfigXml);
                    this.Controls.Add(control);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        /// <summary>
        /// Ensures that the CreateChildControls() is called before events.
        /// Use CreateChildControls() to create your controls.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (!_error)
            {
                try
                {
                    base.OnLoad(e);
                    this.EnsureChildControls();

                    // Your code here...
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        /// <summary>
        /// Clear all child controls and add an error message for display.
        /// </summary>
        /// <param name="ex"></param>
        private void HandleException(Exception ex)
        {
            this._error = true;
            this.Controls.Clear();
            this.Controls.Add(new LiteralControl(ex.ToString()));
        }
    }
}
