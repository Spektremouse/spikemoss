namespace Spikemoss.Models.Gatherers
{
    public interface IHardwareGatherer
    {
        void GatherHardwareData(Server serverArgs);
        bool IsHardwareDataCollectable(Server serverArgs);
    }
}
