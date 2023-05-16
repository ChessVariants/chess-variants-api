
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataAccess.MongoDB.Models;
public record Predicate : IModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("creatorName")]
    public string CreatorName { get; set; } = null!;
    [BsonElement("name")]
    public string Name { get; set; } = null!;
    [BsonElement("description")]
    public string Description { get; set; } = null!;
    [BsonElement("code")]
    public string Code { get; set; } = null!;
}
