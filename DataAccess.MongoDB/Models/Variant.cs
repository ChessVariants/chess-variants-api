using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccess.MongoDB.Models;

/// <summary >
/// Model for a variant document.
/// </summary >
public class Variant : IModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    public string Creator { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string VariantData { get; set; } = null!;
}