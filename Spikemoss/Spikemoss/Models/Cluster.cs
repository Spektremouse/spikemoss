namespace Spikemoss.Models
{
    class Cluster
    {
        #region Members
        private int _clusterID;
        private string _name;
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
