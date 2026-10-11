namespace Cau3_Homestay;

public class LoaiPhong
{
    public int MaLoai { get; set; }
    public string TenLoai { get; set; }
    public decimal? GiaMoiDem { get; set; }
    public string MoTa { get; set; }
    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}

public class Phong
{
    public int MaPhong { get; set; }
    public string SoPhong { get; set; }
    public int? TangSo { get; set; }
    public string TinhTrang { get; set; }
    public string HinhAnh { get; set; }
    public int? MaLoai { get; set; }
    public virtual LoaiPhong MaLoaiNavigation { get; set; }
}
