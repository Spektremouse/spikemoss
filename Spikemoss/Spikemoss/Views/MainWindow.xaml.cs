using Microsoft.Win32;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        public event EventHandler ViewReady;

        public MainWindow(MainViewModel viewmodel)
        {
            InitializeComponent();
            _viewModel = viewmodel;
            this.DataContext = _viewModel;
            _viewModel.RequestShow += OnRequestShow;
            _viewModel.ErrorOccurred += OnErrorOccurred;
            this.ViewReady += _viewModel.OnViewReady;
        }

        private void OnErrorOccurred(object sender, EventArgs e)
        {
            ErrorWindow win = new ErrorWindow(_viewModel);
            win.ShowDialog();
        }

        private void OnRequestShow(object sender, EventArgs e)
        {
            this.Show();
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.DataAccessLayerType == 0)
            {
                this.Hide();
                ConfigurationWindow configuration = new ConfigurationWindow();
                configuration.ShowDialog();
            }
            else
            {
                ViewReady(this, new EventArgs());
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //_viewModel.SelectedItem = e.NewValue as BaseViewModel;
            if (_viewModel != null)
            {
                var treeItem = sender as TreeView;
                _viewModel.SelectedItem = treeItem.SelectedItem;
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            //http://stackoverflow.com/questions/2820357/how-to-exit-a-wpf-app-programmatically
            //http://stackoverflow.com/questions/5682408/command-to-close-an-application-of-console
            //logging
            Application.Current.Shutdown();
        }

        private void ImportConfigurationClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "CSV files (*.csv) | *.csv";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            var result = ofd.ShowDialog();
            if (result == true && ofd.FileName != null)
            {
                _viewModel.Filepath = ofd.FileName;
                ProgressWindow win = new ProgressWindow(_viewModel, _viewModel.ImportConfigurationCommand);
                win.ShowDialog();
            }
        }

        private void ExportConfigurationClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "CSV files (*.csv) | *.csv";
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            var result = sfd.ShowDialog();
            if (result == true && sfd.FileName != null)
            {
                _viewModel.Filepath = sfd.FileName;
                ProgressWindow win = new ProgressWindow(_viewModel, _viewModel.ExportConfigurationCommand);
                win.ShowDialog();
            }
        }

        private void AddClusterClick(object sender, RoutedEventArgs e)
        {
            var addwindow = new AddClusterWindow();
            addwindow.ShowDialog();
        }
    }
}
