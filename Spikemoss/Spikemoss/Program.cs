using Spikemoss.Views;
using Spikemoss.ViewModels.Communication;
using System;
using System.Windows;
using Spikemoss.Models;
using Spikemoss.DataAccessLayer;
using Spikemoss.ViewModels;

namespace Spikemoss
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            DataAccessLayerFactory idalfactory = new DataAccessLayerFactory();
            idalfactory.CreateDataAccessLayer((DataAccessLayerType)Properties.Settings.Default.DataAccessLayerType, Properties.Settings.Default.ConnectionString);

            MainViewModel mainViewModel = new MainViewModel();
            MainWindow mainWindow = new MainWindow(mainViewModel);
            var app = new Application();
            app.Run(mainWindow);                              
        }
    }
}
