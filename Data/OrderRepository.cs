using Microsoft.Data.Sqlite;
using System;

public class OrderRepository
{
    public void CreateTable()
    {
        using var connection = Database.GetConnection();
        const string sql = @"
CREATE TABLE IF NOT EXISTS [Order] (
    Id TEXT PRIMARY KEY,
    CustomerUserId TEXT NOT NULL,
    Status TEXT NOT NULL CHECK (Status IN ('Pending','Paid','Shipped','Cancelled')),
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY (CustomerUserId) REFERENCES Users(Id) ON DELETE CASCADE
);";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    public void AddOrder(string id, string customerUserId, string status, DateTime createdAt)
    {
        using var connection = Database.GetConnection();
        const string sql = @"
INSERT INTO [Order] (Id, CustomerUserId, Status, CreatedAt)
VALUES (@Id, @CustomerUserId, @Status, @CreatedAt);";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@CustomerUserId", customerUserId);
        cmd.Parameters.AddWithValue("@Status", status); // تأكد من إحدى القيم الأربع
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }
}
