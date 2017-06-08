using System;

namespace Spikemoss.Models.Gatherers
{
    public class HardwareGathererFactory
    {
        public virtual IHardwareGatherer CreateHardwareGatherer(OperatingSystemType osType)
        {
            IHardwareGatherer gatherer = null;

            switch (osType)
            {
                case OperatingSystemType.Windows:
                    gatherer = new WindowsGatherer();
                    break;
                case OperatingSystemType.Linux:
                    gatherer = new LinuxGatherer();
                    break;
                default :
                    throw new ArgumentException("Operating System type currently not supported.");
            }

            return gatherer;
        }
    }
}
