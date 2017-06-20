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
            _server = server;
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

        public void ReceiveMessage(object message)
        {
            if (message.GetType() == typeof(string))
            {
                string tempMessage = message as string;
                if (tempMessage == LOAD_COMPLETE_MESSAGE)
                {
                    foreach (var server in _serverList)
                    {
                        var serverVM = new ServerViewModel(server);
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
