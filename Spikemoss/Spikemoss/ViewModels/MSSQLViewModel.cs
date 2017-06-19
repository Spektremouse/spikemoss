using Spikemoss.DataAccessLayer;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Data.SqlClient;

namespace Spikemoss.ViewModels
{
    class MSSQLViewModel : DataViewModel, IReportProgress, IReportErrors
    {
        private string _username;
        private string _password;
        private string _host;
        private string _connectionString;
        private string _errorMessage;
        private string _statusText = null;
        private int _authenticationIndex = 0;
        private bool _authenticationEnabled = false;
        private bool _canSave = false;

        private string _message;
        private int _value;

        private BackgroundWorker _testworker;
        private BackgroundWorker _saveworker;
        public event ProgressFinishHandler ProgressFinish;
        public event ErrorHandler ErrorOccurred;

        public MSSQLViewModel()
        {
            _testworker = new BackgroundWorker();
            _testworker.DoWork += TestWork;
            _testworker.RunWorkerCompleted += TestWorkCompleted;
            _testworker.WorkerReportsProgress = true;
            _testworker.ProgressChanged += ProgressChanged;
            _testworker.WorkerSupportsCancellation = true;

            _saveworker = new BackgroundWorker();
            _saveworker.DoWork += SaveWork;
            _saveworker.RunWorkerCompleted += SaveWorkCompleted;
            _saveworker.WorkerReportsProgress = true;
            _saveworker.ProgressChanged += ProgressChanged;
            _saveworker.WorkerSupportsCancellation = true;
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged("Username"); }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged("Password"); }
        }

        public string Host
        {
            get { return _host; }
            set { _host = value; OnPropertyChanged("Host"); }
        }

        public string ProgressMessage
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("ProgressMessage"); }
        }

        public string StatusText
        {
            get { return _statusText; }
            set { _statusText = value; OnPropertyChanged("StatusText"); }
        }

        public int ProgressValue
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("ProgressValue"); }
        }

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

        public bool SaveEnabled
        {
            get { return _canSave; }
            set { _canSave = value; OnPropertyChanged("SaveEnabled"); }
        }

        public ICommand SaveConnection
        {
            get { return new DelegateCommand(_saveworker.RunWorkerAsync); }
        }

        public ICommand TestConnection
        {
            get { return new DelegateCommand(_testworker.RunWorkerAsync); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged("ErrorMessage"); }
        }

        private void SaveWork(object sender, DoWorkEventArgs e)
        {
            _saveworker.ReportProgress(0, "Starting");
            Properties.Settings.Default.ConnectionString = _connectionString;
            Properties.Settings.Default.DataAccessLayerType = (int)DataAccessLayerType.MSSQL;

            try
            {
                DataAccessLayer.ConnectionString = _connectionString;
                DataAccessLayer.CreateDatabase();
                Properties.Settings.Default.ConnectionString = DataAccessLayer.ConnectionString;
                Properties.Settings.Default.Save();
                ProgressMessage = "Done";
                ProgressValue = 100;
            }
            catch (Exception ex)
            {
                _saveworker.ReportProgress(100, ex.Message);
                if (ex.Message.Contains("already exists"))
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.ConnectionString = _connectionString;
                    builder.InitialCatalog = Properties.Settings.Default.DatabaseName;

                    Properties.Settings.Default.ConnectionString = builder.ConnectionString;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void SaveWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ErrorMessage = e.Error.Message;
                ErrorOccurred(sender, e);
                StatusText = null;
            }
            StatusText = "Save Complete.";
            Mouse.OverrideCursor = null;
            ProgressFinish(this, new EventArgs());
        }

        private void TestWork(object sender, DoWorkEventArgs e)
        {            
            var builder = new SqlConnectionStringBuilder();

            _testworker.ReportProgress(0, "Starting");
            builder.DataSource = Host;
            builder.IntegratedSecurity = !AuthenticationEnabled;
            if (AuthenticationEnabled)
            {
                builder.UserID = Username;
                builder.Password = Password;
            }

            using (var con = new SqlConnection(builder.ConnectionString))
            {
                _testworker.ReportProgress(20, "Testing connection to server.");
                con.Open();
                _testworker.ReportProgress(80, "Connection successful.");
                _testworker.ReportProgress(90, "Closing connection.");
                con.Close();

                _connectionString = builder.ConnectionString;
                SaveEnabled = true;
                ProgressMessage = "Done";
                ProgressValue = 100;
            }
        }

        private void TestWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ErrorMessage = e.Error.Message;
                ErrorOccurred(sender, e);
                StatusText = null;
            }
            else
            {
                StatusText = "Test Complete.";
            }
            Mouse.OverrideCursor = null;
            ProgressFinish(this, new EventArgs());
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ProgressMessage = (string)e.UserState;
            ProgressValue = (int)e.ProgressPercentage;
        }
    }
}
