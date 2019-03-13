using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.UI;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;

namespace Oxbow.Gwe.Core.WorkflowTransitionActions
{
    [DataContract]
    public class PermissionSetWorkflowTransitionAction : WorkflowTransitionAction
    {
        [DataMember]
        public string PermissionSet { get; set; }

        public override UserControl GenerateUserControl(Page page)
        {
            var result = (IWorkflowTransitionActionConfigControl<PermissionSetWorkflowTransitionAction>) page.LoadControl("~/_controltemplates/Oxbow.Gwe/PermissionSetWorkflowTransitionActionConfig.ascx");
            result.UpdateUi(this);
            return (UserControl) result;
        }

        public override void UpdateFromUserControl(UserControl userControl) { ((IWorkflowTransitionActionConfigControl<PermissionSetWorkflowTransitionAction>) userControl).UpdateDataModel(this); }

        public override string GetTypeName() { return "Permission Set"; }

        public override void Execute(SPListItem spListItem)
        {
            if (string.IsNullOrEmpty(PermissionSet))
                return;
            try
            {
                var container = ResolveType.Instance.OfSpListItemContainer(spListItem, WorkflowConfiguration);
                string permissionSetString = ResolveType.Instance.Of<ITemplateEngine>().Render(PermissionSet, container);
                Dictionary<SPPrincipal, SPRoleType> permissionsets = ParsePermissionSet(spListItem.Web, permissionSetString);
                if (permissionsets.Count > 0)
                    CommonCode.SetPermissionsOnListItem(spListItem, permissionsets);
            }
            catch (Exception exp)
            {
                throw new Exception("The PermissionSetWorkflowTransitionHandler ran into the following issue: " + exp);
            }
        }

        public Dictionary<SPPrincipal, SPRoleType> ParsePermissionSet(SPWeb spWeb, string permissionSet)
        {
            var result = new Dictionary<SPPrincipal, SPRoleType>();
            IEnumerable<string> groupings = (permissionSet ?? string.Empty).Split(new[] {','}).Where(x => !string.IsNullOrEmpty(x));
            foreach (string grouping in groupings)
            {
                if (!grouping.Contains(":"))
                    continue;
                try
                {
                    SPPrincipal principal = ResolvePrincipal(spWeb, grouping.Split(new[] {':'})[0]);
                    var roleType = (SPRoleType) Enum.Parse(typeof (SPRoleType), grouping.Split(new[] {':'})[1]);
                    result.Add(principal, roleType);
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("Unable to parse [{0}] into a valid permission set: {1}", grouping, exp));
                }
            }
            return result;
        }

        public SPPrincipal ResolvePrincipal(SPWeb spWeb, string name)
        {
            //attempt to reference a group
            try
            {
                return ResolveType.Instance.Of<IUserIdentificationService>().GetSPGroupByName(name, spWeb);
            }
            catch
            {
            }
            try
            {
                return ResolveType.Instance.Of<IUserIdentificationService>().GetSPUserByUserName(name, spWeb);
            }
            catch
            {
                throw new Exception(string.Format("Unable to resolve the principal name [{0}] into a sharepoint group or user.", name));
            }
        }
    }
}

