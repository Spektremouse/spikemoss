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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Spikemoss.Views
{
    /// <summary>
    /// Interaction logic for AddClusterView.xaml
    /// </summary>
    public partial class AddClusterView : UserControl
    {
        private ClusterViewModel _viewModel;
        private Window _parentWindow;

        public AddClusterView()
        {
            InitializeComponent();
            _viewModel = this.DataContext as ClusterViewModel;
            _viewModel.ErrorOccurred += OnErrorOccurred;
            _viewModel.SaveFinished += OnSaveFinished;
        }

        private void OnSaveFinished(object sender, EventArgs e)
        {
            _parentWindow = Window.GetWindow(this);
            if (_parentWindow.DataContext.GetType() != typeof(MainViewModel))
            {
                _parentWindow.Close();
            }            
        }

        private void OnErrorOccurred(object sender, EventArgs e)
        {
            var errorWindow = new ErrorWindow(_viewModel);
            errorWindow.ShowDialog();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            _parentWindow = Window.GetWindow(this);
            if (_parentWindow.DataContext.GetType() != typeof(MainViewModel))
            {
                _parentWindow.Close();
            }
        }
    }
}
