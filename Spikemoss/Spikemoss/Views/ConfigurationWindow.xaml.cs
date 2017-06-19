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
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private ConfigurationViewModel _viewModel;

        public ConfigurationWindow()
        {
            InitializeComponent();
            _viewModel = this.DataContext as ConfigurationViewModel;
            _viewModel.RequestClose += OnConfigurationFinished;
        }

        private void OnConfigurationFinished(object sender, EventArgs e)
        {            
            this.Close();
        }
    }
}
