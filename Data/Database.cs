using Microsoft.Data.Sqlite;

public static class Database
{
    private static string connectionString = "Data Source=shop.db";

    public static SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}
