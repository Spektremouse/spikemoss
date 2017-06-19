using Spikemoss.DataAccessLayer;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Data.SqlClient;

namespace Spikemoss.ViewModels
{
    class MSSQLViewModel : DataLayerConfigurationViewModel
    {
        private int _authenticationIndex = 0;
        private bool _authenticationEnabled = false;

        public MSSQLViewModel() : base() { }

        public int AuthenticationIndex
        {
            get { return _authenticationIndex; }
            set
            {
                _authenticationIndex = value;
                OnPropertyChanged("AuthenticationIndex");
                if (_authenticationIndex == 2)
                {
                    AuthenticationEnabled = true;
                }
                else
                {
                    AuthenticationEnabled = false;
                }
            }
        }

        public bool AuthenticationEnabled
        {
            get { return _authenticationEnabled; }
            set { _authenticationEnabled = value; OnPropertyChanged("AuthenticationEnabled"); }
        }

        override protected void SaveWork(object sender, DoWorkEventArgs e)
        {
            this.SaveWorker.ReportProgress(0, "Starting");
            Properties.Settings.Default.ConnectionString = this.ConnectionString;
            Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.MSSQL;

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
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.ConnectionString = this.ConnectionString;
                    builder.InitialCatalog = Properties.Settings.Default.DatabaseName;

                    Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.MSSQL;
                    Properties.Settings.Default.ConnectionString = builder.ConnectionString;
                }
            }
        }

        override protected void TestWork(object sender, DoWorkEventArgs e)
        {            
            var builder = new SqlConnectionStringBuilder();

            this.TestWorker.ReportProgress(0, "Starting");
            builder.DataSource = Host;
            builder.IntegratedSecurity = !AuthenticationEnabled;
            if (AuthenticationEnabled)
            {
                builder.UserID = Username;
                builder.Password = Password;
            }

            using (var con = new SqlConnection(builder.ConnectionString))
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
