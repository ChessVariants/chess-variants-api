using DataAccess.MongoDB.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.MongoDB.Repositories;
public class VariantRepository : GenericRepository<Variant>
{
    public const string CollectionName = "Variants";

    public VariantRepository(IMongoDatabase database) : base(database.GetCollection<Variant>(CollectionName))
    {
    }
}
