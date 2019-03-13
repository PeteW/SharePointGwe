using System;
using System.Collections.Generic;
using System.Xml.XPath;
using Oxbow.Gwe.Core.Configuration;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;
using Oxbow.Gwe.Core.Utils;
using Oxbow.Gwe.Lang.Ast;

namespace Oxbow.Gwe.Core.TemplateEngine
{
    public class GweLangTemplateEngine : ITemplateEngine
    {
        #region ITemplateEngine Members

        public List<string> GetOperandsFromFunctionCall(string functionCall, ISpListItemContainer container)
        {
            container.AssertNotNull("ISpListItemContainer cannot be null.");
            string[] equations = CommonCode.GetEquationsFromTemplate(functionCall);
            if (equations.Length > 1)
                throw new Exception(string.Format("The code block [{0}] contains more than one function call, and so we cannot return the operands.", functionCall));
            return ExpressionEvaluator.GetOperandsFromCodeBlock(equations[0], container);
        }

        public string Render(string template, ISpListItemContainer container)
        {
            container.AssertNotNull("ISpListItemContainer cannot be null.");
            string[] codeBlocks = CommonCode.GetCodeBlocksFromTemplate(template);
            foreach (string codeBlock in codeBlocks)
            {
                string code = codeBlock.Trim(new[] { '$', '{', '}' });
                string result = string.Empty;
                Equation eq = null;
                try
                {
                    eq = ExpressionEvaluator.ParseEquation(code);
                }
                catch
                {
                    throw new Exception(string.Format("Unable to parse expression due to a syntax error: [{0}]", codeBlock));
                }
                try
                {
                    result = ExpressionEvaluator.EvaluateEquation(eq, container);
                }
                catch (Exception exp)
                {
                    throw new Exception(string.Format("Error occurred when running the expression: [{0}] : [{1}]", codeBlock, exp));
                }
                template = template.Replace(codeBlock, result);
            }
            return template;
        }

        #endregion
    }
}