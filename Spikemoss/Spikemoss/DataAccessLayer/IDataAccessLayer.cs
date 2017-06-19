namespace Spikemoss.DataAccessLayer
{
    interface IDataAccessLayer
    {
        DataAccessLayerType IdalType { get; }
        string ConnectionString { get; set; }
        void CreateDatabase();
        void CreateTables();
    }
}
