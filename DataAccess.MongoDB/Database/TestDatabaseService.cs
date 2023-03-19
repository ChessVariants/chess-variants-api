namespace DataAccess.MongoDB;

public class TestDatabaseService : DatabaseService
{
    public TestDatabaseService(string connectionString) : base(connectionString, "Test") {}
}
