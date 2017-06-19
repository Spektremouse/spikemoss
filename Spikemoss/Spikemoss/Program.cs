using Spikemoss.Views;
using System;
using System.Windows;
using Spikemoss.Models;
using Spikemoss.DataAccessLayer;

namespace Spikemoss
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ConfigurationWindow startup = new ConfigurationWindow();
            var app = new Application();
            app.Run(startup);
                      
            /*
            MainWindow mainWindow = new MainWindow();

            var app = new Application();

            app.Run(mainWindow);*/
        }
    }
}
