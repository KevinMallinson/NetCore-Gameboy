namespace GameboyInterfaces
{
    public enum RequestType
    {
        GPU,
        RAM,
        CPU
    }
    
    // The bus serves as a way to forward requests
    // for memory between cpu, gpu, ram, etc
    // It also allows us to easily mock.
    public interface IHardwareBus
    {
        IGraphicsProcessingUnit GetGPU();
    }
}