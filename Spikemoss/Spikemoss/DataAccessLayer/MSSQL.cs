using System;
using System.Data.SqlClient;

namespace Spikemoss.DataAccessLayer
{
    class MSSQL : IDataAccessLayer
    {
        private string _connectionString;
        private SimpleAES _aes;
        private static MSSQL _instance;
        private const string DATABASE_NAME = "spikemoss";

        //Used internally to ensure thready safety by locking a private object
        private static readonly object padlock = new object();

        public static MSSQL Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new MSSQL();
                    }
                    return _instance;
                }
            }
        }

        private MSSQL()
        {
            _aes = new SimpleAES();
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public DataAccessLayerType IdalType
        {
            get
            {
                return DataAccessLayerType.MSSQL;
            }
        }


        public void CreateDatabase()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                //SQL injection vulnerability
                var cmd = new SqlCommand("CREATE DATABASE " + DATABASE_NAME, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            //Edit connection string to enable connections to newly created database
            var constr = new SqlConnectionStringBuilder();

            constr.ConnectionString = _connectionString;
            constr.InitialCatalog = DATABASE_NAME;
            _connectionString = constr.ConnectionString;
        }

        public void CreateTables()
        {
            throw new NotImplementedException();
        }
    }
}
