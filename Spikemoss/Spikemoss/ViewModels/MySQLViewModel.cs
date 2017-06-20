using MySql.Data.MySqlClient;
using Spikemoss.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spikemoss.ViewModels
{
    class MySQLViewModel : DataLayerConfigurationViewModel
    {
        private int _port = 3306;

        public MySQLViewModel() : base() { }

        public int Port
        {
            get { return _port; }
            set { _port = value; OnPropertyChanged("Port"); }
        }

        override protected void SaveWork(object sender, DoWorkEventArgs e)
        {
            DataAccessLayerFactory factory = new DataAccessLayerFactory();
            var dal = factory.CreateDataAccessLayer(DataAccessLayerType.MySQL, this.ConnectionString);
            
            this.SaveWorker.ReportProgress(0, "Starting");
            try
            {
                dal.CreateDatabase();
                dal.CreateTables();
                Properties.Settings.Default.ConnectionString = DataAccessLayer.ConnectionString;
                Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.MySQL;
                Properties.Settings.Default.Save();
                ProgressMessage = "Done";
                ProgressValue = 100;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                var builder = new MySqlConnectionStringBuilder();
                builder.ConnectionString = this.ConnectionString;
                builder.Database = Properties.Settings.Default.DatabaseName;
                Properties.Settings.Default.ConnectionString = builder.ConnectionString;
                Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.MySQL;
                Properties.Settings.Default.Save();
            }
        }

        override protected void TestWork(object sender, DoWorkEventArgs e)
        {
            var builder = new MySqlConnectionStringBuilder();

            this.TestWorker.ReportProgress(0, "Starting");
            builder.Server = Host;
            builder.Port = (uint)Port;
            builder.UserID = Username;
            builder.Password = Password;

            using (var con = new MySqlConnection(builder.ConnectionString))
            {
                this.TestWorker.ReportProgress(20, "Testing connection to server.");
                con.Open();
                this.TestWorker.ReportProgress(80, "Connection successful.");
                this.TestWorker.ReportProgress(90, "Closing connection.");
                con.Close();

                this.ConnectionString = builder.ConnectionString;
                SaveEnabled = true;
                ProgressMessage = "Done";
                ProgressValue = 100;
            }
        }

    }
}
