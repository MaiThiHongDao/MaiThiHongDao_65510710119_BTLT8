namespace Cau4_PhongKham;

// ===== CÂU 4 (Form chính): Quản lý Lịch khám =====
public class LichRow
{
    public int MaLich { get; set; }
    public string TenBenhNhan { get; set; }
    public string SDT { get; set; }
    public DateTime? NgayKham { get; set; }
    public string GioKham { get; set; }
    public TimeSpan? Gio { get; set; }
    public string TenBS { get; set; }
    public string ChuyenKhoa { get; set; }
    public string TrangThai { get; set; }
    public int? MaBS { get; set; }
}

public class BsItem
{
    public int Ma { get; set; }
    public string Text { get; set; }
}

public class LichKhamForm : Form
{
    static readonly string[] TrangThais = { "Chờ khám", "Đã khám", "Đã hủy" };
    TextBox txtMa, txtTen, txtSDT;
    DateTimePicker dtpNgay, dtpGio, dtpTu, dtpDen;
    ComboBox cboBS, cboTT, cboLocBS;
    DataGridView dgv;

    public LichKhamForm()
    {
        Text = "Quản Lý Lịch Khám Bệnh - An Khang Clinic";
        ClientSize = new Size(960, 560); StartPosition = FormStartPosition.CenterParent;
        txtMa = new TextBox { Visible = false };

        UI.Lbl(this, "Tên bệnh nhân", 15, 15); txtTen = UI.Txt(this, 110, 15, 200);
        UI.Lbl(this, "Số điện thoại", 15, 50); txtSDT = UI.Txt(this, 110, 50, 200);
        UI.Lbl(this, "Ngày khám", 15, 85);
        dtpNgay = new DateTimePicker { Left = 110, Top = 85, Width = 200, Format = DateTimePickerFormat.Short }; Controls.Add(dtpNgay);
        UI.Lbl(this, "Giờ khám", 340, 50);
        dtpGio = new DateTimePicker { Left = 340, Top = 75, Width = 100, Format = DateTimePickerFormat.Time, ShowUpDown = true }; Controls.Add(dtpGio);
        UI.Lbl(this, "Bác sĩ", 470, 15); cboBS = UI.Cbo(this, 470, 40, 260);
        UI.Lbl(this, "Trạng thái", 750, 15); cboTT = UI.Cbo(this, 750, 40, 120, TrangThais); cboTT.SelectedIndex = 0;

        var bThem = UI.Btn(this, "Thêm", 560, 85, 80); var bSua = UI.Btn(this, "Sửa", 650, 85, 80);
        var bXoa = UI.Btn(this, "Xóa", 740, 85, 80); var bMoi = UI.Btn(this, "Làm mới", 830, 85, 80);
        var bBS = UI.Btn(this, "Quản lý bác sĩ", 15, 125, 130);

        UI.Lbl(this, "Từ ngày", 15, 175);
        dtpTu = new DateTimePicker { Left = 75, Top = 175, Width = 110, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) }; Controls.Add(dtpTu);
        UI.Lbl(this, "Đến ngày", 200, 175);
        dtpDen = new DateTimePicker { Left = 270, Top = 175, Width = 110, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(3) }; Controls.Add(dtpDen);
        cboLocBS = UI.Cbo(this, 400, 175, 280);
        var bTim = UI.Btn(this, "Tìm kiếm", 695, 172, 90);
        var bTatCa = UI.Btn(this, "Hiện tất cả", 795, 172, 100);

        dgv = UI.Grid(this, 15, 215, 930, 335);
        UI.Col(dgv, "Mã lịch", "MaLich", 60); UI.Col(dgv, "Tên bệnh nhân", "TenBenhNhan", 140);
        UI.Col(dgv, "SĐT", "SDT", 100); UI.Col(dgv, "Ngày khám", "NgayKham", 90, "dd/MM/yyyy");
        UI.Col(dgv, "Giờ khám", "GioKham", 70); UI.Col(dgv, "Bác sĩ", "TenBS", 190);
        UI.Col(dgv, "Chuyên khoa", "ChuyenKhoa", 120); UI.Col(dgv, "Trạng thái", "TrangThai", 90);

