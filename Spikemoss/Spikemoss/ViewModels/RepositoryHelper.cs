using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spikemoss.DataAccessLayer;
using Spikemoss.Models;
using System.Collections.ObjectModel;

namespace Spikemoss.ViewModels
{
    public class RepositoryHelper
    {
        private const string UNCLUSTERED_NAME = "Unclusterd";
        private const string ERR_EMPTY_CLUSTER_NAME = "Cluster name cannot be empty.";
        private const string ERR_DUPLICATE_CLUSTER_NAME = "A Cluster named {0} already exists. Please enter a unique name.";
        private static RepositoryHelper _instance;
        private IDataAccessLayer _idal;

        private ObservableCollection<ClusterViewModel> _clusterList = new ObservableCollection<ClusterViewModel>();
        private ObservableCollection<ServerViewModel> _serverList = new ObservableCollection<ServerViewModel>();
        //private ObservableCollection<User> _userList = new ObservableCollection<User>();
        //private ObservableCollection<Hardware> _hardwareList = new ObservableCollection<Hardware>();

        //Used internally to ensure thready safety by locking a private object
        private static readonly object padlock = new object();

        public static RepositoryHelper Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new RepositoryHelper();
                    }
                    return _instance;
                }
            }
        }

        private RepositoryHelper()
        {
            DataAccessLayerFactory factory = new DataAccessLayerFactory();
            _idal = factory.CreateDataAccessLayer((DataAccessLayerType)Properties.Settings.Default.DataAccessLayerType,
                Properties.Settings.Default.ConnectionString);
        }

        public ObservableCollection<ClusterViewModel> Clusters
        {
            get { return _clusterList; }
        }

        public ObservableCollection<ServerViewModel> Servers
        {
            get { return _serverList; }
        }

        public void Load()
        {
            _clusterList.Clear();
            _serverList.Clear();
            LoadClusters();            
            LoadServers();
            LoadClusterServerList();
        }

        public void Save(ClusterViewModel clusterVm)
        {
            if (Validate(clusterVm))
            {
                var cluster = new Cluster();
                if (clusterVm.ClusterID == 0 && clusterVm.Name != UNCLUSTERED_NAME)
                {
                    cluster.Name = clusterVm.Name;
                    _idal.InsertCluster(cluster);
                    clusterVm = new ClusterViewModel(cluster);
                    _clusterList.Add(clusterVm);
                }
                else
                {
                    cluster.ClusterID = clusterVm.ClusterID;
                    cluster.Name = clusterVm.Name;
                    _idal.UpdateCluster(cluster);
                }
            }            
        }

        public void Delete(ClusterViewModel clusterVm)
        {
            var unclustered = _clusterList.Single(tmpCluster => tmpCluster.ClusterID == 0);
            foreach (var serverVm in clusterVm.ServerList)
            {
                serverVm.Server.ClusterID = 0;
                _idal.UpdateServer(serverVm.Server);                
                unclustered.ServerList.Add(serverVm);
            }
            _idal.DeleteCluster(clusterVm.ClusterID);
            _clusterList.Remove(clusterVm);
        }

        public void Save(ServerViewModel serverVm)
        {
            if (Validate(serverVm))
            {
                var server = new Server();
                server.ServerID = serverVm.ServerID;
                server.ClusterID = serverVm.ClusterID;
                server.Address = serverVm.Address;
                server.Hostname = serverVm.Hostname;
                server.ServerType = (ServerType)serverVm.ServerType;

                if (serverVm.ServerID == 0)
                {
                    _idal.InsertServer(server);
                    serverVm = new ServerViewModel(server);
                    _serverList.Add(serverVm);
                }
                else
                {
                    _idal.UpdateServer(server);
                }                
            }
        }

        public void InsertUser(User user)
        {
            _idal.InsertUser(user);
            var userVm = new UserViewModel(user);
        }

        private void LoadClusters()
        {
            var unclustered = new Cluster();
            unclustered.ClusterID = 0;
            unclustered.Name = UNCLUSTERED_NAME;
            
            var unclusteredVm = new ClusterViewModel(unclustered);
            _clusterList.Add(unclusteredVm);

            foreach (var cluster in _idal.GetAllClusters())
            {
                var clusterVm = new ClusterViewModel(cluster);
                _clusterList.Add(clusterVm);
            }
        }

        private void LoadClusterServerList()
        {
            foreach (var clusterVm in _clusterList)
            {
                clusterVm.ServerList = new ObservableCollection<ServerViewModel>(
                    _serverList.Where(serverVm => serverVm.ClusterID == clusterVm.ClusterID));
            }
        }

        private void LoadServers()
        {
            foreach (var server in _idal.GetAllServers())
            {
                var serverVm = new ServerViewModel(server);
                _serverList.Add(serverVm);
            }
        }

        private void LoadServerVirtualGuests() { }

        private void LoadUsers() { }

        private void LoadHardware() { }

        private bool Validate(ClusterViewModel clusterVm)
        {
            if (String.IsNullOrWhiteSpace(clusterVm.Name))
            {                
                if (clusterVm.ClusterID != 0)
                {
                    GetMemento(clusterVm);
                }
                throw new ArgumentException(ERR_EMPTY_CLUSTER_NAME);
            }

            var filterCount = _clusterList.Count(tempCluster => clusterVm.Name == tempCluster.Name &&
                                  clusterVm.Name != "Unclustered" && clusterVm.ClusterID != tempCluster.ClusterID);
            if (filterCount != 0)
            {
                if (clusterVm.ClusterID != 0)
                {
                    GetMemento(clusterVm);
                }
                throw new ArgumentException(String.Format(ERR_DUPLICATE_CLUSTER_NAME, clusterVm.Name));
            }
            return true;
        }

        private bool Validate(ServerViewModel serverVm) { return true; }

        private void GetMemento(ClusterViewModel clusterVm) { }
    }
}
