using System.Collections.Generic;

namespace Oxbow.Gwe.Core.WebServiceModels
{
    public class GetSharePointUsersByGroupResponseItem
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
    public class GetSharePointUsersByGroupResponse:BaseWebServiceResponseModel
    {
        public List<GetSharePointUsersByGroupResponseItem> Items { get; set; }
    }
    public class GetSharePointUsersByGroupRequest:BaseWebServiceRequestModel
    {
        public string GroupName { get; set; }
    }
}