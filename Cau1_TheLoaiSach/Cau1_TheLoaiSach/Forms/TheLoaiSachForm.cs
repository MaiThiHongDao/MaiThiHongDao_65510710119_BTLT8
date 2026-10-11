namespace Cau1_TheLoaiSach;

// ===== CÂU 1: Quản lý Thể loại sách =====
public class TheLoaiSachForm : Form
{
    TextBox txtMa, txtTen, txtMoTa, txtTim;
    Label lblNgayTao;
    Button btnThem, btnSua, btnXoa, btnMoi, btnTim;
    DataGridView dgv;

    public TheLoaiSachForm()
    {
        Text = "Quản Lý Thể Loại Sách - Tri Thức Books";
        ClientSize = new Size(880, 560); StartPosition = FormStartPosition.CenterParent;

        UI.Lbl(this, "Mã thể loại", 15, 15); txtMa = UI.Txt(this, 110, 15, 250, true);
        UI.Lbl(this, "Tên thể loại", 15, 50); txtTen = UI.Txt(this, 110, 50, 250);
        UI.Lbl(this, "Mô tả", 15, 85);
        txtMoTa = UI.Txt(this, 110, 85, 250); txtMoTa.Multiline = true; txtMoTa.Height = 80; txtMoTa.ScrollBars = ScrollBars.Vertical;
        UI.Lbl(this, "Ngày tạo", 15, 180); lblNgayTao = UI.Lbl(this, "", 110, 180);

        btnThem = UI.Btn(this, "Thêm", 480, 15); btnSua = UI.Btn(this, "Sửa", 580, 15);
        btnXoa = UI.Btn(this, "Xóa", 680, 15); btnMoi = UI.Btn(this, "Làm mới", 780, 15);

        txtTim = UI.Txt(this, 15, 215, 345); btnTim = UI.Btn(this, "Tìm kiếm", 370, 213);

        dgv = UI.Grid(this, 15, 255, 850, 290);
        UI.Col(dgv, "Mã TL", "MaTL", 60); UI.Col(dgv, "Tên thể loại", "TenTheLoai", 180);
        UI.Col(dgv, "Mô tả", "MoTa", 320); UI.Col(dgv, "Số lượng sách", "SoLuongSach", 100);
        UI.Col(dgv, "Ngày tạo", "NgayTao", 150, "dd/MM/yyyy HH:mm:ss");

        Load += async (_, _) => await LoadData();
        dgv.SelectionChanged += (_, _) => FillInputs();
        btnTim.Click += async (_, _) => await LoadData(txtTim.Text.Trim());
        txtTim.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) await LoadData(txtTim.Text.Trim()); };
        btnMoi.Click += async (_, _) => { ClearInputs(); txtTim.Clear(); await LoadData(); };
        btnThem.Click += async (_, _) => await Them();
        btnSua.Click += async (_, _) => await Sua();
        btnXoa.Click += async (_, _) => await Xoa();
    }

    async Task LoadData(string keyword = "")
    {
        using var db = new AppDbContext();
        var q = db.TheLoaiSaches.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(keyword)) q = q.Where(x => x.TenTheLoai.Contains(keyword)); // LINQ Where + Contains
        dgv.DataSource = await q.OrderBy(x => x.MaTL).ToListAsync();
        FillInputs();
    }

    void FillInputs()
    {
        if (dgv.CurrentRow?.DataBoundItem is TheLoaiSach t)
        {
            txtMa.Text = t.MaTL.ToString(); txtTen.Text = t.TenTheLoai; txtMoTa.Text = t.MoTa;
            lblNgayTao.Text = t.NgayTao?.ToString("dd/MM/yyyy HH:mm:ss");
        }
        else ClearInputs();
    }

    void ClearInputs() { txtMa.Clear(); txtTen.Clear(); txtMoTa.Clear(); lblNgayTao.Text = ""; txtTen.Focus(); }

    bool Validate(out string ten)
    {
        ten = txtTen.Text.Trim();
        if (ten == "") { UI.Warn("Tên thể loại không được để trống!"); txtTen.Focus(); return false; }
        return true;
    }

    async Task Them()
    {
        if (!Validate(out var ten)) return;
        using var db = new AppDbContext();
        if (await db.TheLoaiSaches.AnyAsync(x => x.TenTheLoai == ten))
        { UI.Warn($"Thể loại \"{ten}\" đã tồn tại!"); txtTen.Focus(); return; }
        db.Add(new TheLoaiSach { TenTheLoai = ten, MoTa = txtMoTa.Text.Trim(), SoLuongSach = 0, NgayTao = DateTime.Now });
        await db.SaveChangesAsync();
        UI.Info("Thêm thể loại thành công!");
        ClearInputs(); await LoadData(txtTim.Text.Trim());
    }

    async Task Sua()
    {
        if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn một dòng cần sửa!"); return; }
        if (!Validate(out var ten)) return;
        using var db = new AppDbContext();
        var tl = await db.TheLoaiSaches.FindAsync(ma);
        if (tl == null) { UI.Error("Thể loại không còn tồn tại!"); await LoadData(); return; }
        if (await db.TheLoaiSaches.AnyAsync(x => x.TenTheLoai == ten && x.MaTL != ma))
        { UI.Warn($"Tên \"{ten}\" đã được thể loại khác sử dụng!"); return; }
        tl.TenTheLoai = ten; tl.MoTa = txtMoTa.Text.Trim();
        await db.SaveChangesAsync();
        UI.Info("Cập nhật thành công!");
        await LoadData(txtTim.Text.Trim());
    }

    async Task Xoa()
    {
        if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn một dòng cần xóa!"); return; }
        if (!UI.Ask($"Bạn có chắc muốn xóa thể loại \"{txtTen.Text}\" không?")) return;
        try
        {
            using var db = new AppDbContext();
            var tl = await db.TheLoaiSaches.FindAsync(ma);
            if (tl == null) { await LoadData(); return; }
            db.Remove(tl);
            await db.SaveChangesAsync();
            UI.Info("Đã xóa thể loại!");
            ClearInputs(); await LoadData(txtTim.Text.Trim());
        }
        catch (DbUpdateException)
        {
            UI.Error("Không thể xóa: thể loại này đang được các đầu sách tham chiếu!");
        }
    }
}
