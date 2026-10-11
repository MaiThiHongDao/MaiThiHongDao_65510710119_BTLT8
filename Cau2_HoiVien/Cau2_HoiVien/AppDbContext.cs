namespace Cau2_HoiVien;

// Tương đương DbContext do EF Core Power Tools (Database First) sinh ra
public class AppDbContext : DbContext
{
    // Đổi chuỗi kết nối tại đây nếu dùng SQL Server khác, ví dụ: Server=.\SQLEXPRESS;...
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=FitZoneDB;Trusted_Connection=True;TrustServerCertificate=True";

    public DbSet<HoiVien> HoiViens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlServer(ConnectionString);

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<HoiVien>(e =>
        {
            e.ToTable("HoiVien"); e.HasKey(x => x.MaHV);
            e.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            e.Property(x => x.NgaySinh).HasColumnType("date");
            e.Property(x => x.SDT).HasMaxLength(15).IsUnicode(false);
            e.Property(x => x.Email).HasMaxLength(100).IsUnicode(false);
            e.Property(x => x.HangThanhVien).HasMaxLength(20);
            e.Property(x => x.NgayDangKy).HasColumnType("datetime").HasDefaultValueSql("GETDATE()");
        });
    }
}
