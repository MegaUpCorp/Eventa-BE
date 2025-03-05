using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Eventa_BusinessObject.Validations
{
    public class CustomGuidListSerializer : SerializerBase<List<Guid>>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, List<Guid> value)
        {
            var bsonWriter = context.Writer;
            bsonWriter.WriteStartArray();
            foreach (var guid in value)
            {
                bsonWriter.WriteString(guid.ToString());
            }
            bsonWriter.WriteEndArray();
        }

        public override List<Guid> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            var list = new List<Guid>();

            if (bsonReader.CurrentBsonType == BsonType.Array)
            {
                bsonReader.ReadStartArray();
                while (bsonReader.State != BsonReaderState.EndOfArray)
                {
                    var guidStr = bsonReader.ReadString();
                    list.Add(Guid.Parse(guidStr));
                }
                bsonReader.ReadEndArray();
            }
            else
            {
                Console.WriteLine($"Unexpected BSON type: {bsonReader.CurrentBsonType}");
                throw new InvalidOperationException("Expected an array but found a different BSON type.");
            }

            return list;
        }
    }
    }
