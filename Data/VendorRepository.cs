using Microsoft.Data.Sqlite;
using System;

public class VendorRepository
{
    public void CreateTable()
    {
        using var connection = Database.GetConnection();
        const string sql = @"
CREATE TABLE IF NOT EXISTS Vendor (
    Id TEXT PRIMARY KEY,
    OwnerUserId TEXT NOT NULL,
    Name TEXT NOT NULL,
    Description TEXT,
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY (OwnerUserId) REFERENCES Users(Id) ON DELETE CASCADE
);";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    public bool AddVendor(string id, string ownerUserId, string name, string? description, DateTime createdAt)
{
    try
    {
        using var connection = Database.GetConnection();
        connection.Open();

        const string sql = @"
INSERT INTO Vendor (Id, OwnerUserId, Name, Description, CreatedAt)
VALUES (@Id, @OwnerUserId, @Name, @Description, @CreatedAt);";

        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@OwnerUserId", ownerUserId);
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Description", (object?)description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss"));

        int rowsAffected = cmd.ExecuteNonQuery();
        return rowsAffected > 0; // ✅ true إذا أضاف صف واحد على الأقل
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error inserting vendor: {ex.Message}");
        return false; // ❌ صار خطأ
    }
}

}
