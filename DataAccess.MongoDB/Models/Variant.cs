using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.MongoDB.Models;
public record Variant : IModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("creator")]
    public string Creator { get; set; } = null!;

    [BsonElement("description")]
    public string Description { get; set; } = null!;

    [BsonElement("code")]
    public string Code { get; set; } = null!;

    [BsonElement("whiteRuleSetIdentifier")]
    public string WhiteRuleSetIdentifier { get; set; } = null!;

    [BsonElement("blackRuleSetIdentifier")]
    public string BlackRuleSetIdentifier { get; set; } = null!;

    [BsonElement("boardIdentifier")]
    public string BoardIdentifier { get; set; } = null!;

    [BsonElement("movesPerTurn")]
    public int MovesPerTurn { get; set; } = 1;
}
