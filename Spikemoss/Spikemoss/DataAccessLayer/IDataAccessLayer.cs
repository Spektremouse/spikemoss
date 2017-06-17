namespace Spikemoss.DataAccessLayer
{
    interface IDataAccessLayer
    {
        string ConnectionString { get; set; }
        void CreateDatabase();
        void CreateTables();
    }
}
