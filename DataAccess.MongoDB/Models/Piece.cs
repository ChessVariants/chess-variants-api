using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MongoDB.Models;
public record Piece : IModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("creator")]
    public string Creator { get; set; } = null!;

    [BsonElement("repeat")]
    public int Repeat { get; set; } = 0;

    [BsonElement("canBeCaptured")]
    public bool CanBeCaptured { get; set; } = true;

    [BsonElement("imagePath")]
    public string ImagePath { get; set; } = null!;

    [BsonElement("movement")]
    public List<MovePattern> Movement { get; set; } = new List<MovePattern>();

    [BsonElement("captures")]
    public List<MovePattern> Captures { get; set; } = new List<MovePattern>();
}

public record MovePattern
{
    [BsonElement("xDir")]
    public int XDir { get; set; } = 0;

    [BsonElement("yDir")]
    public int YDir { get; set; } = 0;

    [BsonElement("minLength")]
    public int MinLength { get; set; } = 0;

    [BsonElement("maxLength")]
    public int MaxLength { get; set; } = 0;
}
