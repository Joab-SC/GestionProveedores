namespace GestionProveedores
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            using (var login = new FrmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }
            Application.Run(new FrmMenuPrincipal());
        }
    }
}