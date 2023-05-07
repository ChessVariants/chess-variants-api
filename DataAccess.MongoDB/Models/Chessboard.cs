using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccess.MongoDB.Models;
public record Chessboard : IModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("creator")]
    public string Creator { get; set; } = null!;

    [BsonElement("board")]
    public List<string> Board { get; set; } = new List<string>();

}