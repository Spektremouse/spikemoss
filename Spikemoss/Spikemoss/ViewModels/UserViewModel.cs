using Spikemoss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels
{
    class UserViewModel : BaseViewModel
    {
        private User _user;
        
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
    }
}
