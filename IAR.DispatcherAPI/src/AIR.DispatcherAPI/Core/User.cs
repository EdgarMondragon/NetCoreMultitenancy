using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace IAR.DispatcherAPI.Core
{
    public class User : IIdentity
    {
        public User(string authenticationType, bool isAuthenticated, string name, IarUserTypes userType, string password, string apiKey, DateTime createdOn)
        {
            AuthenticationType = authenticationType;
            IsAuthenticated = isAuthenticated;
            Name = name;
            UserType = userType;
            Password = password;
            ApiKey = apiKey;
            CreatedOn = createdOn;
        }

        public User()
        {
        }

        public enum IarUserTypes
        {
            Administrator = 0,
            MasterAdmin,
            Dispatcher,
            User,
            DispatcherUser,
            Apparatus
        }
        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public string Name { get; set; }
        public IarUserTypes UserType { get; private set; }
        public string Password { get; set; }
        public string ApiKey { get; set; }
        public DateTime CreatedOn { get; private set; }
    }
}
