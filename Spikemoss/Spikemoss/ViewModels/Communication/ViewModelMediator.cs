using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels.Communication
{
    class ViewModelMediator : IMediator
    {
        private static ViewModelMediator _instance;
        private List<IParticipant> _participantList = new List<IParticipant>();

        //Used internally to ensure thready safety by locking a private object
        private static readonly object padlock = new object();

        public static ViewModelMediator Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new ViewModelMediator();
                    }
                    return _instance;
                }
            }
        }

        private ViewModelMediator() { }

        public List<IParticipant> ParticipantList
        {
            get
            {
                return _participantList;
            }
        }

        public void DistributeMessage(IParticipant sender, object message)
        {
            foreach (var participant in _participantList)
            {
                if (participant != sender)
                {
                    participant.ReceiveMessage(message);
                }
            }
        }

        public void Register(IParticipant participant)
        {
            if (!_participantList.Contains(participant))
            {
                _participantList.Add(participant);
            }            
        }

        public void Unregister(IParticipant participant)
        {
            if (_participantList.Contains(participant))
            {
                _participantList.Remove(participant);
            }            
        }

        public void DirectedMessage<T>(IParticipant sender, IParticipant receiver, object message)
        {
            throw new NotImplementedException();
        }
    }
}
