namespace Cau4_PhongKham;

public class BacSi
{
    public int MaBS { get; set; }
    public string HoTen { get; set; }
    public string ChuyenKhoa { get; set; }
    public string SDT { get; set; }
    public virtual ICollection<LichKham> LichKhams { get; set; } = new List<LichKham>();
}

public class LichKham
{
    public int MaLich { get; set; }
    public string TenBenhNhan { get; set; }
    public string SDT { get; set; }
    public DateTime? NgayKham { get; set; }
    public TimeSpan? GioKham { get; set; }
    public int? MaBS { get; set; }
    public string TrangThai { get; set; }
    public virtual BacSi MaBSNavigation { get; set; }
}
