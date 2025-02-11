using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Constant
{
    public class APIEndPointConstant
    {
        static APIEndPointConstant()
        {
        }
        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1";
        public const string ApiEndpoint = RootEndPoint + ApiVersion;
        public static class Account
        {
            public const string GetAccountById = ApiEndpoint + "/account/{id}";
            public const string AccountEndpoint = ApiEndpoint + "/account";
            public const string Register = AccountEndpoint + "/sys-admin";

        }
        public static class Authentication
        {
            public const string AuthenticationEndpoint = ApiEndpoint + "/auth";
            public const string Login = AuthenticationEndpoint + "/login";
            public const string UpdatePassword = AuthenticationEndpoint + "/changepass";
        }
    }
}
