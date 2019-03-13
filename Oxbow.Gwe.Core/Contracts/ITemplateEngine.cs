using System.Collections.Generic;
using Oxbow.Gwe.Core.Contracts;
using Oxbow.Gwe.Core.Models;

namespace Oxbow.Gwe.Core.Configuration
{
    public interface ITemplateEngine
    {
        List<string> GetOperandsFromFunctionCall(string functionCall, ISpListItemContainer container);
        string Render(string template, ISpListItemContainer container);
    }
}