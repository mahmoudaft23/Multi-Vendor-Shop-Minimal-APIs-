using Microsoft.Data.Sqlite;
using System;

public class OrderItemRepository
{
    public void CreateTable()
    {
        using var connection = Database.GetConnection();
        const string sql = @"
CREATE TABLE IF NOT EXISTS OrderItem (
    Id TEXT PRIMARY KEY,
    OrderId TEXT NOT NULL,
    ProductId TEXT NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitPrice REAL NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES [Order](Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Product(Id) ON DELETE CASCADE
);";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    public void AddOrderItem(string id, string orderId, string productId, int quantity, decimal unitPrice)
    {
        using var connection = Database.GetConnection();
        const string sql = @"
INSERT INTO OrderItem (Id, OrderId, ProductId, Quantity, UnitPrice)
VALUES (@Id, @OrderId, @ProductId, @Quantity, @UnitPrice);";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        cmd.Parameters.AddWithValue("@ProductId", productId);
        cmd.Parameters.AddWithValue("@Quantity", quantity);
        cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
        cmd.ExecuteNonQuery();
    }
}
