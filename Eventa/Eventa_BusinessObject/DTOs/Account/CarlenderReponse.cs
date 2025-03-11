using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventa_BusinessObject.DTOs.Account
{
    public class CarlenderReponse
    {
        [BsonId]
        [BsonElement("id")]
        public Guid Id { get; set; }
        [BsonElement("name")]
        public required string Name { get; set; }
        [BsonElement("profilePicture")]
        public string? ProfilePicture { get; set; }
        [BsonElement("publicUrl")]
        public string? PublicUrl { get; set; }
        [BsonElement("subscribedAccounts")]
        public List<Guid> SubscribedAccounts { get; set; }
    }
}
