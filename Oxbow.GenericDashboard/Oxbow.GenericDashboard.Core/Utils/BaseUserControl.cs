using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Oxbow.GenericDashboard.Core.Utils
{
    public class BaseUserControl: UserControl
    {
        /// <summary>
        /// Gets the name of the logged in user.
        /// </summary>
        /// <value>The name of the logged in user.</value>
        protected string LoggedInUserName
        {
            get { return HttpContext.Current.User.Identity.Name; }
        }

        /// <summary>
        /// Gets or sets the error messages to show.
        /// </summary>
        /// <value>The error messages to show.</value>
        public string ErrorMessagesToShow { get; set; }

        /// <summary>
        /// Gets the image folder URL.
        /// </summary>
        /// <value>The image folder URL.</value>
        protected string ImageFolderUrl
        {
            get { return "/_layouts/images/"; }
        }

        protected Panel PnlErrors
        {
            get { return (Panel)FindControl("pnlErrors"); }
        }

        protected Literal LtrErrorMessage
        {
            get { return (Literal)FindControl("ltrErrorMessage"); }
        }

        protected Panel PnlHighlights
        {
            get { return (Panel)FindControl("pnlHighlights"); }
        }

        protected Literal LtrHighlightMessage
        {
            get { return (Literal)FindControl("ltrHighlightMessage"); }
        }


        /// <summary>
        /// Inits the controls.
        /// </summary>
        protected virtual void InitControls()
        {
        }

        /// <summary>
        /// Binds the controls.
        /// </summary>
        protected virtual void BindControls()
        {
        }

        /// <summary>
        /// Pre render controls.
        /// </summary>
        protected virtual void PreRenderControls()
        {
        }

        /// <summary>
        /// Wires the events.
        /// </summary>
        protected virtual void WireEvents()
        {
        }
        protected virtual void CustomOnLoad()
        {
        }


        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            WireEvents();
            InitControls();
            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            RegisterCSS("/_layouts/Oxbow.GenericDashboard.SharePoint/jquery-ui-1.8.20.custom.css");
            RegisterScript("/_layouts/Oxbow.GenericDashboard.SharePoint/jquery-1.7.2.min.js");
            RegisterScript("/_layouts/Oxbow.GenericDashboard.SharePoint/jquery_cookie.js");
            RegisterScript("/_layouts/Oxbow.GenericDashboard.SharePoint/jquery-ui-1.8.20.custom.min.js");
            RegisterScript("/_layouts/Oxbow.GenericDashboard.SharePoint/jquery.dataTables.min.js");
            if (!IsPostBack)
            {
                BindControls();
            }
            CustomOnLoad();
            base.OnLoad(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            PreRenderControls();
            base.OnPreRender(e);
        }

        protected void RegisterCSS(string url)
        {
            var link = new HtmlLink();
            link.Href = url.Replace("//", "/");
            link.Attributes["rel"] = "stylesheet";
            link.Attributes["type"] = "text/css";
            link.Attributes["media"] = "all";
            if (!Page.Header.Controls.Contains(link))
            Page.Header.Controls.Add(link);
        }

        private void RegisterScript(string url)
        {
            var myJs = new HtmlGenericControl();
            string href = (url).Replace("//", "/");
            myJs.TagName = "script";
            myJs.Attributes.Add("type", "text/javascript");
            myJs.Attributes.Add("language", "javascript"); //don't need it usually but for cross browser.
            myJs.Attributes.Add("src", href);
            if (!Page.Header.Controls.Contains(myJs))
                Page.Header.Controls.Add(myJs);
        }

        /// <summary>
        /// Gets from session.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionVariableKey">The session variable key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        protected T GetFromSession<T>(string sessionVariableKey, T defaultValue)
        {
            if (Session[sessionVariableKey] == null)
                return defaultValue;
            return (T)Session[sessionVariableKey];
        }

        /// <summary>
        /// Clears the error message.
        /// </summary>
        protected void ClearMessages()
        {
            PnlErrors.Visible = false;
            LtrErrorMessage.Text = string.Empty;
            PnlHighlights.Visible = false;
            LtrHighlightMessage.Text = string.Empty;
        }

        /// <summary>
        /// Displays the error message.
        /// </summary>
        /// <param name="e">The e.</param>
        protected void DisplayErrorMessage(Exception e)
        {
            DisplayErrorMessage(e.ToString());
        }

        /// <summary>
        /// Displays the error message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void DisplayErrorMessage(string message)
        {
            PnlErrors.Visible = true;
            LtrErrorMessage.Text = message;
        }

        /// <summary>
        /// Displays the feedback.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void DisplayFeedback(string message)
        {
            PnlHighlights.Visible = true;
            LtrHighlightMessage.Text = message;
        }
    }
}