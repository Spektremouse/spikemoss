using Npgsql;
using System;
using Spikemoss.Models;
using System.Collections.Generic;

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

        public List<Server> GetAllServers()
        {
            throw new NotImplementedException();
        }

        public void DeleteServer(int id)
        {
            throw new NotImplementedException();
        }

        public Server GetServer(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertServer(Server server)
        {
            throw new NotImplementedException();
        }

        public void UpdateServer(Server server)
        {
            throw new NotImplementedException();
        }

        public List<Cluster> GetAllClusters()
        {
            throw new NotImplementedException();
        }        

        public void DeleteCluster(int id)
        {
            throw new NotImplementedException();
        }

        public Cluster GetCluster(int id)
        {
            throw new NotImplementedException();
        }

        public List<Server> GetClusterServers(Cluster cluster) { throw new NotImplementedException(); }

        public void InsertCluster(Cluster Cluster)
        {
            throw new NotImplementedException();
        }

        public void UpdateCluster(Cluster Cluster)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public User GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public void InsertUser(User user)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
