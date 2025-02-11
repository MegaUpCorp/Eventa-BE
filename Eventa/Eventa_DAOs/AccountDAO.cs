using Eventa_BusinessObject.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Eventa_DAOs
{
    public class AccountDAO : BaseDAO<Account>
    {
        public AccountDAO(IMongoDatabase database) : base(database, "Accounts") { }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _collection.Find(a => a.Email == email).FirstOrDefaultAsync();
        }

        public async Task<Account?> GetByUsernameAsync(string username)
        {
            return await _collection.Find(a => a.Username == username).FirstOrDefaultAsync();
        }

        public async Task<Account?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _collection.Find(a => a.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
        }
    }
}
