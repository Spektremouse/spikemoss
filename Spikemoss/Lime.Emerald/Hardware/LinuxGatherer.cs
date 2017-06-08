using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lime.Emerald.Hardware
{
    class LinuxGatherer : IHardwareGatherer
    {
        public void GatherHardwareData()
        {
            Console.WriteLine("Gathering Linux hardware data!");
        }
    }
}
