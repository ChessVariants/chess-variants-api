namespace DataAccess.MongoDB.Models;

/// <summary>
/// Each mongoDB document contains an object id, which this interface represents.
/// </summary>
public interface IModel
{
    public string Id { get; set; }
}
