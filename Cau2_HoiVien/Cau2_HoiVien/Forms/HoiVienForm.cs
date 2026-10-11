namespace Cau2_HoiVien;

// ===== CÂU 2: Quản lý Hội viên Gym =====
public class HoiVienForm : Form
{
    TextBox txtMa, txtHoTen, txtSDT, txtEmail, txtTim;
    RadioButton rdoNam, rdoNu;
    DateTimePicker dtpNgaySinh;
    ComboBox cboHang, cboLoc;
    CheckBox chkHoatDong;
    Button btnThem, btnSua, btnXoa, btnMoi, btnTim;
    DataGridView dgv;
    static readonly string[] Hangs = { "Basic", "VIP", "Premium" };

    public HoiVienForm()
    {
        Text = "Quản Lý Hội Viên Phòng Gym FitZone";
        ClientSize = new Size(880, 570); StartPosition = FormStartPosition.CenterParent;

        txtMa = new TextBox { Visible = false }; // lưu mã HV
        UI.Lbl(this, "Họ tên", 15, 15); txtHoTen = UI.Txt(this, 120, 15, 220);
        UI.Lbl(this, "Số điện thoại", 15, 50); txtSDT = UI.Txt(this, 120, 50, 220);
        UI.Lbl(this, "Email", 15, 85); txtEmail = UI.Txt(this, 120, 85, 220);
        UI.Lbl(this, "Ngày sinh", 15, 120);
        dtpNgaySinh = new DateTimePicker { Left = 120, Top = 120, Width = 220, Format = DateTimePickerFormat.Short, MaxDate = DateTime.Today, Value = DateTime.Today.AddYears(-20) };
        Controls.Add(dtpNgaySinh);
        UI.Lbl(this, "Hạng thành viên", 15, 155); cboHang = UI.Cbo(this, 120, 155, 220, Hangs); cboHang.SelectedIndex = 0;
        chkHoatDong = new CheckBox { Text = "Đang hoạt động", Left = 120, Top = 190, AutoSize = true, Checked = true };
        Controls.Add(chkHoatDong);

        var grp = new GroupBox { Text = "Giới tính", Left = 380, Top = 10, Width = 250, Height = 70 };
        rdoNam = new RadioButton { Text = "Nam", Left = 25, Top = 28, AutoSize = true, Checked = true };
        rdoNu = new RadioButton { Text = "Nữ", Left = 130, Top = 28, AutoSize = true };
        grp.Controls.AddRange(new Control[] { rdoNam, rdoNu }); Controls.Add(grp);

        btnThem = UI.Btn(this, "Thêm", 700, 15, 150); btnSua = UI.Btn(this, "Sửa", 700, 50, 150);
        btnXoa = UI.Btn(this, "Xóa", 700, 85, 150); btnMoi = UI.Btn(this, "Làm mới", 700, 120, 150);

        txtTim = UI.Txt(this, 15, 232, 250);
        UI.Lbl(this, "Hạng thành viên", 290, 232);
        cboLoc = UI.Cbo(this, 400, 232, 130, "Tất cả", "Basic", "VIP", "Premium"); cboLoc.SelectedIndex = 0;
        btnTim = UI.Btn(this, "Tìm kiếm", 550, 230);

        dgv = UI.Grid(this, 15, 270, 850, 285);
        UI.Col(dgv, "Mã HV", "MaHV", 55); UI.Col(dgv, "Họ tên", "HoTen", 150);
        UI.Col(dgv, "Giới tính", "GioiTinh", 70); UI.Col(dgv, "Ngày sinh", "NgaySinh", 90, "dd/MM/yyyy");
        UI.Col(dgv, "SĐT", "SDT", 110); UI.Col(dgv, "Email", "Email", 150);
        UI.Col(dgv, "Hạng thành viên", "HangThanhVien", 110); UI.Col(dgv, "Trạng thái", "TrangThai", 100);

        dgv.CellFormatting += (_, e) =>
        {
            string p = dgv.Columns[e.ColumnIndex].DataPropertyName;
            if (p == "GioiTinh") { e.Value = e.Value is bool b ? (b ? "Nam" : "Nữ") : ""; e.FormattingApplied = true; }
            if (p == "TrangThai") { e.Value = e.Value is bool t ? (t ? "Đang hoạt động" : "Tạm ngưng") : ""; e.FormattingApplied = true; }
        };

        Load += async (_, _) => await LoadData();
        dgv.SelectionChanged += (_, _) => FillInputs();
        btnTim.Click += async (_, _) => await LoadData();
        btnMoi.Click += async (_, _) => { ClearInputs(); txtTim.Clear(); cboLoc.SelectedIndex = 0; await LoadData(); };
        btnThem.Click += async (_, _) => await Them();
        btnSua.Click += async (_, _) => await Sua();
        btnXoa.Click += async (_, _) => await Xoa();
    }

