using Spikemoss.ViewModels.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spikemoss.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Spikemoss.ViewModels
{
    public class MainViewModel : DataViewModel, IParticipant, IReportErrors, IReportProgress
    {
        private const string LOAD_COMPLETE_MESSAGE = "LoadComplete";

        private string _errorMessage;
        private string _progressMessage;
        private int _progressValue;
        private object _selectedItem;

        private BackgroundWorker _loadWorker;
        private BackgroundWorker _configurationIOWorker;
        private ObservableCollection<ClusterViewModel> _clusterViewModelList;
        private IList<ClusterViewModel> _clusterList = new List<ClusterViewModel>();

        public event EventHandler RequestShow;
        public event ErrorHandler ErrorOccurred;
        public event ProgressFinishHandler ProgressFinish;

        public MainViewModel()
        {
            ViewModelMediator.Instance.Register(this);

            _clusterViewModelList = new ObservableCollection<ClusterViewModel>();

            _loadWorker = new BackgroundWorker();
            _loadWorker.DoWork += LoadDataWork;
            _loadWorker.RunWorkerCompleted += LoadDataWorkCompleted;
            _loadWorker.WorkerReportsProgress = true;
            _loadWorker.ProgressChanged += ProgressChanged;
            _loadWorker.WorkerSupportsCancellation = true;

            _configurationIOWorker = new BackgroundWorker();
            _configurationIOWorker.RunWorkerCompleted += ConfigurationIOWorkCompleted;
            _configurationIOWorker.WorkerReportsProgress = true;
            _configurationIOWorker.ProgressChanged += ProgressChanged;
            _configurationIOWorker.WorkerSupportsCancellation = true;
        }        

        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
                if (_selectedItem != null)
                {
                    if (_selectedItem.GetType() == typeof(ClusterViewModel))
                    {
                        _selectedItem = new ClusterViewModel(((ClusterViewModel)_selectedItem).Cluster);
                    }
                    else if (_selectedItem.GetType() == typeof(ServerViewModel))
                    {

                        _selectedItem = new ServerViewModel(((ServerViewModel)_selectedItem).Server);
                    }
                }                
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
                _errorMessage = value; OnPropertyChanged("ErrorMessage");
            }
        }

        public string Filepath
        {
            private get;
            set;
        }

        public ObservableCollection<ClusterViewModel> ClusterList
        {
            get { return _clusterViewModelList; }
            set { _clusterViewModelList = value; OnPropertyChanged("ClusterList"); }
        }

        public ICommand ExportConfigurationCommand
        {
            get
            {
                _configurationIOWorker.DoWork += ExportConfiguration;
                return new DelegateCommand(_configurationIOWorker.RunWorkerAsync);
            }
        }

        public ICommand ImportConfigurationCommand
        {
            get
            {
                _configurationIOWorker.DoWork += ImportConfiguration;
                return new DelegateCommand(_configurationIOWorker.RunWorkerAsync);
            }
        }

        public string ProgressMessage
        {
            get { return _progressMessage;}
            set { _progressMessage = value; OnPropertyChanged("ProgressMessage"); }
        }

        public int ProgressValue
        {
            get { return _progressValue; }
            set { _progressValue = value; OnPropertyChanged("ProgressValue"); }
        }

        public void ReceiveMessage(object message)
        {
            if (message is DataLayerConfigurationViewModel)
            {
                RequestShow(this, new EventArgs());
            }
            if (message.GetType() == typeof(ClusterViewModel))
            {
                var tempCluster = message as ClusterViewModel;
                if (ClusterList.Contains(tempCluster))
                {

                }
                else if (!ClusterList.Contains(tempCluster))
                {
                    foreach (var server in tempCluster.ServerList)
                    {
                        foreach (var cluster in ClusterList)
                        {
                            var tempList = new List<ServerViewModel>();
                            foreach (var serverDuplicate in cluster.ServerList)
                            {
                                if (server.Server.ServerID == serverDuplicate.Server.ServerID)
                                {
                                    tempList.Add(serverDuplicate);
                                }
                            }
                            foreach (var tempDuplicate in tempList)
                            {
                                cluster.ServerList.Remove(tempDuplicate);
                            }
                        }
                    }
                    ClusterList.Add((ClusterViewModel)message);
                }
            }
        }

        public void SendMessage(IMediator mediator, object message)
        {
            mediator.DistributeMessage(this, message);
        }

        public void OnViewReady(object sender, EventArgs e)
        {
            //Listens for an event that signals the ViewModel can begin processing.
            _loadWorker.RunWorkerAsync();
        }

        private void ExportConfiguration(object sender, DoWorkEventArgs e)
        {
            var manager = new ConfigurationIO();

            ProgressMessage = "Exporting configuration file.";

            var serverList = DataAccessLayer.GetAllServers();

            foreach (var server in serverList)
            {
                server.User = DataAccessLayer.GetUser(server.User.UserID);
            }

            manager.ExportConfiguration(Filepath, serverList);
        }

        private void ImportConfiguration(object sender, DoWorkEventArgs e)
        {
            var manager = new ConfigurationIO();

            manager.ImportConfiguration(Filepath);

            int i = 0;
            foreach (var server in manager.ServerList)
            {                
                ProgressMessage = String.Format("{0}/{1} servers imported.", i, manager.ServerList.Count);
                DataAccessLayer.InsertServer(server);
                DataAccessLayer.InsertUser(server.User);
                i++;
            }
        }

        private void ConfigurationIOWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ErrorMessage = e.Error.Message;
                ErrorOccurred(this, new EventArgs());
            }
            ProgressFinish(this, new EventArgs());
            _clusterViewModelList.Clear();
            _clusterList.Clear();
            _loadWorker.RunWorkerAsync();
        }

        private void LoadDataWork(object sender, DoWorkEventArgs e)
        {
            //Created a cluster for unsorted servers
            var unclustered = new Cluster();
            unclustered.ClusterID = 0;
            unclustered.Name = "Unclustered";

            //Load in all the data
            var clusters = DataAccessLayer.GetAllClusters();
            var servers = DataAccessLayer.GetAllServers();
            var users = DataAccessLayer.GetAllUsers();

            //Add the placeholder cluster
            clusters.Add(unclustered);

            //Find and assign each servers user
            foreach (var server in servers)
            {
                server.User = users.SingleOrDefault(user => user.UserID == server.User.UserID);
            }

            //Sort the loaded clusters
            var sortedClusters = clusters.OrderBy(c => c.ClusterID);

            foreach (var cluster in sortedClusters)
            {
                //Create a list of ServerViewModels which the ClusterViewModel will manage
                var serverViewModels = new List<ServerViewModel>();
                //Get a list of servers that belong to a cluster
                var clusterServers = servers.Where(server => server.ClusterID == cluster.ClusterID);

                foreach (var server in clusterServers)
                {
                    //Create the ViewModel that will manage each server instance
                    var serverViewModel = new ServerViewModel(server);
                    //Add the view model to the current clusters ServerViewModel list
                    serverViewModels.Add(serverViewModel);
                }
                var clusterVm = new ClusterViewModel(cluster);
                clusterVm.ServerListToLoad = serverViewModels;
                _clusterList.Add(clusterVm);
            }
        }

        private void LoadDataWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ErrorMessage = e.Error.Message;
                ErrorOccurred(this, new EventArgs());
            }
            foreach (var clustervm in _clusterList)
            {
                _clusterViewModelList.Add(clustervm);
            }
            //Lets everyone know the app is done loading data from the database
            SendMessage(ViewModelMediator.Instance, LOAD_COMPLETE_MESSAGE);
        }        

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
