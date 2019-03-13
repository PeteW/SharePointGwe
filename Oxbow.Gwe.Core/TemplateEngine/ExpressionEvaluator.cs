using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.SharePoint;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;
using Oxbow.Gwe.Lang.Ast;

namespace Oxbow.Gwe.Core.TemplateEngine
{

    public class ExpressionEvaluator 
    {
        public static List<string> GetOperandsFromCodeBlock(string codeBlock, ISpListItemContainer container)
        {
            var result = new List<string>();
            var eq = ParseEquation(codeBlock);
            if(eq.Item.IsText)
                throw new Exception(string.Format("Unable to get operands from non-function call [{0}]", codeBlock));
            var func = eq.Item as Expr.Func;
            for (var i = 0; i < func.Item2.Length; i++)
            {
                result.Add(EvaluateEquation(func.Item2[i], container));
            }
            return result;
        }

        public static Equation ParseEquation(string codeBlock)
        {
            return GweParser.eval(codeBlock);
        }

        public static string EvaluateEquation(Equation equation, ISpListItemContainer container)
        {
            return EvaluateEquation(equation.Item, container);
        }

        private static string EvaluateEquation(Expr expression, ISpListItemContainer container)
        {
            if (expression.IsFunc)
                return EvaluateFunctionCall((Expr.Func)expression, container);
            else if (expression.IsText)
                return ((Expr.Text) expression).Item;
            else
                throw new Exception("Expression was neither a Func nor a Text. Confused by what to do next.");
        }

