using Spikemoss.Models.Gatherers;
using Spikemoss.Models;
using System.ComponentModel;

namespace Spikemoss.ViewModels
{
    class BaseViewModelINotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
