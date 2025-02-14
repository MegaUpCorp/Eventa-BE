using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Eventa_BusinessObject.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            InsDate = DateTime.Now;
            UpdDate = DateTime.Now;
            DelFlg = false;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)] // Chuyển Guid thành chuỗi khi lưu vào MongoDB
        public virtual Guid Id { get; set; } = Guid.NewGuid();

        [BsonElement("insDate")]  // Đánh dấu trường này cho MongoDB
        public DateTime InsDate { get; set; }

        [BsonElement("updDate")]  // Đánh dấu trường này cho MongoDB
        public DateTime UpdDate { get; set; }

        [BsonElement("delFlg")]  // Đánh dấu trường này cho MongoDB
        public bool DelFlg { get; set; } = false;
    }
}