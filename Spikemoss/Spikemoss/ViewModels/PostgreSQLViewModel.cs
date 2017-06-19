using Spikemoss.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Npgsql;
using System.Security;

namespace Spikemoss.ViewModels
{
    class PostgreSQLViewModel : DataLayerConfigurationViewModel
    {
        private int _port = 5432;

        public PostgreSQLViewModel() : base() { }

        public int Port
        {
            get { return _port; }
            set { _port = value; OnPropertyChanged("Port"); }
        }

        override protected void SaveWork(object sender, DoWorkEventArgs e)
        {
            this.SaveWorker.ReportProgress(0, "Starting");
            Properties.Settings.Default.ConnectionString = this.ConnectionString;
            Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.PostgreSQL;

            try
            {
                DataAccessLayer.ConnectionString = this.ConnectionString;
                DataAccessLayer.CreateDatabase();
                Properties.Settings.Default.ConnectionString = DataAccessLayer.ConnectionString;
                ProgressMessage = "Done";
                ProgressValue = 100;
            }
            catch (Exception ex)
            {
                this.SaveWorker.ReportProgress(100, ex.Message);
                if (ex.Message.Contains("already exists"))
                {
                    var builder = new NpgsqlConnectionStringBuilder();
                    builder.ConnectionString = this.ConnectionString;
                    builder.Database = Properties.Settings.Default.DatabaseName;
                    Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.PostgreSQL;
                    Properties.Settings.Default.ConnectionString = builder.ConnectionString;
                }
            }
        }

        override protected void TestWork(object sender, DoWorkEventArgs e)
        {            
            var builder = new NpgsqlConnectionStringBuilder();

            this.TestWorker.ReportProgress(0, "Starting");
            builder.Host = this.Host;
            builder.Port = this.Port;
            builder.Username = this.Username;
            builder.Password = this.Password;

            Console.WriteLine(builder.ConnectionString);

            using (var con = new NpgsqlConnection(builder.ConnectionString))
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
