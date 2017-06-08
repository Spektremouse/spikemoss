namespace Spikemoss.Models
{
    public class User
    {
        #region Members
        private int _userID;
        private string _name;
        private string _password;
        #endregion

        #region Accessors and Mutators
        public int UserID
        {
            get
            {
                return _userID;
            }

            set
            {
                _userID = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }

            set
            {
                _password = value;
            }
        }
        #endregion
    }
}
