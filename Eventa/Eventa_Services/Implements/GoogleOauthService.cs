using Eventa_Services.Interfaces;
using Eventa_Services.Share;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;

namespace Eventa_Services.Implements
{
    public class GoogleOauthService : IGoogleOauthService
    {
        public async Task<Result<object>> DecodeAccessToken(string accessToken)
        {
            try
            {
                var googleInitializer = new BaseClientService.Initializer();
                googleInitializer.ApiKey = "SecretKey";
                Oauth2Service ser = new Oauth2Service(googleInitializer);
                Oauth2Service.TokeninfoRequest req = ser.Tokeninfo();
                req.AccessToken = accessToken;
                Tokeninfo userinfo = await req.ExecuteAsync();
                return new Result<object>
                {
                    Error = 0,
                    Message = "Success",
                    Data = new GoogleOauthDecode()
                    {
                        Email = userinfo.Email,
                        FullName = userinfo.Email
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid token.");
                Console.WriteLine(e.Message);
                return new Result<object>
                {
                    Error = 1,
                    Message = "Invalid token",
                    Data = null
                };
            }
        }
        public class GoogleOauthDecode
        {
            public string FullName { get; set; }
            public string Email { get; set; }

        }
    }
 }
