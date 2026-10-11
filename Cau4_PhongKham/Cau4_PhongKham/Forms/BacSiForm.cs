namespace Cau4_PhongKham;

// ===== CÂU 4 (Form phụ): Quản lý Bác sĩ =====
public class BacSiForm : Form
{
    TextBox txtMa, txtTen, txtCK, txtSDT;
    DataGridView dgv;

    public BacSiForm()
    {
        Text = "Quản Lý Bác Sĩ - An Khang Clinic";
        ClientSize = new Size(700, 440); StartPosition = FormStartPosition.CenterParent;
        UI.Lbl(this, "Mã bác sĩ", 15, 15); txtMa = UI.Txt(this, 110, 15, 200, true);
        UI.Lbl(this, "Họ tên", 15, 50); txtTen = UI.Txt(this, 110, 50, 200);
        UI.Lbl(this, "Chuyên khoa", 15, 85); txtCK = UI.Txt(this, 110, 85, 200);
        UI.Lbl(this, "SĐT", 15, 120); txtSDT = UI.Txt(this, 110, 120, 200);
        var bThem = UI.Btn(this, "Thêm", 400, 15); var bSua = UI.Btn(this, "Sửa", 500, 15);
        var bXoa = UI.Btn(this, "Xóa", 600, 15); var bMoi = UI.Btn(this, "Làm mới", 400, 50);

        dgv = UI.Grid(this, 15, 165, 670, 260);
        UI.Col(dgv, "Mã BS", "MaBS", 70); UI.Col(dgv, "Họ tên", "HoTen", 200);
        UI.Col(dgv, "Chuyên khoa", "ChuyenKhoa", 200); UI.Col(dgv, "SĐT", "SDT", 130);

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
        dgv.DataSource = await db.BacSis.AsNoTracking().Select(x => new BacSi
        { MaBS = x.MaBS, HoTen = x.HoTen, ChuyenKhoa = x.ChuyenKhoa, SDT = x.SDT }).OrderBy(x => x.MaBS).ToListAsync();
        Fill();
    }
    void Fill()
    {
        if (dgv.CurrentRow?.DataBoundItem is BacSi b) { txtMa.Text = b.MaBS.ToString(); txtTen.Text = b.HoTen; txtCK.Text = b.ChuyenKhoa; txtSDT.Text = b.SDT; }
        else Clear();
    }
    void Clear() { txtMa.Clear(); txtTen.Clear(); txtCK.Clear(); txtSDT.Clear(); }

    async Task Save(bool isNew)
    {
        if (txtTen.Text.Trim() == "") { UI.Warn("Họ tên bác sĩ không được để trống!"); txtTen.Focus(); return; }
        if (txtSDT.Text.Trim() != "" && !Regex.IsMatch(txtSDT.Text.Trim(), @"^\d{9,11}$")) { UI.Warn("SĐT chỉ gồm 9-11 chữ số!"); txtSDT.Focus(); return; }
        using var db = new AppDbContext();
        BacSi b;
        if (isNew) { b = new BacSi(); db.Add(b); }
        else
        {
            if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn bác sĩ cần sửa!"); return; }
            b = await db.BacSis.FindAsync(ma); if (b == null) return;
        }
        b.HoTen = txtTen.Text.Trim(); b.ChuyenKhoa = txtCK.Text.Trim(); b.SDT = txtSDT.Text.Trim();
        await db.SaveChangesAsync();
        UI.Info(isNew ? "Thêm bác sĩ thành công!" : "Cập nhật thành công!");
        if (isNew) Clear();
        await LoadData();
    }

    async Task Xoa()
    {
        if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn bác sĩ cần xóa!"); return; }
        if (!UI.Ask($"Xóa bác sĩ \"{txtTen.Text}\"?")) return;
        try
        {
            using var db = new AppDbContext();
            var b = await db.BacSis.FindAsync(ma);
            if (b != null) { db.Remove(b); await db.SaveChangesAsync(); }
            UI.Info("Đã xóa bác sĩ!"); Clear(); await LoadData();
        }
        catch (DbUpdateException) { UI.Error("Không thể xóa: bác sĩ này đang có lịch khám!"); }
    }
}
