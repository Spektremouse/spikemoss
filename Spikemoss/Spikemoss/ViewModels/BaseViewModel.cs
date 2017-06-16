using Spikemoss.Models.Gatherers;
using Spikemoss.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spikemoss.ViewModels
{
    class BaseViewModel
    {
        public Server SelectedServer { get; set; }

        private void CollectHardwareData()
        {
            IHardwareGatherer gatherer;
            HardwareGathererFactory factory = new HardwareGathererFactory();
            gatherer = factory.CreateHardwareGatherer(SelectedServer.OperatingSystemType);
            gatherer.GatherHardwareData(SelectedServer);
        }
    }
}
