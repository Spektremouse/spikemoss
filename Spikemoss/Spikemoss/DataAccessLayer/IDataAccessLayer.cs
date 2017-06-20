using Spikemoss.Models;
using System.Collections.Generic;

namespace Spikemoss.DataAccessLayer
{
    public interface IDataAccessLayer
    {
        DataAccessLayerType IdalType { get; }
        string ConnectionString { get; set; }
        void CreateDatabase();
        void CreateTables();

        List<Server> GetAllServers();

        void DeleteServer(int id);

        Server GetServer(int id);

        void InsertServer(Server server);

        void UpdateServer(Server server);

        List<Cluster> GetAllClusters();

        void DeleteCluster(int id);

        Cluster GetCluster(int id);

        List<Server> GetClusterServers(Cluster cluster);

        void InsertCluster(Cluster Cluster);

        void UpdateCluster(Cluster Cluster);

        List<User> GetAllUsers();

        void DeleteUser(int id);

        User GetUser(int id);

        void InsertUser(User user);

        void UpdateUser(User user);



    }
}
