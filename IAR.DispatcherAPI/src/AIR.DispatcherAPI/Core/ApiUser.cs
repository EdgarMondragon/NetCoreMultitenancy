using System;
using System.Security.Principal;

namespace IAR.DispatcherAPI.Core
{
    public class ApiUser: IPrincipal
    {
        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
        public IIdentity Identity { get; private set; }

        public User User => Identity as User;
        public ApiUser(IIdentity identity)
        {
            Identity = identity;
        }
    }
}
