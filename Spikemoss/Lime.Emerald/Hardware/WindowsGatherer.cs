using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lime.Emerald.Hardware
{
    class WindowsGatherer : IHardwareGatherer
    {
        public void GatherHardwareData()
        {
            Console.WriteLine("Gathering Windows hardware data!");
        }
    }
}
