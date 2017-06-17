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
            var constr = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();

            constr.Password = "Crazedpenguin1";
            constr.UserID = "root";
            constr.Server = "192.168.1.102";
            IDataAccessLayer idal = MySQL.Instance;
            idal.ConnectionString = constr.ConnectionString;
            idal.CreateDatabase();
            idal.CreateTables();
                      
            /*
            MainWindow mainWindow = new MainWindow();

            var app = new Application();

            app.Run(mainWindow);*/
        }
    }
}
