using DataAccess.MongoDB.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace DataAccess.MongoDB;

/// <summary>
/// This class has functionality useful for all repositories which are standard CRUD operations.
/// The methods abstract operations on a <see cref="IMongoCollection{TDocument}"/>
/// </summary>
/// <typeparam name="T">Any implementation of <see cref="IModel"/></typeparam>
public class GenericRepository<T> where T : IModel
{
    readonly protected IMongoCollection<T> _collection;

    public GenericRepository(IMongoCollection<T> collection)
    {
        _collection = collection;
    }

    public async Task<List<T>> GetAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<T?> GetAsync(string id)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T newT)
    {
        await _collection.InsertOneAsync(newT);
    }

    public async Task UpdateAsync(string id, T updatedT)
    {
        await _collection.ReplaceOneAsync(x => x.Id == id, updatedT);
    }

    public async Task RemoveAsync(string id)
    {
        await _collection.DeleteOneAsync(x => x.Id == id);
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await _collection.Find(expression).ToListAsync();
    }
}
