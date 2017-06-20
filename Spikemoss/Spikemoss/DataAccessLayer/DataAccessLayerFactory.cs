using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.DataAccessLayer
{
    class DataAccessLayerFactory
    {
        public virtual IDataAccessLayer CreateDataAccessLayer(DataAccessLayerType idalType, string connectionString)
        {
            IDataAccessLayer idal = null;

            switch (idalType)
            {
                case DataAccessLayerType.MySQL:
                    idal = MySQL.Instance;
                    break;
                case DataAccessLayerType.MSSQL:
                    idal = MSSQL.Instance;
                    break;
                default:
                    throw new ArgumentException("Database not supported.");
            }
            idal.ConnectionString = connectionString;
            return idal;
        }
    }
}
