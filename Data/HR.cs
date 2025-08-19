using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

public class HrEmployeeRepository
{
    public void CreateTable()
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = @"CREATE TABLE IF NOT EXISTS HrEmployees (
                           Id TEXT PRIMARY KEY, 
                           Name TEXT NOT NULL,
                           Email TEXT NOT NULL UNIQUE,
                           Phone TEXT NOT NULL,
                           PasswordHash TEXT NOT NULL,
                           CreatedAt TEXT NOT NULL
                        );";
        using var cmd = new SqliteCommand(query, connection);
        cmd.ExecuteNonQuery();
    }

    // ➕ إضافة موظف HR جديد
    public bool AddHrEmployee(string id, string name, string email, string phone, string passwordHash, DateTime createdAt)
    {
        try
        {
            using var connection = Database.GetConnection();
            connection.Open();

            string query = @"INSERT INTO HrEmployees (Id, Name, Email, Phone, PasswordHash, CreatedAt) 
                             VALUES (@Id, @Name, @Email, @Phone, @PasswordHash, @CreatedAt)";

            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Phone", phone);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
            return true;
        }
        catch (SqliteException ex)
        {
            if (ex.SqliteErrorCode == 19) // UNIQUE constraint failed (إيميل مكرر)
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

    // ✏️ تحديث بيانات موظف HR
    public bool UpdateHrEmployee(string id, string? name = null, string? email = null, string? phone = null, string? passwordHash = null)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        var setParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(name)) setParts.Add("Name = @Name");
        if (!string.IsNullOrWhiteSpace(email)) setParts.Add("Email = @Email");
        if (!string.IsNullOrWhiteSpace(phone)) setParts.Add("Phone = @Phone");
        if (!string.IsNullOrWhiteSpace(passwordHash)) setParts.Add("PasswordHash = @PasswordHash");

        if (setParts.Count == 0) return false; // لا يوجد شيء للتعديل

        string query = $"UPDATE HrEmployees SET {string.Join(", ", setParts)} WHERE Id = @Id";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", id);
        if (!string.IsNullOrWhiteSpace(name)) cmd.Parameters.AddWithValue("@Name", name);
        if (!string.IsNullOrWhiteSpace(email)) cmd.Parameters.AddWithValue("@Email", email);
        if (!string.IsNullOrWhiteSpace(phone)) cmd.Parameters.AddWithValue("@Phone", phone);
        if (!string.IsNullOrWhiteSpace(passwordHash)) cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

        int affected = cmd.ExecuteNonQuery();
        return affected > 0;
    }

    // ❌ حذف موظف HR
    public bool DeleteHrEmployee(string id)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = "DELETE FROM HrEmployees WHERE Id = @Id";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", id);

        int affected = cmd.ExecuteNonQuery();
        return affected > 0;
    }

    // ✅ هل الموظف موجود عبر Id؟
    public bool HrEmployeeExistsById(string id)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = "SELECT 1 FROM HrEmployees WHERE Id = @Id LIMIT 1";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Id", id);

        var result = cmd.ExecuteScalar();
        return result != null;
    }

    // ✅ هل البريد موجود مسبقًا؟
    public bool HrEmployeeExistsByEmail(string email)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = "SELECT 1 FROM HrEmployees WHERE Email = @Email LIMIT 1";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Email", email);

        var result = cmd.ExecuteScalar();
        return result != null;
    }

    // 🔐 التحقق من بيانات الدخول
    public bool VerifyCredentials(string email, string passwordHash)
    {
        using var connection = Database.GetConnection();
        connection.Open();

        string query = @"SELECT 1 FROM HrEmployees 
                         WHERE Email = @Email AND PasswordHash = @PasswordHash
                         LIMIT 1";
        using var cmd = new SqliteCommand(query, connection);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

        var result = cmd.ExecuteScalar();
        return result != null;
    }
}
