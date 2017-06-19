using System;
using System.Collections.Generic;
using Spikemoss.DataAccessLayer;

namespace Spikemoss.ViewModels
{
    class ConfigurationViewModel : BaseViewModel
    {
        private BaseViewModel _cview;
        private List<string> _databaseTypeList;
        private int _dbTypeIndex = 0;

        public ConfigurationViewModel()
        {
            _databaseTypeList = new List<string>();
            _databaseTypeList.Add("Select one...");
            foreach (var dbtype in Enum.GetValues(typeof(DataAccessLayerType)))
            {
                _databaseTypeList.Add(dbtype.ToString());
            }     
        }

        public BaseViewModel CurrentView
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

                switch (_dbTypeIndex)
                {
                    case 1:
                        CurrentView = new MSSQLViewModel();
                        break;
                    case 2:
                        CurrentView = new MySQLViewModel();
                        break;
                    case 3:
                        CurrentView = new OracleSQLViewModel();
                        break;
                    case 4:
                        CurrentView = new PostgreSQLViewModel();
                        break;
                    default:
                        CurrentView = null;
                        break;
                }
            }
        }
    }
}
