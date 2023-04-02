namespace DataAccess.MongoDB;

/// <summary>
/// This class connects to the testing database in MongoDB and can be used freely for testing.
/// </summary>
public class TestDatabaseService : DatabaseService
{
    public TestDatabaseService(string connectionString) : base(connectionString, "Test") {}
}