        private static string EvaluateFunctionCall(Expr.Func func, ISpListItemContainer container)
        {
            var formContainer = container as IFormContainer;
            if (func.Item1.ToLower() == "!getclientformurl")
            {
                func.AssertFunctionArgumentLength(0);
                return container.SpListItem.GetAbsoluteUrl();
            }
            else if (func.Item1.ToLower() == "!getbrowserformurl")
            {
                if(func.Item2.Length==0)
                {
                    return container.SpListItem.GetBrowserFormUrl();
                }
                else if(func.Item2.Length==1)
                {
                    var redirectUrl = EvaluateEquation(func.Item2[0], container);
                    return container.SpListItem.GetBrowserFormUrl(redirectUrl);
                }
                else
                    throw new Exception(string.Format("the function [!getBrowserformurl] expects 0 or 1 paramter(s) but in this case it has been called with [{0}] paramters.", func.Item2.Length));
            }
            else if(func.Item1.ToLower()=="!listitemfield")
            {
                func.AssertFunctionArgumentLength(1);
                var listItemField = EvaluateEquation(func.Item2[0], container);
                return container.SpListItem.EvaluateFieldAsString(listItemField);
            }
            else if (func.Item1.ToLower() == "!xpath")
            {
                formContainer.AssertNotNull(container.SpListItem.GetDescription()+ " attempted to perform an infopath operation (xpath) on a non-infopath content type.");
                func.AssertFunctionArgumentLength(1);
                var xpath = EvaluateEquation(func.Item2[0],container);
                var node = formContainer.GetNodeByXpath(xpath);
                if (node == null)
                    throw new Exception(string.Format("The XPath value [{0}] was not found within the document: {1}", xpath, formContainer.Xml));
                return node.Value;
            }
            else if (func.Item1.ToLower() == "!evalxpath")
            {
                formContainer.AssertNotNull(container.SpListItem.GetDescription()+ " attempted to perform an infopath operation (evalxpath) on a non-infopath content type.");
                func.AssertFunctionArgumentLength(1);
                var expr = EvaluateEquation(func.Item2[0],container);
                return (formContainer.EvalXpathExpression(expr)??string.Empty).ToString();
            }
            else if (func.Item1.ToLower() == "!resolveuseremail")
            {
                func.AssertFunctionArgumentLength(2);
                var username = EvaluateEquation(func.Item2[0],container);
                var domain = EvaluateEquation(func.Item2[1],container);
                var spUser = ResolveType.Instance.Of<IUserIdentificationService>().GetSPUserByUserName(domain + @"\" + username, container.SpListItem.Web);
                return spUser.Email;
            }
            else if (func.Item1.ToLower() == "!resolveuserfullname")
            {
                func.AssertFunctionArgumentLength(2);
                var username = EvaluateEquation(func.Item2[0],container);
                var domain = EvaluateEquation(func.Item2[1],container);
                var spUser = ResolveType.Instance.Of<IUserIdentificationService>().GetSPUserByUserName(domain + @"\" + username, container.SpListItem.Web);
                return spUser.Name;
            }
            else if (func.Item1.ToLower() == "!getmanagerforuser")
            {
                func.AssertFunctionArgumentLength(2);
                var username = EvaluateEquation(func.Item2[0],container);
                var domain = EvaluateEquation(func.Item2[1],container);
                var spUser = ResolveType.Instance.Of<IUserIdentificationService>().GetManager(domain + @"\" + username, container.SpListItem.Web);
                var userName = spUser.LoginName;
                if (userName.IndexOf("\\") > 0)
                {
                    userName = userName.Substring(userName.IndexOf("\\") + 1);
                }
                return userName;
            }
            else if (func.Item1.ToLower() == "!if")
            {
                func.AssertFunctionArgumentLength(3);
                var conditionalExpr = (EvaluateEquation(func.Item2[0], container) ?? string.Empty).ToLower();
                var trueExpr = EvaluateEquation(func.Item2[1], container);
                var falseExpr = EvaluateEquation(func.Item2[2], container);
                return conditionalExpr.IsGweTrue() ? trueExpr : falseExpr;
            }
            else if (func.Item1.ToLower() == "!and")
            {
                if(func.Item2.Length<1)
                    throw new Exception("!and operator requires at least one argument.");
                for (var i = 0; i < func.Item2.Length; i++)
                {
                    var conditionalExpr = (EvaluateEquation(func.Item2[i], container) ?? string.Empty).ToLower();
                    if (!conditionalExpr.IsGweTrue())
                        return false.ToString();
                }
                return true.ToString();
            }
            else if (func.Item1.ToLower() == "!not")
            {
                func.AssertFunctionArgumentLength(1);
                var conditionalExpr = (EvaluateEquation(func.Item2[0], container) ?? string.Empty).ToLower();
                return (!(conditionalExpr.IsGweTrue())).ToString();
            }
            else if (func.Item1.ToLower() == "!htmldecode")
            {
                func.AssertFunctionArgumentLength(1);
                var expr = (EvaluateEquation(func.Item2[0], container) ?? string.Empty).ToLower();
                return HttpUtility.HtmlDecode(expr);
            }
            else if (func.Item1.ToLower() == "!htmlencode")
            {
                func.AssertFunctionArgumentLength(1);
                var expr = (EvaluateEquation(func.Item2[0], container) ?? string.Empty).ToLower();
                return HttpUtility.HtmlEncode(expr);
            }
            else if (func.Item1.ToLower() == "!or")
            {
                if(func.Item2.Length<1)
                    throw new Exception("!or operator requires at least one argument.");
                for (var i = 0; i < func.Item2.Length; i++)
                {
                    var conditionalExpr = (EvaluateEquation(func.Item2[i], container) ?? string.Empty).ToLower();
                    if (conditionalExpr.IsGweTrue())
                        return true.ToString();
                }
                return false.ToString();
            }
            else if (func.Item1.ToLower() == "!concatenate")
            {
                if(func.Item2.Length<1)
                    throw new Exception("!concatenate operator requires at least one argument.");
                var sb = new StringBuilder();
                for (var i = 0; i < func.Item2.Length; i++)
                {
                    sb.Append((EvaluateEquation(func.Item2[i], container) ?? string.Empty));                    
                }
                return sb.ToString();
            }
            else if (func.Item1.ToLower() == "!equals")
            {
                func.AssertFunctionArgumentLength(2);
                var a = EvaluateEquation(func.Item2[0], container);
                var b = EvaluateEquation(func.Item2[1], container);
                return (a.ToLower().Trim()==b.ToLower().Trim()).ToString();
            }
            else if (func.Item1.ToLower() == "!notequals")
            {
                func.AssertFunctionArgumentLength(2);
                var a = EvaluateEquation(func.Item2[0], container);
                var b = EvaluateEquation(func.Item2[1], container);
                return (a.ToLower().Trim()!=b.ToLower().Trim()).ToString();
            }
            else if (func.Item1.ToLower() == "!contains")
            {
                func.AssertFunctionArgumentLength(2);
                var a = EvaluateEquation(func.Item2[0], container);
                var b = EvaluateEquation(func.Item2[1], container);
                return (a.ToLower().Trim().Contains(b.ToLower().Trim())).ToString();
            }
            else if(func.Item1.ToLower()=="!iscontenttype")
            {
                func.AssertFunctionArgumentLength(1);
                var contentTypeName = EvaluateEquation(func.Item2[0], container);
                var contentType = container.SpListItem.Web.GetContentTypeByName(contentTypeName);
                return (container.SpListItem.ContentType.Id.IsChildOf(contentType.Id)).ToString();
            }
            else if(func.Item1.ToLower()=="!currentsiteurl")
            {
                var result = container.SpListItemStrict.Web.Site.Url;
                if (!result.EndsWith("/"))
                    result += "/";
                return result;
            }
            else if(func.Item1.ToLower()=="!add")
            {
                func.AssertFunctionArgumentLength(2);
                var a = EvaluateEquation(func.Item2[0], container);
                var b = EvaluateEquation(func.Item2[1], container);
                int aInt, bInt;
                double aFloat, bFloat;
                if(int.TryParse(a, out aInt) && int.TryParse(b, out bInt))
                {
                    return (aInt + bInt).ToString();
                }
                else if(double.TryParse(a, out aFloat) && double.TryParse(b, out bFloat) )
                {
                    return (aFloat + bFloat).ToString();
                }
                else
                {
                    throw new Exception(string.Format("In the add function, either the expression [{0}] or the expression [{1}] did not evaluate to a numeric value, hence the add operation failed.", a, b));
                }
            }
            else if(func.Item1.ToLower()=="!adddate")
            {
                func.AssertFunctionArgumentLength(3);
                var dateVal = EvaluateEquation(func.Item2[0], container);
                var intval = EvaluateEquation(func.Item2[1], container);
                var scopeVal = EvaluateEquation(func.Item2[2], container);
                
                DateTime date;
                int interval;
                try
                {
                    date = DateTime.Parse(dateVal);
                }
                catch(Exception exp)
                {
                    throw new Exception(string.Format("adddate: Unable to parse the expression [{0}] into a date value: {1}", dateVal, exp));
                }
                try
                {
                    interval = int.Parse(intval);
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("adddate: Unable to parse the expression [{0}] into a int value for the interval: {1}", intval, exp));
                }
                if(scopeVal.ToLower().Trim()=="hours")
                    date = date.AddHours(interval);
                else if(scopeVal.ToLower().Trim()=="minutes")
                    date = date.AddMinutes(interval);
                else if (scopeVal.ToLower().Trim() == "seconds")
                    date = date.AddSeconds(interval);
                else if (scopeVal.ToLower().Trim() == "days")
                    date = date.AddDays(interval);
                else
                {
                    throw new Exception(string.Format("The interval should be within the set of values [hours, minutes, seconds, days]. Actual interval was : [{0}]", interval));
                }
                return date.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            else if(func.Item1.ToLower()=="!currentweburl")
            {
                var result = container.SpListItemStrict.Web.Url;
                if (!result.EndsWith("/"))
                    result += "/";
                return result;
            }
            else if(func.Item1.ToLower()=="!now")
            {
                func.AssertFunctionArgumentLength(0);
                return DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            else if(func.Item1.ToLower()=="!today")
            {
                func.AssertFunctionArgumentLength(0);
                return DateTime.Now.ToString("yyyy-MM-dd");
            }
            else if(func.Item1.ToLower()=="!var")
            {
                func.AssertFunctionArgumentLength(1);
                var variableName = EvaluateEquation(func.Item2[0], container);
                var workflowConfigVariable = container.WorkflowConfigurationStrict.WorkflowConfigurationVariables.FirstOrDefault(x => x.Name == variableName);
                if(workflowConfigVariable==null)
                    throw new Exception(string.Format("There is no variable named [{0}].", variableName));
                return ResolveType.Instance.Of<ITemplateEngine>().Render(workflowConfigVariable.Value, container);
            }

            else
                throw new Exception(string.Format("No recognized function :{0}", func.Item1));
        }
    }
}