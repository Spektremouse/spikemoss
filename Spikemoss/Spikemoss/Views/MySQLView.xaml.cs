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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Spikemoss.Views
{
    /// <summary>
    /// Interaction logic for MySQLView.xaml
    /// </summary>
    public partial class MySQLView : UserControl
    {
        private MySQLViewModel _viewModel;

        public MySQLView()
        {
            InitializeComponent();
            _viewModel = this.DataContext as MySQLViewModel;
            _viewModel.ErrorOccurred += OnErrorOccurred;
        }

        private void PortPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void OnErrorOccurred(object sender, EventArgs e)
        {
            ErrorWindow win = new ErrorWindow(_viewModel);
            win.ShowDialog();
        }

        private void TestClick(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as MySQLViewModel;
            ProgressWindow win = new ProgressWindow(context, context.TestConnection);
            win.ShowDialog();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as MSSQLViewModel;
            ProgressWindow win = new ProgressWindow(context, context.TestConnection);
            win.ShowDialog();
        }
    }
}
