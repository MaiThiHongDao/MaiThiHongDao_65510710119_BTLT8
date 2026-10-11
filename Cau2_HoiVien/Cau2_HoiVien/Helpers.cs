namespace Cau2_HoiVien;

public static class UI
{
    public static Label Lbl(Control p, string t, int x, int y)
    { var l = new Label { Text = t, Left = x, Top = y + 3, AutoSize = true }; p.Controls.Add(l); return l; }
    public static TextBox Txt(Control p, int x, int y, int w, bool ro = false)
    { var t = new TextBox { Left = x, Top = y, Width = w, ReadOnly = ro }; p.Controls.Add(t); return t; }
    public static Button Btn(Control p, string t, int x, int y, int w = 90)
    { var b = new Button { Text = t, Left = x, Top = y, Width = w, Height = 28 }; p.Controls.Add(b); return b; }
    public static ComboBox Cbo(Control p, int x, int y, int w, params object[] items)
    {
        var c = new ComboBox { Left = x, Top = y, Width = w, DropDownStyle = ComboBoxStyle.DropDownList };
        if (items.Length > 0) c.Items.AddRange(items);
        p.Controls.Add(c); return c;
    }
    public static DataGridView Grid(Control p, int x, int y, int w, int h)
    {
        var g = new DataGridView
        {
            Left = x, Top = y, Width = w, Height = h, ReadOnly = true, AllowUserToAddRows = false,
            AllowUserToDeleteRows = false, MultiSelect = false, AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = true,
            BackgroundColor = Color.White,
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        p.Controls.Add(g); return g;
    }
    public static void Col(DataGridView g, string header, string prop, int w, string fmt = null)
    {
        var c = new DataGridViewTextBoxColumn { HeaderText = header, DataPropertyName = prop, Width = w, Name = prop };
        if (fmt != null) c.DefaultCellStyle.Format = fmt;
        g.Columns.Add(c);
    }
    public static void Info(string m) => MessageBox.Show(m, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
    public static void Warn(string m) => MessageBox.Show(m, "Dữ liệu không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    public static void Error(string m) => MessageBox.Show(m, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
    public static bool Ask(string m) =>
        MessageBox.Show(m, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
}

public static class ImageHelper
{
    public static string Dir => Path.Combine(AppContext.BaseDirectory, "Images");

    /// <summary>Đọc ảnh vào bộ nhớ (không khóa file). Trả null nếu không có ảnh.</summary>
    public static Image Load(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return null;
        string path = Path.Combine(Dir, fileName);
        if (!File.Exists(path)) return null;
        using var ms = new MemoryStream(File.ReadAllBytes(path));
        return new Bitmap(ms);
    }

    /// <summary>Copy ảnh vào thư mục Images, trả về TÊN FILE (không phải full path).</summary>
    public static string CopyToImages(string sourcePath)
    {
        Directory.CreateDirectory(Dir);
        string name = Path.GetFileName(sourcePath);
        string dest = Path.Combine(Dir, name);
        if (File.Exists(dest) && !string.Equals(Path.GetFullPath(sourcePath), Path.GetFullPath(dest), StringComparison.OrdinalIgnoreCase))
        {
            name = $"{Path.GetFileNameWithoutExtension(name)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(name)}";
            dest = Path.Combine(Dir, name);
        }
        if (!File.Exists(dest)) File.Copy(sourcePath, dest);
        return name;
    }
}