        Load += async (_, _) => { await LoadCombos(); await LoadData(false); };
        dgv.SelectionChanged += (_, _) => Fill();
        bBS.Click += async (_, _) => { using (var f = new BacSiForm()) f.ShowDialog(this); await LoadCombos(); await LoadData(false); };
        bTim.Click += async (_, _) => await LoadData(true);
        bTatCa.Click += async (_, _) => await LoadData(false);
        bMoi.Click += async (_, _) => { Clear(); await LoadData(false); };
        bThem.Click += async (_, _) => await Save(true);
        bSua.Click += async (_, _) => await Save(false);
        bXoa.Click += async (_, _) => await Xoa();
    }

    async Task LoadCombos()
    {
        using var db = new AppDbContext();
        var ds = (await db.BacSis.AsNoTracking().OrderBy(b => b.MaBS).ToListAsync())
            .Select(b => new BsItem { Ma = b.MaBS, Text = $"BS. {b.HoTen} - {b.ChuyenKhoa}" }).ToList();
        cboBS.DataSource = ds.ToList(); cboBS.DisplayMember = "Text"; cboBS.ValueMember = "Ma";
        cboBS.SelectedIndex = -1;
        var loc = new List<BsItem> { new BsItem { Ma = 0, Text = "-- Tất cả bác sĩ --" } }; loc.AddRange(ds);
        cboLocBS.DataSource = loc; cboLocBS.DisplayMember = "Text"; cboLocBS.ValueMember = "Ma";
    }

    async Task LoadData(bool filter)
    {
        using var db = new AppDbContext();
        var q = db.LichKhams.Include(x => x.MaBSNavigation).AsNoTracking().AsQueryable();   // JOIN BacSi
        if (filter)
        {
            if (dtpTu.Value.Date > dtpDen.Value.Date) { UI.Warn("\"Từ ngày\" không được lớn hơn \"Đến ngày\"!"); return; }
            DateTime tu = dtpTu.Value.Date, den = dtpDen.Value.Date;
            int maBS = (cboLocBS.SelectedItem as BsItem)?.Ma ?? 0;
            q = q.Where(x => x.NgayKham >= tu && x.NgayKham <= den && (maBS == 0 || x.MaBS == maBS));
        }
        var list = await q.OrderBy(x => x.NgayKham).ThenBy(x => x.GioKham).ToListAsync();
        dgv.DataSource = list.Select(x => new LichRow
        {
            MaLich = x.MaLich, TenBenhNhan = x.TenBenhNhan, SDT = x.SDT, NgayKham = x.NgayKham,
            Gio = x.GioKham, GioKham = x.GioKham?.ToString(@"hh\:mm"),
            TenBS = x.MaBSNavigation != null ? $"BS. {x.MaBSNavigation.HoTen} - {x.MaBSNavigation.ChuyenKhoa}" : "",
            ChuyenKhoa = x.MaBSNavigation?.ChuyenKhoa ?? "",   // null-safe, tránh NullReferenceException
            TrangThai = x.TrangThai, MaBS = x.MaBS
        }).ToList();
        Fill();
    }

    void Fill()
    {
        if (dgv.CurrentRow?.DataBoundItem is not LichRow r) { return; }
        txtMa.Text = r.MaLich.ToString(); txtTen.Text = r.TenBenhNhan; txtSDT.Text = r.SDT;
        dtpNgay.Value = r.NgayKham ?? DateTime.Today;
        dtpGio.Value = DateTime.Today.Add(r.Gio ?? TimeSpan.Zero);
        if (r.MaBS != null) cboBS.SelectedValue = r.MaBS.Value; else cboBS.SelectedIndex = -1;
        int i = Array.IndexOf(TrangThais, r.TrangThai); cboTT.SelectedIndex = i < 0 ? 0 : i;
    }

    void Clear()
    {
        txtMa.Clear(); txtTen.Clear(); txtSDT.Clear(); dtpNgay.Value = DateTime.Today;
        dtpGio.Value = DateTime.Today.AddHours(8); cboBS.SelectedIndex = -1; cboTT.SelectedIndex = 0;
    }

    bool Validate(bool isNew, DateTime? oldDate)
    {
        if (txtTen.Text.Trim() == "") { UI.Warn("Tên bệnh nhân không được để trống!"); txtTen.Focus(); return false; }
        if (cboBS.SelectedValue == null) { UI.Warn("Vui lòng chọn bác sĩ!"); cboBS.Focus(); return false; }
        // Không cho đặt lịch ở quá khứ (khi sửa chỉ kiểm tra nếu đổi ngày)
        if (dtpNgay.Value.Date < DateTime.Today && (isNew || oldDate?.Date != dtpNgay.Value.Date))
        { UI.Warn("Không thể đặt lịch khám vào ngày trong quá khứ!"); dtpNgay.Focus(); return false; }
        string sdt = txtSDT.Text.Trim();
        if (sdt != "" && !Regex.IsMatch(sdt, @"^\d{9,11}$")) { UI.Warn("SĐT chỉ gồm 9-11 chữ số!"); txtSDT.Focus(); return false; }
        return true;
    }

    void ToEntity(LichKham l)
    {
        l.TenBenhNhan = txtTen.Text.Trim(); l.SDT = txtSDT.Text.Trim(); l.NgayKham = dtpNgay.Value.Date;
        var t = dtpGio.Value.TimeOfDay; l.GioKham = new TimeSpan(t.Hours, t.Minutes, 0);
        l.MaBS = (int)cboBS.SelectedValue; l.TrangThai = cboTT.SelectedItem.ToString();
    }

    async Task Save(bool isNew)
    {
        using var db = new AppDbContext();
        LichKham l;
        if (isNew)
        {
            if (!Validate(true, null)) return;
            l = new LichKham(); db.Add(l);
        }
        else
        {
            if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn lịch khám cần sửa!"); return; }
            l = await db.LichKhams.FindAsync(ma);
            if (l == null) { UI.Error("Lịch khám không còn tồn tại!"); await LoadData(false); return; }
            if (!Validate(false, l.NgayKham)) return;
        }
        ToEntity(l);
        await db.SaveChangesAsync();
        UI.Info(isNew ? "Đặt lịch khám thành công!" : "Cập nhật lịch khám thành công!");
        if (isNew) Clear();
        await LoadData(false);
    }

    async Task Xoa()
    {
        if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn lịch khám cần xóa!"); return; }
        if (!UI.Ask($"Bạn có chắc muốn xóa lịch khám của \"{txtTen.Text}\"?")) return;
        try
        {
            using var db = new AppDbContext();
            var l = await db.LichKhams.FindAsync(ma);
            if (l != null) { db.Remove(l); await db.SaveChangesAsync(); }
            UI.Info("Đã xóa lịch khám!"); Clear(); await LoadData(false);
        }
        catch (DbUpdateException ex) { UI.Error("Không thể xóa lịch khám: " + ex.GetBaseException().Message); }
    }
}
