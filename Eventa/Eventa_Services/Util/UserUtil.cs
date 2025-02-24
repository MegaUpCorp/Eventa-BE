using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Eventa_Services.Util
{
    public class UserUtil
    {

        public static Guid? GetAccountId(HttpContext httpContext)
        {
            if (httpContext == null || httpContext.User == null)
            {
                return null;
            }

            var nameIdentifierClaim = httpContext.User.FindFirst("id");

            if (nameIdentifierClaim == null)
            {
                return null;
            }

            if (!Guid.TryParse(nameIdentifierClaim.Value, out Guid accountId))
            {
                throw new BadHttpRequestException(nameIdentifierClaim.Value);

            }
            return accountId;
        }





        public static string GetName(HttpContext httpContext)
        {
                var nameClaim = httpContext.User.FindFirst(ClaimTypes.Name);
                return nameClaim?.Value;
        }

        public static string GetRoleName(HttpContext httpContext)
        {
            var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value;
        }

    }
}
