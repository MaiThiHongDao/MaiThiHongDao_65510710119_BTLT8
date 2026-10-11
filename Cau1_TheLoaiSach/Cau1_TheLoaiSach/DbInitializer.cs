using Microsoft.Data.SqlClient;
namespace Cau1_TheLoaiSach;

/// <summary>Tự tạo database + bảng + dữ liệu mẫu (chạy Database.sql) nếu database chưa tồn tại.</summary>
public static class DbInitializer
{
    public static void Run()
    {
        var csb = new SqlConnectionStringBuilder(AppDbContext.ConnectionString);
        string dbName = csb.InitialCatalog;
        csb.InitialCatalog = "master";
        using var cn = new SqlConnection(csb.ConnectionString);
        cn.Open();
        using (var check = new SqlCommand("SELECT DB_ID(@n)", cn))
        {
            check.Parameters.AddWithValue("@n", dbName);
            if (check.ExecuteScalar() != DBNull.Value) return; // đã có database
        }
        string script = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Database.sql"));
        foreach (var batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(batch)) continue;
            using var cmd = new SqlCommand(batch, cn);
            cmd.ExecuteNonQuery();
        }
    }
}
