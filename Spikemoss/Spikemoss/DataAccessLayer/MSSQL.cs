using Spikemoss.Models;
using System;
using System.Collections.Generic;
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

        public List<Server> GetAllServers()
        {
            var list = new List<Server>();
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM server;");
                using (SqlDataReader data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var server = new Server();
                        server.ServerID = data.GetInt32(data.GetOrdinal("ServerID"));
                        server.ClusterID = data.GetInt32(data.GetOrdinal("ClusterID"));
                        server.User.UserID = data.GetInt32(data.GetOrdinal("UserID"));
                        server.VirtualHostID = data.GetInt32(data.GetOrdinal("VirtualHostID"));
                        server.Address = data.GetString(data.GetOrdinal("Address"));
                        server.Hostname = data.GetString(data.GetOrdinal("Hostname"));
                        server.SSHPort = data.GetInt32(data.GetOrdinal("SSHPort"));
                        server.Error = data.GetString(data.GetOrdinal("Error"));
                        server.OperatingSystem = (OperatingSystemType)Enum.Parse(typeof(OperatingSystemType),
                            data.GetString(data.GetOrdinal("OperatingSystemType")));
                        server.ServerType = (ServerType)Enum.Parse(typeof(ServerType),
                            data.GetString(data.GetOrdinal("ServerType")));
                        server.IsConfigured = data.GetBoolean(data.GetOrdinal("IsConfigured"));
                        server.IsVirtual = data.GetBoolean(data.GetOrdinal("IsVirtual"));
                    }
                }
                con.Close();
            }
            return list;
        }

        public void DeleteServer(int id)
        {
        }

        public Server GetServer(int id)
        {
            return null;
        }

        public void InsertServer(Server server) { }

        public void UpdateServer(Server server)
        {
        }

        public List<Cluster> GetAllClusters()
        {
            var list = new List<Cluster>();
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM cluster;");
                using (SqlDataReader data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var cluster = new Cluster();
                        cluster.ClusterID = data.GetInt32(data.GetOrdinal("ClusterID"));
                        cluster.Name = data.GetString(data.GetOrdinal("Name"));
                        list.Add(cluster);
                    }
                }
                con.Close();
            }
            return list;
        }

        public void DeleteCluster(int id)
        {
        }

        public Cluster GetCluster(int id)
        {
            return null;
        }

        public List<Server> GetClusterServers(Cluster cluster) { throw new NotImplementedException(); }

        public void InsertCluster(Cluster cluster) { }

        public void UpdateCluster(Cluster cluster)
        {
        }

        public List<User> GetAllUsers()
        {
            var list = new List<User>();
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM user;");
                using (SqlDataReader data = cmd.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var user = new User();
                        user.UserID = data.GetInt32(data.GetOrdinal("UserID"));
                        user.Name = data.GetString(data.GetOrdinal("Name"));
                        user.Name = data.GetString(data.GetOrdinal("Password"));
                        list.Add(user);
                    }
                }
                con.Close();
            }
            return list;
        }

        public void DeleteUser(int id)
        {
        }

        public User GetUser(int id)
        {
            return null;
        }

        public void InsertUser(User user) { }

        public void UpdateUser(User user)
        {
        }
    }
}