    async Task LoadData()
    {
        string ten = txtTim.Text.Trim();
        string hang = cboLoc.SelectedIndex > 0 ? cboLoc.SelectedItem.ToString() : null;
        using var db = new AppDbContext();
        // LINQ Where nhiều điều kiện nối bằng &&
        dgv.DataSource = await db.HoiViens.AsNoTracking()
            .Where(x => (ten == "" || x.HoTen.Contains(ten)) && (hang == null || x.HangThanhVien == hang))
            .OrderBy(x => x.MaHV).ToListAsync();
        FillInputs();
    }

    void FillInputs()
    {
        if (dgv.CurrentRow?.DataBoundItem is not HoiVien h) { ClearInputs(); return; }
        txtMa.Text = h.MaHV.ToString(); txtHoTen.Text = h.HoTen; txtSDT.Text = h.SDT; txtEmail.Text = h.Email;
        rdoNam.Checked = h.GioiTinh != false; rdoNu.Checked = h.GioiTinh == false;
        var ns = h.NgaySinh ?? DateTime.Today.AddYears(-20);
        dtpNgaySinh.Value = ns > dtpNgaySinh.MaxDate ? dtpNgaySinh.MaxDate : (ns < dtpNgaySinh.MinDate ? dtpNgaySinh.MinDate : ns);
        int idx = Array.IndexOf(Hangs, h.HangThanhVien); cboHang.SelectedIndex = idx < 0 ? 0 : idx;
        chkHoatDong.Checked = h.TrangThai == true;
    }

    void ClearInputs()
    {
        txtMa.Clear(); txtHoTen.Clear(); txtSDT.Clear(); txtEmail.Clear();
        rdoNam.Checked = true; dtpNgaySinh.Value = DateTime.Today.AddYears(-20);
        cboHang.SelectedIndex = 0; chkHoatDong.Checked = true; txtHoTen.Focus();
    }

    // Mỗi trường báo lỗi riêng
    bool Validate()
    {
        if (txtHoTen.Text.Trim() == "") { UI.Warn("Họ tên không được để trống!"); txtHoTen.Focus(); return false; }
        string sdt = txtSDT.Text.Trim();
        if (!Regex.IsMatch(sdt, @"^\d{9,11}$"))
        { UI.Warn("Số điện thoại chỉ được chứa chữ số và phải có từ 9 đến 11 ký tự!"); txtSDT.Focus(); return false; }
        if (!txtEmail.Text.Contains('@')) { UI.Warn("Email phải chứa ký tự '@'!"); txtEmail.Focus(); return false; }
        var dob = dtpNgaySinh.Value.Date; var today = DateTime.Today;
        int tuoi = today.Year - dob.Year; if (dob > today.AddYears(-tuoi)) tuoi--;
        if (tuoi < 15) { UI.Warn($"Hội viên phải từ 15 tuổi trở lên (hiện {tuoi} tuổi)!"); dtpNgaySinh.Focus(); return false; }
        return true;
    }

    void ToEntity(HoiVien h)
    {
        h.HoTen = txtHoTen.Text.Trim(); h.GioiTinh = rdoNam.Checked; h.NgaySinh = dtpNgaySinh.Value.Date;
        h.SDT = txtSDT.Text.Trim(); h.Email = txtEmail.Text.Trim();
        h.HangThanhVien = cboHang.SelectedItem.ToString(); h.TrangThai = chkHoatDong.Checked;
    }

    async Task Them()
    {
        if (!Validate()) return;
        using var db = new AppDbContext();
        var h = new HoiVien { NgayDangKy = DateTime.Now }; ToEntity(h);
        db.Add(h); await db.SaveChangesAsync();
        UI.Info("Thêm hội viên thành công!"); ClearInputs(); await LoadData();
    }

    async Task Sua()
    {
        if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn hội viên cần sửa!"); return; }
        if (!Validate()) return;
        using var db = new AppDbContext();
        var h = await db.HoiViens.FindAsync(ma);
        if (h == null) { UI.Error("Hội viên không còn tồn tại!"); await LoadData(); return; }
        ToEntity(h); await db.SaveChangesAsync();
        UI.Info("Cập nhật hội viên thành công!"); await LoadData();
    }

    async Task Xoa()
    {
        if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn hội viên cần xóa!"); return; }
        if (!UI.Ask($"Bạn có chắc muốn xóa hội viên \"{txtHoTen.Text}\"?")) return;
        try
        {
            using var db = new AppDbContext();
            var h = await db.HoiViens.FindAsync(ma);
            if (h != null) { db.Remove(h); await db.SaveChangesAsync(); }
            UI.Info("Đã xóa hội viên!"); ClearInputs(); await LoadData();
        }
        catch (DbUpdateException ex) { UI.Error("Không thể xóa hội viên: " + ex.GetBaseException().Message); }
    }
}
