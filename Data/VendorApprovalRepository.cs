using Microsoft.Data.Sqlite;
using System;

public class VendorApprovalRepository
{
    private readonly UserRepository _userRepo;

    public VendorApprovalRepository(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    // إنشاء جدول VendorsApproval (بدون OwnerUserId و Description)
    public void CreateTable()
    {
        using var conn = Database.GetConnection();
        conn.Open();

        string sql = @"CREATE TABLE IF NOT EXISTS VendorsApproval (
                          Id TEXT PRIMARY KEY,
                          Name TEXT NOT NULL,
                          CreatedAt TEXT NOT NULL,
                          Status TEXT CHECK(Status IN ('wait','Accepted','Rejected')) NOT NULL DEFAULT 'wait',
                          Email TEXT NOT NULL UNIQUE,
                          PasswordHash TEXT NOT NULL
                       );";

        using var cmd = new SqliteCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    // ➕ إضافة Vendor جديد (الـ Status دايمًا يبدأ 'wait')
    public bool AddVendor(string id, string name, string email, string passwordHash, DateTime createdAt)
    {
        try
        {
            using var conn = Database.GetConnection();
            conn.Open();

            string sql = @"INSERT INTO VendorsApproval 
                            (Id, Name, CreatedAt, Status, Email, PasswordHash) 
                           VALUES 
                            (@Id, @Name, @CreatedAt, 'wait', @Email, @PasswordHash)";
            
            using var cmd = new SqliteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@CreatedAt", createdAt.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

            return cmd.ExecuteNonQuery() > 0;
        }
        catch (SqliteException ex)
        {
            if (ex.SqliteErrorCode == 19) // UNIQUE constraint failed
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

    // ✅ Admin يقبل Vendor → يضاف لجدول Users ويحذف من VendorsApproval
    public bool AcceptVendor(string vendorId)
{
    using var conn = Database.GetConnection();
    conn.Open();

    string sql = "SELECT * FROM VendorsApproval WHERE Id=@Id LIMIT 1";
    using var cmd = new SqliteCommand(sql, conn);
    cmd.Parameters.AddWithValue("@Id", vendorId);
    using var reader = cmd.ExecuteReader();

    if (reader.Read())
    {
        string id = reader.GetString(reader.GetOrdinal("Id"));
        string email = reader.GetString(reader.GetOrdinal("Email"));
        string passwordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
        string name = reader.GetString(reader.GetOrdinal("Name"));
        string createdAt = reader.GetString(reader.GetOrdinal("CreatedAt"));

        // ⚠️ سكّر الـ reader قبل أي Query جديدة
        reader.Close();

        // أضف لجدول Users كـ Vendor
        bool added = _userRepo.AddUser(
            id,
            email,
            passwordHash,
            "Vendor",
            name,
            DateTime.Parse(createdAt)
        );

        if (!added)
        {
            Console.WriteLine("❌ Failed to insert user (maybe duplicate email/id).");
            return false;
        }

        // احذف من جدول VendorsApproval
        string deleteSql = "DELETE FROM VendorsApproval WHERE Id=@Id";
        using var deleteCmd = new SqliteCommand(deleteSql, conn);
        deleteCmd.Parameters.AddWithValue("@Id", vendorId);
        deleteCmd.ExecuteNonQuery();

        return true;
    }

    return false;
}


    // ❌ Admin يرفض Vendor → يحذف من جدول VendorsApproval فقط
    public bool RejectVendor(string vendorId)
    {
        using var conn = Database.GetConnection();
        conn.Open();

        string sql = "DELETE FROM VendorsApproval WHERE Id=@Id";
        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", vendorId);

        return cmd.ExecuteNonQuery() > 0;
    }
    public List<VendorApprovalDto> GetAllVendors()
{
    var vendors = new List<VendorApprovalDto>();

    using var conn = Database.GetConnection();
    conn.Open();

    string sql = "SELECT Id, Name, CreatedAt, Status, Email, PasswordHash FROM VendorsApproval";
    using var cmd = new SqliteCommand(sql, conn);
    using var reader = cmd.ExecuteReader();

    while (reader.Read())
    {
        vendors.Add(new VendorApprovalDto
        {
            Id = reader.GetString(0),
            Name = reader.GetString(1),
            CreatedAt = DateTime.Parse(reader.GetString(2)),
            Status = reader.GetString(3),
            Email = reader.GetString(4),
            PasswordHash = reader.GetString(5)
        });
    }

    return vendors;
}

}
