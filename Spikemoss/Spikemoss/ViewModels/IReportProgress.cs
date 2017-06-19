using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spikemoss.ViewModels
{
    public delegate void ProgressStartHandler(object sender, EventArgs e);
    public delegate void ProgressFinishHandler(object sender, EventArgs e);

    public interface IReportProgress
    {
        string ProgressMessage { get; set; }
        int ProgressValue { get; set; }
        event ProgressFinishHandler ProgressFinish;
        //bool IsInderterminate { get: }
        //bool CanCancel { get; } 
    }
}
