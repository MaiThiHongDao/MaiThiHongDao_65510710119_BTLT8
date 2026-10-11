namespace Cau1_TheLoaiSach;

// Tương đương DbContext do EF Core Power Tools (Database First) sinh ra
public class AppDbContext : DbContext
{
    // Đổi chuỗi kết nối tại đây nếu dùng SQL Server khác, ví dụ: Server=.\SQLEXPRESS;...
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=TriThucBooksDB;Trusted_Connection=True;TrustServerCertificate=True";

    public DbSet<TheLoaiSach> TheLoaiSaches { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlServer(ConnectionString);

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<TheLoaiSach>(e =>
        {
            e.ToTable("TheLoaiSach"); e.HasKey(x => x.MaTL);
            e.Property(x => x.TenTheLoai).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.TenTheLoai).IsUnique();
            e.Property(x => x.MoTa).HasMaxLength(255);
            e.Property(x => x.SoLuongSach).HasDefaultValue(0);
            e.Property(x => x.NgayTao).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
        });
    }
}
