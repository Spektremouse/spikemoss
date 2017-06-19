using Spikemoss.DataAccessLayer;
using Spikemoss.Properties;

namespace Spikemoss.ViewModels
{
    class DataViewModel : BaseViewModel
    {
        private DataAccessLayerFactory factory = new DataAccessLayerFactory();

        public IDataAccessLayer DataAccessLayer
        {
            get
            {
                return factory.CreateDataAccessLayer((DataAccessLayerType)Settings.Default.DataAccessLayerType,
                    Settings.Default.ConnectionString);
            }
        }
    }
}
