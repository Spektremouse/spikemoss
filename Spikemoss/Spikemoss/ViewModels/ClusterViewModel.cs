using Spikemoss.Models;
using Spikemoss.ViewModels.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spikemoss.ViewModels
{
    public class ClusterViewModel : BaseViewModel, IParticipant, IReportErrors
    {
        private const string LOAD_COMPLETE_MESSAGE = "LoadComplete";
        private const string UNCLUSTERED_NAME = "Unclusterd";

        private Cluster _cluster;
        private ObservableCollection<ServerViewModel> _serverList;
        private ObservableCollection<ServerViewModel> _filteredList;

        private ServerViewModel _selectedServer;
        private string _searchText;
        private string _errorMessage;
        private bool _hideAttachDetach = true;
        private bool _showAll = true;
        private bool _showPhysical = false;
        private bool _showVirtual = false;
        private bool _showUnknown = false;

        public event ErrorHandler ErrorOccurred;
        public event EventHandler SaveFinished;

        public ClusterViewModel()
        {
            ViewModelMediator.Instance.Register(this);
            _serverList = new ObservableCollection<ServerViewModel>();
            _cluster = new Cluster();
        }

        public ClusterViewModel(Cluster cluster)
        {
            ViewModelMediator.Instance.Register(this);
            _serverList = new ObservableCollection<ServerViewModel>();
            _cluster = cluster;
            if (_cluster.Name == UNCLUSTERED_NAME)
            {
                HideAttachDetach = false;
            }
        }

        //The list of servers attached to the cluster
        public ObservableCollection<ServerViewModel> ServerList
        {
            get
            {
                if (_cluster.Name == UNCLUSTERED_NAME && _cluster.ClusterID == 0)
                {
                    return new ObservableCollection<ServerViewModel>
                        (
                        RepositoryHelper.Instance.Servers.Where(server =>
                        server.ClusterID == _cluster.ClusterID)
                        );
                }
                else if (_cluster.Name != UNCLUSTERED_NAME && _cluster.ClusterID == 0)
                {
                    return new ObservableCollection<ServerViewModel>();
                }
                else
                {
                    return new ObservableCollection<ServerViewModel>
                        (
                        RepositoryHelper.Instance.Servers.Where(server =>
                        server.ClusterID == _cluster.ClusterID && _cluster.Name != UNCLUSTERED_NAME)
                        );
                }                
            }
            set { _serverList = value; OnPropertyChanged("ServerList"); }
        }

        //The filtered list of servers to be attached
        public ObservableCollection<ServerViewModel> FilteredServers
        {
            get
            {
                return new ObservableCollection<ServerViewModel>
                        (
                        RepositoryHelper.Instance.Servers.Where(server =>
                        server.ClusterID != _cluster.ClusterID && server.ClusterID == 0)
                        );
            }
            set { _filteredList = value; OnPropertyChanged("FilteredServers"); } 
        }

        public int ClusterID
        {
            get { return _cluster.ClusterID; }
            private set { _cluster.ClusterID = value; }
        }

        public string Name
        {
            get { return _cluster.Name; }
            set { _cluster.Name = value; OnPropertyChanged("Name"); }
        }

        public bool ShowAll
        {
            get { return _showAll; }
            set { _showAll = value; OnPropertyChanged("ShowAll"); FilterChanged(); }
        }

        public bool ShowPhysical
        {
            get { return _showPhysical; }
            set { _showPhysical = value; OnPropertyChanged("ShowPhysical"); FilterChanged(); }
        }

        public bool ShowVirtual
        {
            get { return _showVirtual; }
            set { _showVirtual = value; OnPropertyChanged("ShowVirtual"); FilterChanged(); }
        }

        public bool ShowUnknown
        {
            get { return _showUnknown; }
            set { _showUnknown = value; OnPropertyChanged("ShowUnknown"); FilterChanged(); }
        }

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; OnPropertyChanged("SearchText"); FilterChanged(); }
        }

        public bool HideAttachDetach
        {
            get { return _hideAttachDetach; }
            set { _hideAttachDetach = value;  OnPropertyChanged("HideAttachDetach"); }
        }

        public bool ReadOnlyName
        {
            get { return !_hideAttachDetach; }
        }

        public ServerViewModel SelectedServer
        {
            get { return _selectedServer; }
            set { _selectedServer = value; OnPropertyChanged("SelectedServer"); }
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
                foreach (var serverVm in _serverList)
                {
                    if (serverVm != null)
                    {
                        RepositoryHelper.Instance.Save(serverVm);
                    }                    
                }
                var handler = SaveFinished;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                var handler = ErrorOccurred;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }
        }

        public ICommand AttachCommand
        {
            get { return new DelegateCommand(Attach); }
        }

        private void Attach()
        {
            if (!_serverList.Contains(SelectedServer) && SelectedServer != null)
            {
                SelectedServer.ClusterID = ClusterID;
            }            
            Save();
            ServerList = ServerList;
            FilteredServers = FilteredServers;
        }

        private void Detach()
        {
            if (_serverList.Contains(SelectedServer))
            {
                SelectedServer.ClusterID = 0;
                Save();
            }
            ServerList = ServerList;
            FilteredServers = FilteredServers;
        }

        public ICommand DetachCommand
        {
            get { return new DelegateCommand(Detach); }
        }

        public ICommand DeleteCommand
        {
            get { return new DelegateCommand(Delete); }
        }

        private void Delete()
        {            
            try
            {
                RepositoryHelper.Instance.Delete(this);
                ViewModelMediator.Instance.Unregister(this);
                SendMessage(ViewModelMediator.Instance, this);
            }        
            catch (Exception ex)
            {
                ErrorMessage = "Error deleting cluster. " + ex.Message;
                ErrorOccurred(this, new EventArgs());
            }            
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

        public void ReceiveMessage(object message)
        {

        }

        public void SendMessage(IMediator mediator, object message)
        {
            mediator.DistributeMessage(this, message);
        }

        private void FilterChanged()
        {
            if (ShowAll == true)
            {
                FilteredServers = FilteredServers;
            }
            if (ShowPhysical == true)
            {
                FilteredServers = new ObservableCollection<ServerViewModel>(FilteredServers.Where(server =>
                server.ServerType == (int)ServerType.Physical));
            }
            if (ShowVirtual == true)
            {
                FilteredServers = new ObservableCollection<ServerViewModel>(FilteredServers.Where(server =>
                server.ServerType == (int)ServerType.Virtual));
            }
            if (ShowUnknown == true)
            {
                FilteredServers = new ObservableCollection<ServerViewModel>(FilteredServers.Where(server =>
                server.ServerType == (int)ServerType.Unknown));
            }
            if (!String.IsNullOrWhiteSpace(SearchText))
            {
                FilteredServers = new ObservableCollection<ServerViewModel>(FilteredServers.Where(server =>
                server.Address.Contains(SearchText) || server.Hostname.Contains(SearchText)));
            }
        }
    }
}
