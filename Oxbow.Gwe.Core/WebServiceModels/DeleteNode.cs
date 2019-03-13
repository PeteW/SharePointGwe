namespace Oxbow.Gwe.Core.WebServiceModels
{
    public class DeleteNodeResponse:BaseWebServiceResponseModel
    {
        
    }
    public class DeleteNodeRequest:BaseWebServiceRequestModel
    {
        public string FileName { get; set; }
        public string ListName { get; set; }
        public string NodeXpath { get; set; }
    }
}