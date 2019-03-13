namespace Oxbow.Gwe.Core.WebServiceModels
{
    public class GetUserFullIdentityRequest:BaseWebServiceRequestModel
    {
        public string DomainName { get; set; }
        public string UserName { get; set; }
    }
    public class GetUserFullIdentityResponse:BaseWebServiceResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}