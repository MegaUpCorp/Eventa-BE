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

        [BsonId]  // Đánh dấu thuộc tính này là ID trong MongoDB
        [BsonRepresentation(BsonType.String)]  // Có thể sử dụng Guid dưới dạng chuỗi
        public virtual Guid Id { get; set; }

        [BsonElement("insDate")]  // Đánh dấu trường này cho MongoDB
        public DateTime InsDate { get; set; }

        [BsonElement("updDate")]  // Đánh dấu trường này cho MongoDB
        public DateTime UpdDate { get; set; }

        [BsonElement("delFlg")]  // Đánh dấu trường này cho MongoDB
        public bool DelFlg { get; set; } = false;
    }
}