using Spikemoss.ViewModels.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels
{
    public class MainViewModel : BaseViewModel, IParticipant
    {
        public event EventHandler RequestShow;

        public MainViewModel()
        {
            ViewModelMediator.Instance.Register(this);
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
    }
}
