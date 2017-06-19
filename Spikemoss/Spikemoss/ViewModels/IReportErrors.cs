using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels
{
    public delegate void ErrorHandler(object sender, EventArgs e);

    public interface IReportErrors
    {
        string ErrorMessage { get; set; }
        event ErrorHandler ErrorOccurred;
    }
}
