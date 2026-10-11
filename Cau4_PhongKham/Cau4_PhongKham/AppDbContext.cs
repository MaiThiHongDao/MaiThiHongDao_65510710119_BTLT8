namespace Cau4_PhongKham;

// Tương đương DbContext do EF Core Power Tools (Database First) sinh ra
public class AppDbContext : DbContext
{
    // Đổi chuỗi kết nối tại đây nếu dùng SQL Server khác, ví dụ: Server=.\SQLEXPRESS;...
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=AnKhangClinicDB;Trusted_Connection=True;TrustServerCertificate=True";

    public DbSet<BacSi> BacSis { get; set; }
    public DbSet<LichKham> LichKhams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlServer(ConnectionString);

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<BacSi>(e =>
        {
            e.ToTable("BacSi"); e.HasKey(x => x.MaBS);
            e.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            e.Property(x => x.ChuyenKhoa).HasMaxLength(100);
            e.Property(x => x.SDT).HasMaxLength(15).IsUnicode(false);
        });
        mb.Entity<LichKham>(e =>
        {
            e.ToTable("LichKham"); e.HasKey(x => x.MaLich);
            e.Property(x => x.TenBenhNhan).HasMaxLength(100).IsRequired();
            e.Property(x => x.SDT).HasMaxLength(15).IsUnicode(false);
            e.Property(x => x.NgayKham).HasColumnType("date");
            e.Property(x => x.GioKham).HasColumnType("time(0)");
            e.Property(x => x.TrangThai).HasMaxLength(20);
            e.HasOne(x => x.MaBSNavigation).WithMany(x => x.LichKhams).HasForeignKey(x => x.MaBS);
        });
    }
}
