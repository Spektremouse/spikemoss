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
    public class ClusterViewModel : DataViewModel, IParticipant, IReportErrors
    {
        private const string LOAD_COMPLETE_MESSAGE = "LoadComplete";

        private IList<ServerViewModel> _serverListToLoad;
        private Cluster _cluster;
        private ObservableCollection<ServerViewModel> _serverViewModelList;
        private ObservableCollection<ServerViewModel> _listToFilter;
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
            _serverViewModelList = new ObservableCollection<ServerViewModel>();
            _cluster = new Cluster();
        }

        public ClusterViewModel(Cluster cluster)
        {
            ViewModelMediator.Instance.Register(this);
            _serverViewModelList = new ObservableCollection<ServerViewModel>();
            _cluster = cluster;
            if (_cluster.Name == "Unclustered")
            {
                HideAttachDetach = false;
            }
        }

        public Cluster Cluster
        {
            get { return _cluster; }
            set { _cluster = value; }
        }

        //The list of servers attached to the cluster
        public ObservableCollection<ServerViewModel> ServerList
        {
            get { return _serverViewModelList; }
            set { _serverViewModelList = value; OnPropertyChanged("ClusterList"); }
        }

        public ObservableCollection<ServerViewModel> FilteredServers
        {
            get
            {
                if (_listToFilter == null)
                {
                    _listToFilter = new ObservableCollection<ServerViewModel>();
                    foreach (var server in DataAccessLayer.GetAllServers())
                    {
                        if (server.ClusterID != _cluster.ClusterID && _cluster.ClusterID != 0)
                        {
                            var serverVm = new ServerViewModel(server);
                            _listToFilter.Add(serverVm);
                        }                        
                    }
                    _filteredList = _listToFilter;
                }
                return _filteredList;
            }
            set { _filteredList = value; OnPropertyChanged("FilteredServers"); } 
        }

        public ObservableCollection<ServerViewModel> AddFilteredServers
        {
            get
            {
                if (_listToFilter == null)
                {
                    _listToFilter = new ObservableCollection<ServerViewModel>();
                    foreach (var server in DataAccessLayer.GetAllServers())
                    {
                        if (server.ClusterID == 0)
                        {
                            var serverVm = new ServerViewModel(server);
                            _listToFilter.Add(serverVm);
                        }                            
                    }
                    _filteredList = _listToFilter;
                }
                return _filteredList;
            }
            set { _filteredList = value; OnPropertyChanged("FilteredServers"); }
        }

        public IList<ServerViewModel> ServerListToLoad
        {
            private get { return _serverListToLoad; }
            set { _serverListToLoad = value; OnPropertyChanged("ClusterList"); }
        }

        public int ID
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
            if (ValidateSave())
            {
                if (_cluster.ClusterID == 0)
                {
                    DataAccessLayer.InsertCluster(_cluster);
                }
                else
                {
                    DataAccessLayer.UpdateCluster(_cluster);
                }
                foreach (var serverVm in _serverViewModelList)
                {
                    serverVm.Server.ClusterID = _cluster.ClusterID;
                    DataAccessLayer.UpdateServer(serverVm.Server);
                }
                SendMessage(ViewModelMediator.Instance, this);
                var handler = SaveFinished;

                if (handler != null)
                {
                    SaveFinished(this, new EventArgs());
                }
                
            }    
        }

        private bool ValidateSave()
        {
            if (String.IsNullOrWhiteSpace(Name))
            {
                ErrorMessage = "Name cannot be empty.";
                var handler = ErrorOccurred;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
                if (Cluster.ClusterID != 0)
                {
                    GetMemento();
                }
                return false;
            }
            try
            {
                var tempClusterList = DataAccessLayer.GetAllClusters();
                var filterCount = tempClusterList.Count(tempCluster => _cluster.Name == tempCluster.Name && 
                                  _cluster.Name != "Unclustered" && _cluster.ClusterID != tempCluster.ClusterID);
                if (filterCount != 0)
                {
                    ErrorMessage = String.Format("A Cluster named {0} already exists. Please enter a unique name.", Cluster.Name);
                    var handler = ErrorOccurred;

                    if (handler != null)
                    {
                        handler(this, new EventArgs());
                    }
                    if (Cluster.ClusterID != 0)
                    {
                        GetMemento();
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while saving. " + ex.Message;
                var handler = ErrorOccurred;

                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
                if (Cluster.ClusterID != 0)
                {
                    GetMemento();
                }
                return false;
            }
            return true;
        }

        private void GetMemento()
        {
            try
            {
                Cluster = DataAccessLayer.GetCluster(Cluster.ClusterID);
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while restoring data. " + ex.Message;
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
            if (!ServerList.Contains(SelectedServer))
            {
                ServerList.Add(SelectedServer);
            }
            if (_filteredList.Contains(SelectedServer))
            {
                _listToFilter.Remove(SelectedServer);
                FilteredServers.Remove(SelectedServer);
            }
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
                foreach (var serverVm in _serverViewModelList)
                {
                    serverVm.Server.ClusterID = 0;
                    DataAccessLayer.UpdateServer(serverVm.Server);
                }

                DataAccessLayer.DeleteCluster(ID);
                ID = 0;
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

        private void Detach()
        {
            if (!_filteredList.Contains(SelectedServer))
            {
                _listToFilter.Add(SelectedServer);
                FilteredServers.Add(SelectedServer);
            }
            if (ServerList.Contains(SelectedServer))
            {
                ServerList.Remove(SelectedServer);
            }            
        }

        public void ReceiveMessage(object message)
        {
            if (message != null)
            {
                if (message.GetType() == typeof(string))
                {
                    string tempMessage = message as string;
                    if (tempMessage == LOAD_COMPLETE_MESSAGE)
                    {
                        //null ref err on config import
                        foreach (var serverVM in _serverListToLoad)
                        {
                            _serverViewModelList.Add(serverVM);
                        }
                    }
                }
            }
        }

        public void SendMessage(IMediator mediator, object message)
        {
            mediator.DistributeMessage(this, message);
        }

        public void FilterChanged()
        {
            if (ShowAll == true)
            {
                FilteredServers = _listToFilter;
            }
            if (ShowPhysical == true)
            {
                FilteredServers = new ObservableCollection<ServerViewModel>(_listToFilter.Where(server => server.ServerType == (int)ServerType.Physical));
            }
            if (ShowVirtual == true)
            {
                FilteredServers = new ObservableCollection<ServerViewModel>(_listToFilter.Where(server => server.ServerType == (int)ServerType.Virtual));
            }
            if (ShowUnknown == true)
            {
                FilteredServers = new ObservableCollection<ServerViewModel>(_listToFilter.Where(server => server.ServerType == (int)ServerType.Unknown));
            }
            if (!String.IsNullOrWhiteSpace(SearchText))
            {
                var tempFilter = FilteredServers.Where(server => server.Address.Contains(SearchText) || server.Hostname.Contains(SearchText));
                FilteredServers = new ObservableCollection<ServerViewModel>(tempFilter);
            }
        }
    }
}
