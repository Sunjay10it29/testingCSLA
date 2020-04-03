using System;
using System.Collections.Generic;
using Csla;

namespace WebApplication1.Data
{
  [Serializable]
    public class CustomIdentity : Csla.Security.CslaIdentityBase<CustomIdentity>
    {
        private static Dictionary<string, string> Users = new Dictionary<string, string>
        {
            { "admin", "kop" },
            { "supervisor", "moo" },
            { "manager", "sunny" }
        };

        [Fetch]
        private void Fetch(UserCredentials credentials)
        {
            // validate credentials here
            if (!string.IsNullOrWhiteSpace(credentials.Username) &&
             Users.ContainsKey(credentials.Username.ToLower()) == true
             && Users.ContainsValue(credentials.Password) == true)
            {
                Name = credentials.Username;
                IsAuthenticated = true;
                AuthenticationType = "Password";
                Roles = new Csla.Core.MobileList<string>
                {

                };
                Roles.Add("admin");
                Roles.Add("supervisor");
                Roles.Add("manager");
            }
            else
            {
                Name = string.Empty;
                IsAuthenticated = false;
                AuthenticationType = string.Empty;
                Roles = new Csla.Core.MobileList<string>();
            }
        }
    }
}
