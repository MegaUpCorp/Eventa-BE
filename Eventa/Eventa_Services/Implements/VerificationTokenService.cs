using Eventa_Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_Services.Implements
{
    public class VerificationTokenService : IVerificationTokenService
    {
        private readonly IMemoryCache _cache;

        public VerificationTokenService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateToken(string email)
        {
            var token = Guid.NewGuid().ToString();
            _cache.Set(token, email, TimeSpan.FromDays(7)); // Token có hiệu lực 1 tiếng
            return token;
        }

        public bool ValidateToken(string token, out string email)
        {
            return _cache.TryGetValue(token, out email);
        }
    }
}
