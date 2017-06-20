using Spikemoss.Models;
using Spikemoss.ViewModels.Communication;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels
{
    public class ClusterViewModel : BaseViewModel, IParticipant
    {
        private const string LOAD_COMPLETE_MESSAGE = "LoadComplete";

        private IList<ServerViewModel> _serverListToLoad;
        private Cluster _cluster;
        private ObservableCollection<ServerViewModel> _serverViewModelList;
        
        public Cluster CurrentCluster
        {
            private get { return _cluster; }
            set { _cluster = value; }
        }

        public ClusterViewModel()
        {
            ViewModelMediator.Instance.Register(this);
            _serverViewModelList = new ObservableCollection<ServerViewModel>();
        }

        public ObservableCollection<ServerViewModel> ServerList
        {
            get { return _serverViewModelList; }
            set { _serverViewModelList = value; OnPropertyChanged("ClusterList"); }
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

        public void ReceiveMessage(object message)
        {
            if (message.GetType() == typeof(string))
            {
                string tempMessage = message as string;
                if (tempMessage == LOAD_COMPLETE_MESSAGE)
                {
                    foreach (var serverVM in _serverListToLoad)
                    {
                        _serverViewModelList.Add(serverVM);
                    }
                }
            }
        }

        public void SendMessage(IMediator mediator, object message)
        {
            throw new NotImplementedException();
        }
    }
}
