namespace Cau3_Homestay;

// ===== CÂU 3 (Form chính): Quản lý Phòng + upload ảnh =====
public class PhongRow
{
    public int MaPhong { get; set; }
    public Image Anh { get; set; }
    public string SoPhong { get; set; }
    public int? TangSo { get; set; }
    public string TenLoai { get; set; }
    public decimal? GiaMoiDem { get; set; }
    public string TinhTrang { get; set; }
    public string HinhAnh { get; set; }
    public int? MaLoai { get; set; }
}

public class PhongForm : Form
{
    static readonly string[] TinhTrangs = { "Trống", "Đang ở", "Đang dọn" };
    TextBox txtMa, txtSoPhong;
    NumericUpDown nudTang;
    ComboBox cboLoai, cboTinhTrang, cboLocLoai, cboLocTT;
    PictureBox pic;
    DataGridView dgv;
    string _hinhAnh; // chỉ lưu TÊN FILE

    public PhongForm()
    {
        Text = "Quản Lý Phòng - Sunrise Homestay";
        ClientSize = new Size(940, 600); StartPosition = FormStartPosition.CenterParent;
        txtMa = new TextBox { Visible = false };

        UI.Lbl(this, "Số phòng", 15, 10); txtSoPhong = UI.Txt(this, 15, 35, 130);
        UI.Lbl(this, "Tầng số", 165, 10);
        nudTang = new NumericUpDown { Left = 165, Top = 35, Width = 70, Minimum = 0, Maximum = 100 }; Controls.Add(nudTang);
        UI.Lbl(this, "Loại phòng", 260, 10); cboLoai = UI.Cbo(this, 260, 35, 150);
        UI.Lbl(this, "Tình trạng", 430, 10); cboTinhTrang = UI.Cbo(this, 430, 35, 130, TinhTrangs); cboTinhTrang.SelectedIndex = 0;
        pic = new PictureBox { Left = 585, Top = 10, Width = 120, Height = 80, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
        Controls.Add(pic);
        var btnAnh = UI.Btn(this, "Chọn ảnh...", 595, 95, 100);
        var btnQL = UI.Btn(this, "Quản lý loại phòng", 15, 75, 150); btnQL.Width = 150;

        var bThem = UI.Btn(this, "Thêm", 730, 10, 95); var bSua = UI.Btn(this, "Sửa", 830, 10, 95);
        var bXoa = UI.Btn(this, "Xóa", 730, 45, 95); var bMoi = UI.Btn(this, "Làm mới", 830, 45, 95);

        cboLocLoai = UI.Cbo(this, 15, 135, 180); cboLocTT = UI.Cbo(this, 210, 135, 150);
        cboLocTT.Items.Add("-- Tất cả tình trạng --"); cboLocTT.Items.AddRange(TinhTrangs); cboLocTT.SelectedIndex = 0;
        var bTim = UI.Btn(this, "Tìm kiếm", 380, 133);

        dgv = UI.Grid(this, 15, 175, 910, 415);
        dgv.RowTemplate.Height = 50;
        UI.Col(dgv, "Mã phòng", "MaPhong", 70);
        dgv.Columns.Add(new DataGridViewImageColumn { HeaderText = "Ảnh", DataPropertyName = "Anh", Name = "Anh", Width = 90, ImageLayout = DataGridViewImageCellLayout.Zoom });
        UI.Col(dgv, "Số phòng", "SoPhong", 100); UI.Col(dgv, "Tầng", "TangSo", 70);
        UI.Col(dgv, "Loại phòng", "TenLoai", 150); UI.Col(dgv, "Giá/đêm", "GiaMoiDem", 110, "N0");
        UI.Col(dgv, "Tình trạng", "TinhTrang", 110);

        Load += async (_, _) => { await LoadCombos(); await LoadData(); };
        dgv.SelectionChanged += (_, _) => Fill();
        btnAnh.Click += (_, _) => ChonAnh();
        btnQL.Click += async (_, _) => { using (var f = new LoaiPhongForm()) f.ShowDialog(this); await LoadCombos(); await LoadData(); };
        bTim.Click += async (_, _) => await LoadData();
        bMoi.Click += async (_, _) => { Clear(); cboLocLoai.SelectedIndex = 0; cboLocTT.SelectedIndex = 0; await LoadData(); };
        bThem.Click += async (_, _) => await Save(true);
        bSua.Click += async (_, _) => await Save(false);
        bXoa.Click += async (_, _) => await Xoa();
    }

    async Task LoadCombos()
    {
        using var db = new AppDbContext();
        var loais = await db.LoaiPhongs.AsNoTracking().OrderBy(x => x.MaLoai).ToListAsync();
        cboLoai.DataSource = loais.ToList(); cboLoai.DisplayMember = "TenLoai"; cboLoai.ValueMember = "MaLoai";
        var loc = new List<LoaiPhong> { new LoaiPhong { MaLoai = 0, TenLoai = "-- Tất cả loại phòng --" } }; loc.AddRange(loais);
        cboLocLoai.DataSource = loc; cboLocLoai.DisplayMember = "TenLoai"; cboLocLoai.ValueMember = "MaLoai";
    }

    async Task LoadData()
    {
        int maLoai = (cboLocLoai.SelectedItem as LoaiPhong)?.MaLoai ?? 0;
        string tt = cboLocTT.SelectedIndex > 0 ? cboLocTT.SelectedItem.ToString() : null;
        using var db = new AppDbContext();
        var list = await db.Phongs.Include(p => p.MaLoaiNavigation).AsNoTracking()   // Eager loading
            .Where(p => (maLoai == 0 || p.MaLoai == maLoai) && (tt == null || p.TinhTrang == tt))
            .OrderBy(p => p.MaPhong).ToListAsync();
        dgv.DataSource = list.Select(p => new PhongRow
        {
            MaPhong = p.MaPhong, Anh = ImageHelper.Load(p.HinhAnh), SoPhong = p.SoPhong, TangSo = p.TangSo,
            TenLoai = p.MaLoaiNavigation?.TenLoai, GiaMoiDem = p.MaLoaiNavigation?.GiaMoiDem,
            TinhTrang = p.TinhTrang, HinhAnh = p.HinhAnh, MaLoai = p.MaLoai
        }).ToList();
        Fill();
    }

    void Fill()
    {
        if (dgv.CurrentRow?.DataBoundItem is not PhongRow r) { Clear(); return; }
        txtMa.Text = r.MaPhong.ToString(); txtSoPhong.Text = r.SoPhong; nudTang.Value = r.TangSo ?? 0;
        if (r.MaLoai != null) cboLoai.SelectedValue = r.MaLoai.Value;
        int i = Array.IndexOf(TinhTrangs, r.TinhTrang); cboTinhTrang.SelectedIndex = i < 0 ? 0 : i;
        _hinhAnh = r.HinhAnh; ShowPic(r.HinhAnh);
    }

    void ShowPic(string file) { var old = pic.Image; pic.Image = ImageHelper.Load(file); old?.Dispose(); }

    void Clear()
    {
        txtMa.Clear(); txtSoPhong.Clear(); nudTang.Value = 0; cboTinhTrang.SelectedIndex = 0;
        if (cboLoai.Items.Count > 0) cboLoai.SelectedIndex = 0;
        _hinhAnh = null; ShowPic(null);
    }

    void ChonAnh()
    {
        using var ofd = new OpenFileDialog { Filter = "Ảnh|*.jpg;*.jpeg;*.png;*.bmp;*.gif", Title = "Chọn ảnh phòng" };
        if (ofd.ShowDialog() != DialogResult.OK) return;
        try
        {
            _hinhAnh = ImageHelper.CopyToImages(ofd.FileName); // copy vào /Images, chỉ giữ tên file
            ShowPic(_hinhAnh);
        }
        catch (Exception ex) { UI.Error("Không thể tải ảnh: " + ex.Message); }
    }

    bool Validate()
    {
        if (txtSoPhong.Text.Trim() == "") { UI.Warn("Số phòng không được để trống!"); txtSoPhong.Focus(); return false; }
        if (cboLoai.SelectedValue == null) { UI.Warn("Hãy chọn loại phòng (thêm loại phòng trước nếu chưa có)!"); return false; }
        return true;
    }

    async Task Save(bool isNew)
    {
        if (!Validate()) return;
        string so = txtSoPhong.Text.Trim();
        using var db = new AppDbContext();
        Phong p;
        if (isNew)
        {
            if (await db.Phongs.AnyAsync(x => x.SoPhong == so)) { UI.Warn($"Số phòng {so} đã tồn tại!"); return; }
            p = new Phong(); db.Add(p);
        }
        else
        {
            if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn phòng cần sửa!"); return; }
            p = await db.Phongs.FindAsync(ma);
            if (p == null) { UI.Error("Phòng không còn tồn tại!"); await LoadData(); return; }
            if (await db.Phongs.AnyAsync(x => x.SoPhong == so && x.MaPhong != ma)) { UI.Warn($"Số phòng {so} đã tồn tại!"); return; }
        }
        p.SoPhong = so; p.TangSo = (int)nudTang.Value; p.MaLoai = (int)cboLoai.SelectedValue;
        p.TinhTrang = cboTinhTrang.SelectedItem.ToString(); p.HinhAnh = _hinhAnh;
        await db.SaveChangesAsync();
        UI.Info(isNew ? "Thêm phòng thành công!" : "Cập nhật phòng thành công!");
        if (isNew) Clear();
        await LoadData();
    }

    async Task Xoa()
    {
        if (!int.TryParse(txtMa.Text, out int ma)) { UI.Warn("Hãy chọn phòng cần xóa!"); return; }
        if (!UI.Ask($"Bạn có chắc muốn xóa phòng {txtSoPhong.Text}?")) return;
        try
        {
            using var db = new AppDbContext();
            var p = await db.Phongs.FindAsync(ma);
            if (p != null) { db.Remove(p); await db.SaveChangesAsync(); }
            UI.Info("Đã xóa phòng!"); Clear(); await LoadData();
        }
        catch (DbUpdateException ex) { UI.Error("Không thể xóa phòng: " + ex.GetBaseException().Message); }
    }
}
