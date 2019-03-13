namespace Oxbow.Gwe.Core.WebServiceModels
{
    public class AppendXmlRequest:BaseWebServiceRequestModel
    {
        public string FileName { get; set; }
        public string ListName { get; set; }
        public string Xml { get; set; }
        public bool Prepend { get; set; }
        public string ParentNodeXpath { get; set; }
    }
    public class AppendXmlResponse:BaseWebServiceResponseModel
    {
         
    }
}