using Spikemoss.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels
{
    public class UserViewModel : BaseViewModel
    {
        private User _user;
        private bool _useExisting = false;

        public UserViewModel()
        {
            _user = new User();
        }

        public UserViewModel(User user)
        {
            _user = user;
        }

        public int UserID
        {
            get { return _user.UserID; }            
        }

        public string Name
        {
            get { return _user.Name; }
            set { _user.Name = value; OnPropertyChanged("Name"); }
        }

        public string Password
        {
            get { return _user.Password; }
            set { _user.Password = value; OnPropertyChanged("Password"); }
        }

        public bool UseExisting
        {
            get { return _useExisting; }
            set { _useExisting = value; OnPropertyChanged("UseExisting"); }
        }

        public ObservableCollection<UserViewModel> UserList
        {
            get { return RepositoryHelper.Instance.Users; }
        }
    }
}
