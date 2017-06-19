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
    /// Interaction logic for MSSQLView.xaml
    /// </summary>
    public partial class MSSQLView : UserControl
    {
        private MSSQLViewModel _viewModel;  

        public MSSQLView()
        {
            InitializeComponent();
            _viewModel = this.DataContext as MSSQLViewModel;

            //Potential Memory Leak
            //WeakEventManager<IReportErrors, EventArgs>.AddHandler(_viewModel, nameof(_viewModel.ErrorOccurred), OnErrorOccurred);
            _viewModel.ErrorOccurred += OnErrorOccurred;
        }

        private void OnErrorOccurred(object sender, EventArgs e)
        {
            ErrorWindow win = new ErrorWindow(_viewModel);
            win.ShowDialog();
        }

        private void TestClick(object sender, RoutedEventArgs e)
        {
            ProgressWindow win = new ProgressWindow(_viewModel, _viewModel.TestConnection);
            win.ShowDialog();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            ProgressWindow win = new ProgressWindow(_viewModel, _viewModel.TestConnection);
            win.ShowDialog();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                _viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.ErrorOccurred -= OnErrorOccurred;
        }
    }
}
