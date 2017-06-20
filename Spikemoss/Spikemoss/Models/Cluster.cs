using System.Collections.Generic;

namespace Spikemoss.Models
{
    public class Cluster
    {
        #region Members
        private int _clusterID;
        private string _name;
        private IEnumerable<Server> _serverList;
        #endregion

        #region Accessors and Mutators
        public int ClusterID
        {
            get
            {
                return _clusterID;
            }

            set
            {
                _clusterID = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        /*public IEnumerable<Server> ServerList
        {
            get { return _serverList; }
            set { _serverList = value; }
        }*/
        #endregion
    }
}
