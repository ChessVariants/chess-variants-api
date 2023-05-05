using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataAccess.MongoDB.Models;
public record MoveTemplate : IModel
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
    [BsonElement("actions")]
    public List<ActionRec> Actions { get; set; } = null!;
    [BsonElement("identifier")]
    public string Identifier { get; set; } = null!;
    [BsonElement("click")]
    public Position Click { get; set; } = null!;

}
