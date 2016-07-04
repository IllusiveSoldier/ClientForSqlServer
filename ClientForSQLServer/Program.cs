using System;
using System.Windows.Forms;

namespace ClientForSqlServer
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    static class Data
    {
        private static string stringValueForRTB = null;

        public static string stringValueForRTBGetSet { get { return stringValueForRTB; } set { stringValueForRTB = value; } }
    }

    static class SizeOfMainForm
    {
        private static int valueOfX;
        private static int valueOfY;

        public static int valueOfXGetSet { get { return valueOfX; } set { valueOfX = value; } }
        public static int valueOfYGetSet { get { return valueOfY; } set { valueOfY = value; } }
    }
}
