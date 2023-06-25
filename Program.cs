namespace VisualStudioDownloader
{
    using System;
    using System.Windows.Forms;

    public delegate void DelReadErrOutput(string result);

    public delegate void DelReadStdOutput(string result);

    internal static class Program
    {
        [STAThread] 
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

