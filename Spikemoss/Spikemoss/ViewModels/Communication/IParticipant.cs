using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels.Communication
{
    public interface IParticipant
    {
        void SendMessage(IMediator mediator, object message);
        void ReceiveMessage(object message);
    }
}
