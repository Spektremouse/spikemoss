using Spikemoss.ViewModels.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spikemoss.ViewModels
{
    abstract class DataLayerConfigurationViewModel : DataViewModel, IReportErrors, IReportProgress, IParticipant
    {
        private string _username;
        private string _password;
        private string _host;
        private string _connectionString;
        private string _errorMessage;
        private string _statusText = null;
        private string _message;
        private bool _canSave = false;        
        private int _value;

        private BackgroundWorker _testworker;
        private BackgroundWorker _saveworker;

        public event ProgressFinishHandler ProgressFinish;
        public event ErrorHandler ErrorOccurred;

        public DataLayerConfigurationViewModel()
        {
            ViewModelMediator.Instance.Register(this);

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

        protected string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        protected BackgroundWorker SaveWorker
        {
            get { return _saveworker; }
        }

        protected BackgroundWorker TestWorker
        {
            get { return _testworker; }
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

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged("ErrorMessage"); }
        }

        public int ProgressValue
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged("ProgressValue"); }
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
       
        protected abstract void TestWork(object sender, DoWorkEventArgs e);

        protected abstract void SaveWork(object sender, DoWorkEventArgs e);

        protected virtual void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ProgressMessage = (string)e.UserState;
            ProgressValue = (int)e.ProgressPercentage;
        }

        protected virtual void SaveWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            SendMessage(ViewModelMediator.Instance, this);
        }        

        protected virtual void TestWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
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

        public void ReceiveMessage(object message)
        {
            //Not Used
        }

        public void SendMessage(IMediator mediator, object message)
        {
            mediator.DistributeMessage(this, message);
        }        
    }
}
