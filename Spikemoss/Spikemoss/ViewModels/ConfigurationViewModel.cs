using System;
using System.Collections.Generic;
using Spikemoss.DataAccessLayer;
using Spikemoss.ViewModels.Communication;

namespace Spikemoss.ViewModels
{
    class ConfigurationViewModel : BaseViewModel, IParticipant
    {
        private DataLayerConfigurationViewModel _cview;
        private List<string> _databaseTypeList;
        private int _dbTypeIndex = 0;

        public event EventHandler RequestClose;

        private MSSQLViewModel _mssqlvm = new MSSQLViewModel();
        private MySQLViewModel _mysqlvm = new MySQLViewModel();
        private OracleSQLViewModel _oraclevm = new OracleSQLViewModel();
        private PostgreSQLViewModel _postgrevm = new PostgreSQLViewModel();                            

        public ConfigurationViewModel()
        {
            ViewModelMediator.Instance.Register(this);

            _databaseTypeList = new List<string>();
            _databaseTypeList.Add("Select one...");
            foreach (var dbtype in Enum.GetValues(typeof(DataAccessLayerType)))
            {
                _databaseTypeList.Add(dbtype.ToString());
            }     
        }

        public DataLayerConfigurationViewModel CurrentView
        {
            get { return _cview; }
            set { _cview = value; OnPropertyChanged("CurrentView"); }
        }

        public List<string> DatabaseTypeList
        {
            get { return _databaseTypeList; }
        }

        public int SelectedDatabaseTypeIndex
        {
            get { return _dbTypeIndex; }
            set
            {
                if (_dbTypeIndex == value)
                {
                    return;
                }

                _dbTypeIndex = value;
                OnPropertyChanged("SelectedDatabaseTypeIndex");

                if (_dbTypeIndex > 0 && _dbTypeIndex < (_databaseTypeList.Count - 1))
                {
                    var idal = (DataAccessLayerType)_dbTypeIndex;
                    switch (idal)
                    {
                        case DataAccessLayerType.MSSQL:
                            CurrentView = _mssqlvm;
                            break;
                        case DataAccessLayerType.MySQL:
                            CurrentView = _mysqlvm;
                            break;
                        case DataAccessLayerType.OracleSQL:
                            CurrentView = _oraclevm;
                            break;
                        case DataAccessLayerType.PostgreSQL:
                            CurrentView = _postgrevm;
                            break;
                        default:
                            CurrentView = null;
                            break;
                    }
                }
                else
                {
                    CurrentView = null;
                }                                
            }
        }

        public void ReceiveMessage(object message)
        {
            if(message is DataLayerConfigurationViewModel)
            {
                RequestClose(this, new EventArgs());
            }
        }

        public void SendMessage(IMediator mediator, object message)
        {
            mediator.DistributeMessage(this, message);
        }
    }
}
