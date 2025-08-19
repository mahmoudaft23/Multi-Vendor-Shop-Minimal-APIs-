using Microsoft.Data.Sqlite;
using System;

public class UserRepository
{
    public void CreateTable()
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = @"CREATE TABLE IF NOT EXISTS Users (
                           Id TEXT PRIMARY KEY, 
                           Email TEXT NOT NULL UNIQUE,
                           PasswordHash TEXT NOT NULL,
                           Role TEXT CHECK(Role IN ('Admin', 'Vendor', 'Customer')) NOT NULL,
                           DisplayName TEXT NOT NULL,
                           CreatedAt TEXT NOT NULL
                        );";
        using var cmd = new SqliteCommand(query, connection);
        cmd.ExecuteNonQuery();
    }

   public bool AddUser(string id, string email, string passwordHash, string role, string displayName, DateTime createdAt)
{
    try
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = @"INSERT INTO Users (Id, Email, PasswordHash, Role, DisplayName, CreatedAt) 
                         VALUES (@Id, @Email, @PasswordHash, @Role, @DisplayName, @CreatedAt)";

        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
        cmd.Parameters.AddWithValue("@Role", role);
        cmd.Parameters.AddWithValue("@DisplayName", displayName);
        cmd.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss"));

        cmd.ExecuteNonQuery();
        return true;
    }
    catch (SqliteException ex)
    {
        // كود الخطأ 19 يعني UNIQUE constraint failed (إيميل مكرر)
        if (ex.SqliteErrorCode == 19)
        {
            Console.WriteLine("❌ Email already exists");
            return false;
        }

        Console.WriteLine($"Database error: {ex.Message}");
        return false;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
        return false;
    }
}


    // ✅ Update: تعديل بيانات المستخدم (باستخدام Id كمفتاح)
    public bool UpdateUser(string id, string? email = null, string? passwordHash = null, string? role = null, string? displayName = null)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        // نبني SET ديناميكيًا حسب القيم غير الفارغة
        var setParts = new System.Collections.Generic.List<string>();
        if (!string.IsNullOrWhiteSpace(email)) setParts.Add("Email = @Email");
        if (!string.IsNullOrWhiteSpace(passwordHash)) setParts.Add("PasswordHash = @PasswordHash");
        if (!string.IsNullOrWhiteSpace(role)) setParts.Add("Role = @Role");
        if (!string.IsNullOrWhiteSpace(displayName)) setParts.Add("DisplayName = @DisplayName");

        if (setParts.Count == 0) return false; // لا يوجد شيء للتعديل

        string query = $"UPDATE Users SET {string.Join(", ", setParts)} WHERE Id = @Id";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        if (!string.IsNullOrWhiteSpace(email)) cmd.Parameters.AddWithValue("@Email", email);
        if (!string.IsNullOrWhiteSpace(passwordHash)) cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
        if (!string.IsNullOrWhiteSpace(role)) cmd.Parameters.AddWithValue("@Role", role);
        if (!string.IsNullOrWhiteSpace(displayName)) cmd.Parameters.AddWithValue("@DisplayName", displayName);

        int affected = cmd.ExecuteNonQuery();
        return affected > 0;
    }

    // ❌ Delete: حذف مستخدم عبر Id
    public bool DeleteUser(string id)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = "DELETE FROM Users WHERE Id = @Id";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", id);

        int affected = cmd.ExecuteNonQuery();
        return affected > 0;
    }

    // ✅ Check: هل المستخدم موجود عبر Id؟
    public bool UserExistsById(string id)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = "SELECT 1 FROM Users WHERE Id = @Id LIMIT 1";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", id);

        var result = cmd.ExecuteScalar();
        return result != null;
    }

    // ✅ Check: هل البريد موجود مسبقًا؟
    public bool UserExistsByEmail(string email)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = "SELECT 1 FROM Users WHERE Email = @Email LIMIT 1";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Email", email);

        var result = cmd.ExecuteScalar();
        return result != null;
    }

    // ✅ Check/Verify: التحقق من بيانات الدخول (Email + PasswordHash)
    public bool VerifyCredentials(string email, string passwordHash)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = @"SELECT 1 FROM Users 
                         WHERE Email = @Email AND PasswordHash = @PasswordHash
                         LIMIT 1";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

        var result = cmd.ExecuteScalar();
        return result != null;
    }

    // (اختياري) جلب دور المستخدم عبر البريد — مفيد للتفويض
    public string? GetUserRoleByEmail(string email)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = "SELECT Role FROM Users WHERE Email = @Email LIMIT 1";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Email", email);

        var role = cmd.ExecuteScalar();
        return role?.ToString();
    }
}
