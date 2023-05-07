using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataAccess.MongoDB.Models;
public record RuleSetModel : IModel
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
    [BsonElement("predicate")]
    public string Predicate { get; set; } = null!;
    [BsonElement("moves")]
    public List<string> Moves { get; set; } = null!;
    [BsonElement("events")]
    public List<string> Events { get; set; } = null!;
    [BsonElement("stalemateEvents")]
    public List<string> StalemateEvents { get; set; } = null!;

}