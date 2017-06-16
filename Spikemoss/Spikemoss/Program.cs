using Spikemoss.Views;
using System;
using System.Windows;

namespace Spikemoss
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            MainWindow mainWindow = new MainWindow();

            var app = new Application();

            app.Run(mainWindow);
        }
    }
}
