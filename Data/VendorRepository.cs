using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

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
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error inserting vendor: {ex.Message}");
            return false;
        }
    }

    // ✅ إرجاع كل Vendors حسب OwnerUserId
       public List<vendorglobaldto> GetVendorsByOwnerId(string ownerUserId)
    {
        var vendors = new List<vendorglobaldto>();

        using var connection = Database.GetConnection();
        connection.Open();

        const string sql = @"SELECT Id, OwnerUserId, Name, Description, CreatedAt 
                             FROM Vendor 
                             WHERE OwnerUserId = @OwnerUserId;";

        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@OwnerUserId", ownerUserId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var vendor = new vendorglobaldto
            {
                Id = reader.GetString(0),
                OwnerUserId = reader.GetString(1),
                Name = reader.GetString(2),
                Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                CreatedAt = DateTime.Parse(reader.GetString(4))
            };

            vendors.Add(vendor);
        }

        return vendors;
    }
    // ✅ إرجاع Vendor واحد حسب Id و OwnerUserId
public vendorglobaldto? GetVendorByIdAndOwner(string vendorId, string ownerUserId)
{
    using var connection = Database.GetConnection();
    connection.Open();

    const string sql = @"SELECT Id, OwnerUserId, Name, Description, CreatedAt 
                         FROM Vendor 
                         WHERE Id = @VendorId AND OwnerUserId = @OwnerUserId;";

    using var cmd = new SqliteCommand(sql, connection);
    cmd.Parameters.AddWithValue("@VendorId", vendorId);
    cmd.Parameters.AddWithValue("@OwnerUserId", ownerUserId);

    using var reader = cmd.ExecuteReader();
    if (reader.Read())
    {
        return new vendorglobaldto
        {
            Id = reader.GetString(0),
            OwnerUserId = reader.GetString(1),
            Name = reader.GetString(2),
            Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
            CreatedAt = DateTime.Parse(reader.GetString(4))
        };
    }

    return null; // إذا ما لقي Vendor يخص هذا Owner
}

}


