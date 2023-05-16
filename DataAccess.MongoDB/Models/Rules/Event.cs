using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DataAccess.MongoDB.Models;
public record EventModel : IModel
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

}

public record ActionRec
{
    [BsonElement("win")]
    public Win? Win { get; set; } = null!;
    [BsonElement("set")]
    public SetPiece Set { get; set; } = null!;
    [BsonElement("move")]
    public MovePiece Move { get; set; } = null!;
    [BsonElement("isTie")]
    public bool IsTie { get; set; } = false;
    [BsonElement("isPromotion")]
    public bool IsPromotion { get; set; } = false;
}

public record Win
{
    [BsonElement("whiteWins")]
    public bool WhiteWins { get; set; } = false;
}

public record SetPiece
{
    [BsonElement("identifier")]
    public string Identifier { get; set; } = null!;
    [BsonElement("at")]
    public Position At { get; set; } = null!;
}
public record MovePiece
{
    [BsonElement("from")]
    public Position From { get; set; } = null!;
    [BsonElement("to")]
    public Position To { get; set; } = null!;
}

public record Position
{
    [BsonElement("positionAbsolute")]
    public PositionAbsoluteModel PositionAbsolute { get; set; } = null!;
    [BsonElement("positionRelative")]
    public PositionRelativeModel PositionRelative { get; set; } = null!;
}

public record PositionAbsoluteModel
{
    [BsonElement("coordinate")]
    public string Coordinate { get; set; } = null!;
}


public record PositionRelativeModel
{
    [BsonElement("x")]
    public int X { get; set; } = 0;
    [BsonElement("y")]
    public int Y { get; set; } = 0;
    [BsonElement("to")]
    public bool To { get; set; } = false;
}

