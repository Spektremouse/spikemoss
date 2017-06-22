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
    /// Interaction logic for AddClusterWindow.xaml
    /// </summary>
    public partial class AddClusterWindow : Window
    {
        private ClusterViewModel _viewModel;

        public AddClusterWindow()
        {
            InitializeComponent();
            _viewModel = this.DataContext as ClusterViewModel;
            _viewModel.ErrorOccurred += OnErrorOccurred;
            _viewModel.SaveFinished += OnSaveFinished;
        }

        private void OnSaveFinished(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnErrorOccurred(object sender, EventArgs e)
        {
            var errWin = new ErrorWindow(_viewModel);
            errWin.ShowDialog();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _viewModel.ErrorOccurred -= OnErrorOccurred;
            _viewModel.SaveFinished -= OnSaveFinished;
        }
    }
}
