using Npgsql;
using System;

namespace Spikemoss.DataAccessLayer
{
    class PostgreSQL : IDataAccessLayer
    {
        private string _connectionString;
        private SimpleAES _aes;
        private static PostgreSQL _instance;
        private const string DATABASE_NAME = "spikemoss";

        //Used internally to ensure thready safety by locking a private object
        private static readonly object padlock = new object();

        public static PostgreSQL Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new PostgreSQL();
                    }
                    return _instance;
                }
            }
        }

        private PostgreSQL()
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
                return DataAccessLayerType.PostgreSQL;
            }
        }


        public void CreateDatabase()
        {
            using (var con = new NpgsqlConnection(_connectionString))
            {
                con.Open();
                //SQL injection vulnerability
                var cmd = new NpgsqlCommand("CREATE DATABASE " + DATABASE_NAME, con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            //Edit connection string to enable connections to newly created database
            var constr = new NpgsqlConnectionStringBuilder();

            constr.ConnectionString = _connectionString;
            constr.Database = DATABASE_NAME;
            _connectionString = constr.ConnectionString;
        }

        public void CreateTables()
        {
            throw new NotImplementedException();
        }
    }
}
