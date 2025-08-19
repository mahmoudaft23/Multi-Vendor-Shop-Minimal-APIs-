using Microsoft.Data.Sqlite;
using System;

public class ProductRepository
{
    public void CreateTable()
    {
        using var connection = Database.GetConnection();
        const string sql = @"
CREATE TABLE IF NOT EXISTS Product (
    Id TEXT PRIMARY KEY,
    VendorId TEXT NOT NULL,
    Name TEXT NOT NULL,
    Description TEXT,
    Price REAL NOT NULL,
    Stock INTEGER NOT NULL,
    IsActive INTEGER NOT NULL CHECK (IsActive IN (0,1)),
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT NOT NULL,
    FOREIGN KEY (VendorId) REFERENCES Vendor(Id) ON DELETE CASCADE
);

CREATE TRIGGER IF NOT EXISTS trg_Product_Update_UpdatedAt
AFTER UPDATE ON Product
FOR EACH ROW
BEGIN
    UPDATE Product
    SET UpdatedAt = strftime('%Y-%m-%d %H:%M:%S','now')
    WHERE Id = NEW.Id;
END;";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    public void AddProduct(
        string id,
        string vendorId,
        string name,
        string? description,
        decimal price,
        int stock,
        bool isActive,
        DateTime createdAt,
        DateTime updatedAt)
    {
        using var connection = Database.GetConnection();
        const string sql = @"
INSERT INTO Product (Id, VendorId, Name, Description, Price, Stock, IsActive, CreatedAt, UpdatedAt)
VALUES (@Id, @VendorId, @Name, @Description, @Price, @Stock, @IsActive, @CreatedAt, @UpdatedAt);";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@VendorId", vendorId);
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Description", (object?)description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Price", price);
        cmd.Parameters.AddWithValue("@Stock", stock);
        cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.Parameters.AddWithValue("@UpdatedAt", updatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }
}
