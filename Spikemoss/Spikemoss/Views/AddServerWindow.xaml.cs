using Spikemoss.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddServerWindow.xaml
    /// </summary>
    public partial class AddServerWindow : Window
    {
        private ServerViewModel _viewModel;

        public AddServerWindow(MainWindow owner)
        {
            InitializeComponent();
            this.Owner = owner;
            _viewModel = this.DataContext as ServerViewModel;
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

        private void Window_Closed(object sender, EventArgs e)
        {
            _viewModel.ErrorOccurred -= OnErrorOccurred;
            _viewModel.SaveFinished -= OnSaveFinished;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PortPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
