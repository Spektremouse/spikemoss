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
    public class ServerViewModel : BaseViewModel, IParticipant 
    {
        private const string LOAD_COMPLETE_MESSAGE = "LoadComplete";

        private Server _server;
        private List<Server> _serverList = new List<Server>();
        ObservableCollection<ServerViewModel> _serverViewModelList;

        public ServerViewModel(Server server)
        {
            ViewModelMediator.Instance.Register(this);
            _server = server;
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

        public void ReceiveMessage(object message)
        {
            if (message != null)
            {
                if (message.GetType() == typeof(string))
                {
                    string tempMessage = message as string;
                    if (tempMessage == LOAD_COMPLETE_MESSAGE)
                    {

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
