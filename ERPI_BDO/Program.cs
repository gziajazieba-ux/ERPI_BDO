namespace ERPI_BDO
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FormGlowne());
        }
    }
}