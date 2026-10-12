using System;
using System.Windows.Forms;

namespace BaiTapThucHanh4
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Bai4_7Form());
        }
    }
}