using System.Net;

namespace Spikemoss.Models
{
    public class Server
    {
        #region Members
        private bool _isConfigured;
        private int _clusterID;
        private int _serverID;
        private int _sshPort;
        private int _virtualHostID;        
        private string _error;
        private string _hostname;
        private OperatingSystemType _operatingSystem;
        private ServerType _serverType;     
        private Hardware _hardware;
        private IPAddress _address;
        private User _user;
        #endregion

        #region Accessors and Mutators
        public bool IsConfigured
        {
            get { return _isConfigured; }
            set { _isConfigured = value; }
        }

        public int ClusterID
        {
            get { return _clusterID; }
            set { _clusterID = value; }
        }

        public int ServerID
        {
            get { return _serverID; }
            set { _serverID = value; }
        }

        public int VirtualHostID
        {
            get { return _virtualHostID; }
            set { _virtualHostID = value; }
        }

        public int SSHPort
        {
            get { return _sshPort; }
            set { _sshPort = value; } 
        }

        public string Address
        {
            get { return _address.ToString(); }
            set { _address = IPAddress.Parse(value); }
        }

        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public string Hostname
        {
            get { return _hostname; }
            set { _hostname = value; }
        }

        public OperatingSystemType OperatingSystem
        {
            get { return _operatingSystem; }
            set { _operatingSystem = value; }
        }

        public ServerType ServerType
        {
            get { return _serverType; }
            set { _serverType = value; }
        }

        public Hardware Hardware
        {
            get { return _hardware; }
            set { _hardware = value; }
        }

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }
        #endregion
    }
}
