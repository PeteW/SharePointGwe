using System.Collections.Generic;
using Microsoft.SharePoint;

namespace Oxbow.Gwe.Core.Contracts
{
    public interface IUserIdentificationService
    {
        SPUser GetSPUserByEmail(string email, SPWeb webContext);
        SPUser GetSPUserByUserName(string userName, SPWeb webContext);
        SPGroup GetSPGroupByName(string groupName, SPWeb webContext);
        SPUser GetManager(string userName, SPWeb webContext);
        bool IsUserMemberOfSpGroup(string userName, string groupName, SPWeb webContext);
    }
}