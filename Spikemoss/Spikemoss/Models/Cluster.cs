using System.Collections.Generic;

namespace Spikemoss.Models
{
    public class Cluster
    {
        #region Members
        private int _clusterID = 0;
        private string _name = "";
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
        #endregion
    }
}
