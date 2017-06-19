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
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        private ICommand mTask;

        public ProgressWindow(IReportProgress reporter, ICommand task)
        {
            InitializeComponent();
            this.DataContext = reporter;
            reporter.ProgressFinish += ProgressFinished;
            mTask = task;
        }

        private void ProgressFinished(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mTask.Execute(null);
        }
    }
}
