using Spikemoss.Models;
using Spikemoss.ViewModels.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spikemoss.ViewModels
{
    public class ServerViewModel : BaseViewModel, IReportErrors
    {
        private string _errorMessage;
        private Hardware _hardware;
        private Server _server;
        private User _user;
        private UserViewModel _userViewModel;        
        private ObservableCollection<ServerViewModel> _virtualGuestList;

        public event ErrorHandler ErrorOccurred;
        public event EventHandler SaveFinished;

        public ServerViewModel()
        {
            _server = new Server();
            _user = new User();
            _hardware = new Hardware();
            _server.Hardware = _hardware;
            _server.User = _user;
            _userViewModel = new UserViewModel(_user);
        }

        public ServerViewModel(Server server)
        {
            _server = server;
            _userViewModel = new UserViewModel(server.User);
        }

        public int ServerID
        {
            get { return _server.ServerID; }
            set { _server.ServerID = value; OnPropertyChanged("ServerID"); }
        }

        public int ClusterID
        {
            get { return _server.ClusterID; }
            set { _server.ClusterID = value; OnPropertyChanged("ClusterID"); }
        }

        public Server Server
        {
            get { return _server; }
            set { _server = value; }
        }

        public UserViewModel User
        {
            get { return _userViewModel; }
            set { _userViewModel = value; OnPropertyChanged("User"); }
        }

        public string Hostname
        {
            get { return _server.Hostname; }
            set { _server.Hostname = value; OnPropertyChanged("Hostname"); }
        }

        public string Address
        {
            get { return _server.Address; }
            set { _server.Address = value; OnPropertyChanged("Address"); }
        }

        public int ServerType
        {
            get { return (int)_server.ServerType; }
            set { _server.ServerType = (ServerType)value; OnPropertyChanged("ServerType"); }
        }

        public bool IsConfigured
        {
            get { return _server.IsConfigured; }
            set { _server.IsConfigured = value; OnPropertyChanged("IsConfigured"); }
        }

        public bool IsVirtual
        {
            get { return _server.IsVirtual; }
            set { _server.IsVirtual = value; OnPropertyChanged("IsVirtual"); }
        }

        public string Error
        {
            get { return _server.Error; }
            set { _server.Error = value; OnPropertyChanged("Error"); }
        }

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                _errorMessage = value;
            }
        }

        public int OperatingSystem
        {
            get { return (int)_server.OperatingSystem; }
            set { _server.OperatingSystem = (OperatingSystemType)value; OnPropertyChanged("OperatingSystem"); }
        }

        public int SSHPort
        {
            get { return _server.SSHPort; }
            set { _server.SSHPort = value; OnPropertyChanged("SSHPort"); }
        }

        public ICommand SaveCommand
        {
            get { return new DelegateCommand(Save); }
        }

        private void Save()
        {
            try
            {
                RepositoryHelper.Instance.Save(this);

                var handler = SaveFinished;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
                Error = "";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Error = ErrorMessage;
                var handler = ErrorOccurred;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }
        }
    }
}
