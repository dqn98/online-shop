using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Contracts.Domains;

public abstract class MongoEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public virtual string? Id { get; protected init; }
    
    [BsonElement("createdDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedDate { get; set; }
    
    [BsonElement("lastModified")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? LastModified { get; set; }
}