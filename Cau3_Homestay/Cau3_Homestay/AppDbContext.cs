namespace Cau3_Homestay;

// Tương đương DbContext do EF Core Power Tools (Database First) sinh ra
public class AppDbContext : DbContext
{
    // Đổi chuỗi kết nối tại đây nếu dùng SQL Server khác, ví dụ: Server=.\SQLEXPRESS;...
    public const string ConnectionString =
        @"Server=(localdb)\MSSQLLocalDB;Database=SunriseHomestayDB;Trusted_Connection=True;TrustServerCertificate=True";

    public DbSet<LoaiPhong> LoaiPhongs { get; set; }
    public DbSet<Phong> Phongs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlServer(ConnectionString);

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<LoaiPhong>(e =>
        {
            e.ToTable("LoaiPhong"); e.HasKey(x => x.MaLoai);
            e.Property(x => x.TenLoai).HasMaxLength(100).IsRequired();
            e.Property(x => x.GiaMoiDem).HasColumnType("decimal(18, 2)");
            e.Property(x => x.MoTa).HasMaxLength(255);
        });
        mb.Entity<Phong>(e =>
        {
            e.ToTable("Phong"); e.HasKey(x => x.MaPhong);
            e.Property(x => x.SoPhong).HasMaxLength(10).IsUnicode(false).IsRequired();
            e.Property(x => x.TinhTrang).HasMaxLength(20);
            e.Property(x => x.HinhAnh).HasMaxLength(255);
            e.HasOne(x => x.MaLoaiNavigation).WithMany(x => x.Phongs).HasForeignKey(x => x.MaLoai);
        });
    }
}
