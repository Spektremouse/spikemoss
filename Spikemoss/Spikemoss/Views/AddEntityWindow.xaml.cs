using Spikemoss.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Spikemoss.Views
{
    /// <summary>
    /// Interaction logic for AddEntityWindow.xaml
    /// </summary>
    public partial class AddEntityWindow : Window
    {
        private AddEntityViewModel _viewmodel;

        public AddEntityWindow(AddEntityViewModel viewmodel)
        {
            InitializeComponent();
            _viewmodel = viewmodel;
            this.DataContext = _viewmodel;
        }
    }
}
