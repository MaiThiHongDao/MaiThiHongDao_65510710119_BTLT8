namespace Cau3_Homestay;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        try { DbInitializer.Run(); }
        catch (Exception ex)
        {
            UI.Error("Không kết nối / tạo được database.\nKiểm tra ConnectionString trong AppDbContext.cs.\n\n" + ex.Message);
            return;
        }
        Application.Run(new PhongForm());
    }
}
