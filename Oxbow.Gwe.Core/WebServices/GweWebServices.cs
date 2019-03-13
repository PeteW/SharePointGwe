using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Services;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Utils;
using Oxbow.Gwe.Core.WebServiceModels;

namespace Oxbow.Gwe.Core.WebServices
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class GweWebServices : WebService
    {
        [WebMethod]
        public FormHistoryItem[] GetFormHistory(string historyListName, string formTitle)
        {
            try
            {
                return FormHistoryItem.GetFormHistory(SPContext.Current.Web, historyListName, formTitle);
            }
            catch (Exception exp)
            {
                LogError("GetFormHistory", exp);
                return new FormHistoryItem[0];
            }
        }
        [WebMethod]
        public string GetManager(string domainName, string userName)
        {
            try
            {
                var spUser = ResolveType.Instance.Of<IUserIdentificationService>().GetManager(userName, SPContext.Current.Web);
                if (spUser == null)
                    return string.Empty;
                return spUser.LoginName;
            }
            catch(Exception exp)
            {
                LogError("GetManager",exp);
                return string.Empty;
            }
        }
        [WebMethod]
        public GetUserFullIdentityResponse GetUserFullIdentity(GetUserFullIdentityRequest request)
        {
            try
            {
                var spUser = ResolveType.Instance.Of<IUserIdentificationService>().GetSPUserByUserName(request.UserName, SPContext.Current.Web);
                var result = new GetUserFullIdentityResponse(){ExecutionOutcome = ExecutionOutcome.Success()};
                result.Email = spUser.Email;
                result.FullName = spUser.Name;
                result.Id = spUser.ID;
                result.UserName = spUser.LoginName;
                return result;
            }
            catch(Exception exp)
            {
                LogError("GetUserFullIdentity",exp);
                return new GetUserFullIdentityResponse(){ExecutionOutcome = ExecutionOutcome.Fail(exp.ToString())};
            }
        }

        [WebMethod]
        public string GetUserEmail(string domainName, string userName)
        {
            try
            {
                var spUser = ResolveType.Instance.Of<IUserIdentificationService>().GetSPUserByUserName(userName, SPContext.Current.Web);
                return spUser.Email;
            }
            catch (Exception exp)
            {
                LogError("GetUserEmail", exp);
                return string.Empty;
            }
        }
        [WebMethod]
        public bool IsUserMemberOfSharePointGroup(string domainName, string userName, string sharePointGroupName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    return false;
                //the code below is causing issues in SOME SharePoint environments
                //the issue seems to be that iterating through spUser.Groups throws an error saying the web has already been disposed.
//                var spUser = ResolveType.Instance.Of<IUserIdentificationService>().GetSPUserByUserName(userName, SPContext.Current.Web);
//                foreach (SPGroup spGroup in spUser.Groups)
//                {
//                    if (spGroup.Name.ToLower() == sharePointGroupName.ToLower())
//                        return true;
//                }
//                return false;
                return ResolveType.Instance.Of<IUserIdentificationService>().IsUserMemberOfSpGroup(userName, sharePointGroupName, SPContext.Current.Web);
            }
            catch (Exception exp)
            {
                LogError("IsUserMemberOfSharePointGroup", exp);
                return false;
            }
        }
        [WebMethod(MessageName = "Response1")]
        public GetSharePointUsersByGroupResponse GetSharePointUsersByGroup(string groupName)
        {
            try
            {
                var result = new GetSharePointUsersByGroupResponse();
                result.Items = new List<GetSharePointUsersByGroupResponseItem>();
                var group = ResolveType.Instance.Of<IUserIdentificationService>().GetSPGroupByName(groupName, SPContext.Current.Web);
                foreach (SPUser u in group.Users)
                {
                    result.Items.Add(new GetSharePointUsersByGroupResponseItem() { Email = u.Email, FullName = u.Name, UserName = u.LoginName });
                }
                return result;
            }
            catch (Exception exp)
            {
                LogError("GetSharePointUsersByGroup", exp);
                return new GetSharePointUsersByGroupResponse() { Items = new List<GetSharePointUsersByGroupResponseItem>() };
            }
        }
        
        [WebMethod]
        public DeleteNodeResponse DeleteNode(DeleteNodeRequest request)
        {
            try
            {
                var list = SPContext.Current.Web.GetSpList(request.ListName);
                //try to find the item.
                var items = list.GetItems(new SPQuery { Query = string.Format("<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}.xml</Value></Eq></Where>", request.FileName), RowLimit = 1 });
                //we need the item, if we dont have it then that is an exception
                if (items.Count == 0)
                    throw new Exception(string.Format("Unable to find a file named: [{0}]", request.FileName));
                if (items.Count > 1)
                    throw new Exception(string.Format("Found multiple files named: [{0}]", request.FileName));
                var item = items[0];
                var formContainer = ResolveType.Instance.OfSpListItemContainer(item, null) as IFormContainer;
                if (formContainer == null)
                    throw new Exception(item.GetDescription() + " : Expected to have a type IFormContainer.");
                formContainer.DeleteNode(request.NodeXpath);
                formContainer.PerformSystemSave();
                return new DeleteNodeResponse() { ExecutionOutcome = ExecutionOutcome.Success() };
            }
            catch (Exception exp)
            {
                LogError("DeleteNode", exp);
                return new DeleteNodeResponse() { ExecutionOutcome = ExecutionOutcome.Fail(exp.Message)};
            }
        }

        [WebMethod]
        public AppendXmlResponse AppendXml(AppendXmlRequest request)
        {
            try
            {
                Thread.Sleep(2000);

                var list = SPContext.Current.Web.Lists[request.ListName];
                var items = list.GetItems(new SPQuery {Query = string.Format("<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}.xml</Value></Eq></Where>", request.FileName), RowLimit = 1});
                //we need the item, if we dont have it then that is an exception
                if (items.Count == 0)
                    throw new Exception(string.Format("Unable to find a file named: [{0}]", request.FileName));
                if (items.Count > 1)
                    throw new Exception(string.Format("Found multiple files named: [{0}]", request.FileName));
                var item = items[0];
                var modifiedByUser = item.GetSpUserStrict("Modified By");
                

                var formContainer = ResolveType.Instance.OfSpListItemContainer(item, null) as IFormContainer;
                if (formContainer == null)
                    throw new Exception(item.GetDescription() + " : Expected to have a type IFormContainer.");
                formContainer.AppendChildXml(request.ParentNodeXpath, request.Prepend, request.Xml);
                formContainer.PerformSystemSave();
                //                                                                          });
                return new AppendXmlResponse() {ExecutionOutcome = ExecutionOutcome.Success()};
            }
            catch (Exception exp)
            {
                LogError("AppendXml", exp);
                return new AppendXmlResponse() {ExecutionOutcome = ExecutionOutcome.Fail(exp.Message)};
            }
        }

        private void LogError(string methodName, Exception exp, string appId)
        {
            LogError(string.Format("{0} - ({1})", methodName, appId), exp);
        }
        private void LogError(string methodName, Exception exp)
        {
            var id = UserIdentity.GetCurrentUserName();
            ResolveType.Instance.Of<ILogger>().Error(string.Format("Exception caught inside of {0} method of web service. The calling user was: [{1}]: {2}", methodName, id, exp));
        }
    }
}