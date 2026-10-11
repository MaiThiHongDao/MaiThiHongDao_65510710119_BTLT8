namespace Cau3_Homestay;

// ===== CÂU 3 (Form phụ): Quản lý Loại phòng =====
public class LoaiPhongForm : Form
{
    TextBox txtMa, txtTen, txtMoTa;
    NumericUpDown nudGia;
    DataGridView dgv;

    public LoaiPhongForm()
    {
        Text = "Quản Lý Loại Phòng - Sunrise Homestay";
        ClientSize = new Size(720, 460); StartPosition = FormStartPosition.CenterParent;
        UI.Lbl(this, "Mã loại", 15, 15); txtMa = UI.Txt(this, 110, 15, 200, true);
        UI.Lbl(this, "Tên loại", 15, 50); txtTen = UI.Txt(this, 110, 50, 200);
        UI.Lbl(this, "Giá mỗi đêm", 15, 85);
        nudGia = new NumericUpDown { Left = 110, Top = 85, Width = 200, Maximum = 1000000000, Increment = 10000, ThousandsSeparator = true };
        Controls.Add(nudGia);
        UI.Lbl(this, "Mô tả", 15, 120); txtMoTa = UI.Txt(this, 110, 120, 200);
        var bThem = UI.Btn(this, "Thêm", 400, 15); var bSua = UI.Btn(this, "Sửa", 500, 15);
        var bXoa = UI.Btn(this, "Xóa", 600, 15); var bMoi = UI.Btn(this, "Làm mới", 400, 50);

        dgv = UI.Grid(this, 15, 170, 690, 275);
        UI.Col(dgv, "Mã loại", "MaLoai", 70); UI.Col(dgv, "Tên loại", "TenLoai", 180);
        UI.Col(dgv, "Giá/đêm", "GiaMoiDem", 120, "N0"); UI.Col(dgv, "Mô tả", "MoTa", 250);

        Load += async (_, _) => await LoadData();
        dgv.SelectionChanged += (_, _) => Fill();
        bMoi.Click += async (_, _) => { Clear(); await LoadData(); };
        bThem.Click += async (_, _) => await Save(true);
        bSua.Click += async (_, _) => await Save(false);
        bXoa.Click += async (_, _) => await Xoa();
    }

    async Task LoadData()
    {
        using var db = new AppDbContext();
        dgv.DataSource = await db.LoaiPhongs.AsNoTracking().Select(x => new LoaiPhong
        { MaLoai = x.MaLoai, TenLoai = x.TenLoai, GiaMoiDem = x.GiaMoiDem, MoTa = x.MoTa }).OrderBy(x => x.MaLoai).ToListAsync();
        Fill();
    }

    void Fill()
    {
        if (dgv.CurrentRow?.DataBoundItem is LoaiPhong l)
        { txtMa.Text = l.MaLoai.ToString(); txtTen.Text = l.TenLoai; nudGia.Value = l.GiaMoiDem ?? 0; txtMoTa.Text = l.MoTa; }
        else Clear();
    }
    void Clear() { txtMa.Clear(); txtTen.Clear(); nudGia.Value = 0; txtMoTa.Clear(); }

    async Task Save(bool isNew)
    {
        if (txtTen.Text.Trim() == "") { UI.Warn("Tên loại phòng không được để trống!"); txtTen.Focus(); return; }
        using var db = new AppDbContext();
        LoaiPhong l;
        if (isNew) { l = new LoaiPhong(); db.Add(l); }
        else
        {
            if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn loại phòng cần sửa!"); return; }
            l = await db.LoaiPhongs.FindAsync(ma);
            if (l == null) return;
        }
        l.TenLoai = txtTen.Text.Trim(); l.GiaMoiDem = nudGia.Value; l.MoTa = txtMoTa.Text.Trim();
        await db.SaveChangesAsync();
        UI.Info(isNew ? "Thêm loại phòng thành công!" : "Cập nhật thành công!");
        if (isNew) Clear();
        await LoadData();
    }

    async Task Xoa()
    {
        if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn loại phòng cần xóa!"); return; }
        if (!UI.Ask($"Xóa loại phòng \"{txtTen.Text}\"?")) return;
        try
        {
            using var db = new AppDbContext();
            var l = await db.LoaiPhongs.FindAsync(ma);
            if (l != null) { db.Remove(l); await db.SaveChangesAsync(); }
            UI.Info("Đã xóa loại phòng!"); Clear(); await LoadData();
        }
        catch (DbUpdateException) { UI.Error("Không thể xóa: loại phòng này đang có phòng sử dụng!"); }
    }
}
