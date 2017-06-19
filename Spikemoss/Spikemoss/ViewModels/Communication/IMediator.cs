using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels.Communication
{
    public interface IMediator
    {
        List<IParticipant> ParticipantList { get; }
        void DistributeMessage(IParticipant sender, object message);
        void DirectedMessage<T>(IParticipant sender, IParticipant receiver, object message);
        void Register(IParticipant participant);
        void Unregister(IParticipant participant);
    }
}
