using Spikemoss.ViewModels.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spikemoss.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Spikemoss.ViewModels
{
    public class MainViewModel : DataViewModel, IParticipant, IReportErrors
    {
        private const string LOAD_COMPLETE_MESSAGE = "LoadComplete";

        private BaseViewModel _cview;
        private string _errorMessage;
        private object _selectedItem;

        private BackgroundWorker _worker;
        private ObservableCollection<ClusterViewModel> _clusterViewModelList;
        private IList<ClusterViewModel> _clusterList = new List<ClusterViewModel>();

        public event EventHandler RequestShow;
        public event ErrorHandler ErrorOccurred;
       
        public MainViewModel()
        {
            ViewModelMediator.Instance.Register(this);

            _clusterViewModelList = new ObservableCollection<ClusterViewModel>();

            _worker = new BackgroundWorker();
            _worker.DoWork += LoadDataWork;
            _worker.RunWorkerCompleted += LoadDataWorkCompleted;
            _worker.WorkerReportsProgress = true;
            _worker.ProgressChanged += ProgressChanged;
            _worker.WorkerSupportsCancellation = true;
        }

        public BaseViewModel CurrentView
        {
            get { return _cview; }
            set { _cview = value; OnPropertyChanged("CurrentView"); }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {            
            //throw new NotImplementedException();
        }

        private void LoadDataWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ErrorOccurred(this, new EventArgs());
            }
            foreach (var clustervm in _clusterList)
            {
                _clusterViewModelList.Add(clustervm);
            }
            SendMessage(ViewModelMediator.Instance, LOAD_COMPLETE_MESSAGE);
        }

        private void LoadDataWork(object sender, DoWorkEventArgs e)
        {
            var unclustered = new Cluster();
            unclustered.ClusterID = 0;
            unclustered.Name = "Unclustered";

            var clusters = DataAccessLayer.GetAllClusters();
            clusters.Add(unclustered);
            var servers = DataAccessLayer.GetAllServers();
            var users = DataAccessLayer.GetAllUsers();

            var sortedClusters = clusters.OrderBy(c => c.ClusterID);

            foreach (var server in servers)
            {
                server.User = users.SingleOrDefault(user => user.UserID == server.User.UserID);
                Console.WriteLine(server.ClusterID);
            }

            foreach (var cluster in sortedClusters)
            {
                Console.WriteLine("ClusterID:"+cluster.ClusterID);
                cluster.ServerList = servers.Where(server => server.ClusterID == cluster.ClusterID);
                var clusterVm = new ClusterViewModel(cluster);
                _clusterList.Add(clusterVm);
            }            
        }

        public object SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); Console.WriteLine(_selectedItem.GetType()); }
        }

        public ObservableCollection<ClusterViewModel> ClusterList { get { return _clusterViewModelList; } set { _clusterViewModelList = value; OnPropertyChanged("ClusterList"); } }

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

        public void ReceiveMessage(object message)
        {
            if (message is DataLayerConfigurationViewModel)
            {
                RequestShow(this, new EventArgs());
            }
        }

        public void SendMessage(IMediator mediator, object message)
        {
            mediator.DistributeMessage(this, message);
        }

        public void OnViewReady(object sender, EventArgs e)
        {
            _worker.RunWorkerAsync();
        }
    }
}
