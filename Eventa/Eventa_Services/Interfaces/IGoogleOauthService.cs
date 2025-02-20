using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eventa_Services.Share;


namespace Eventa_Services.Interfaces
{
    public interface IGoogleOauthService
    {
        Task<Result<object>> DecodeAccessToken(string accessToken);

    }
}
