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
        public MainWindow(MainViewModel _viewmodel)
        {
            InitializeComponent();

            this.DataContext = _viewmodel;
            _viewmodel.RequestShow += OnRequestShow;
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
        }
    }
}
