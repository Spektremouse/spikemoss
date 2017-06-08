namespace Spikemoss.Models
{
    public class Hardware
    {
        #region Members       
        private int _socketsCapacityPhysical = 0;
        private int _socketsPopulatedPhysical = 0;
        private int _totalLogicalCores = 0;
        private int _totalPhysicalCores = 0;
        private string _machineName;
        private string _manufacturer;
        private string _model;
        private string _operatingSystemName;
        private string _operatingSystemVersion;
        private string _processorID;
        private string _processorSpeed;
        #endregion

        #region Accessors and Mutators
        public int SocketsCapacityPhysical
        {
            get
            {
                return _socketsCapacityPhysical;
            }

            set
            {
                _socketsCapacityPhysical = value;
            }
        }

        public int SocketsPopulatedPhysical
        {
            get
            {
                return _socketsPopulatedPhysical;
            }

            set
            {
                _socketsPopulatedPhysical = value;
            }
        }

        public int TotalLogicalCores
        {
            get
            {
                return _totalLogicalCores;
            }

            set
            {
                _totalLogicalCores = value;
            }
        }

        public int TotalPhysicalCores
        {
            get
            {
                return _totalPhysicalCores;
            }

            set
            {
                _totalPhysicalCores = value;
            }
        }

        public string MachineName
        {
            get
            {
                return _machineName;
            }

            set
            {
                _machineName = value;
            }
        }

        public string Manufacturer
        {
            get
            {
                return _manufacturer;
            }

            set
            {
                _manufacturer = value;
            }
        }

        public string Model
        {
            get
            {
                return _model;
            }

            set
            {
                _model = value;
            }
        }

        public string OperatingSystemName
        {
            get
            {
                return _operatingSystemName;
            }

            set
            {
                _operatingSystemName = value;
            }
        }

        public string OperatingSystemVersion
        {
            get
            {
                return _operatingSystemVersion;
            }

            set
            {
                _operatingSystemVersion = value;
            }
        }

        public string ProcessorIdentifier
        {
            get
            {
                return _processorID;
            }

            set
            {
                _processorID = value;
            }
        }

        public string ProcessorSpeed
        {
            get
            {
                return _processorSpeed;
            }

            set
            {
                _processorSpeed = value;
            }
        }
        #endregion

        #region Methods
        #endregion
    }
}
