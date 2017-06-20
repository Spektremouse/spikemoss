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
            DataAccessLayerFactory factory = new DataAccessLayerFactory();
            var dal = factory.CreateDataAccessLayer(DataAccessLayerType.MSSQL, this.ConnectionString);

            this.SaveWorker.ReportProgress(0, "Starting");
            try
            {
                dal.CreateDatabase();
                dal.CreateTables();
                Properties.Settings.Default.ConnectionString = DataAccessLayer.ConnectionString;
                Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.MSSQL;
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
                var builder = new SqlConnectionStringBuilder();
                builder.ConnectionString = this.ConnectionString;
                builder.InitialCatalog = Properties.Settings.Default.DatabaseName;
                Properties.Settings.Default.ConnectionString = builder.ConnectionString;
                Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.MSSQL;
                Properties.Settings.Default.Save();
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
