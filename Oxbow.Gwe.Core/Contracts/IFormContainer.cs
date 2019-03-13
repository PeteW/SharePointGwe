using System.Xml.XPath;

namespace Oxbow.Gwe.Core.Contracts
{
    public interface IFormContainer:ISpListItemContainer
    {
        string Xml { get; }
        string GetValueByXpath(string xPath, string defaultValue);
        void SetNodeToNil(string xPath);
        void SetNodeByXpath(string xPath, string value);
        XPathNavigator GetNodeByXpath(string xPath);
        void AppendChildXml(string parentNodeXpath, bool prepend, string xml);
        void DeleteNode(string xpath);
        string EvalXpathExpression(string xpathExpression);
    }
}