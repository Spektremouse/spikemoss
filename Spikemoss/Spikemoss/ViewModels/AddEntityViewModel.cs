using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spikemoss.Models;
using Spikemoss.ViewModels.Communication;

namespace Spikemoss.ViewModels
{
    public class AddEntityViewModel : DataViewModel, IParticipant
    {
        private int _entityIndex = 0;
        private BaseViewModel _currentEntity;
        private MainViewModel _mainVm;
        private ClusterViewModel _clusterVm;
        private ServerViewModel _serverVm;
        private UserViewModel _userVm;

        public AddEntityViewModel(MainViewModel mainViewModel)
        {
            ViewModelMediator.Instance.Register(this);
            _serverVm = new ServerViewModel(new Server());

            _clusterVm = new ClusterViewModel(new Cluster());
            _userVm = new UserViewModel();
            //_userVM.User = new User();

            _currentEntity = _clusterVm;
        }

        public int CurrentEntityIndex
        {
            get { return _entityIndex; }
            set
            {
                _entityIndex = value;
                switch (CurrentEntityIndex)
                {
                    case 1:
                        CurrentEntity = (ServerViewModel)_serverVm;
                        break;
                    case 2:
                        CurrentEntity = _userVm;
                        break;
                    default:
                        CurrentEntity = (ClusterViewModel)_clusterVm;
                        break;
                }
                OnPropertyChanged("CurrentEntityIndex");                
            }
        }

        public BaseViewModel CurrentEntity
        {
            get { return _currentEntity; }
            set { _currentEntity = value; OnPropertyChanged("CurrentEntity"); } 
        }

        public void ReceiveMessage(object message)
        {
            //throw new NotImplementedException();
        }

        public void SendMessage(IMediator mediator, object message)
        {
            //throw new NotImplementedException();
        }
    }
}
