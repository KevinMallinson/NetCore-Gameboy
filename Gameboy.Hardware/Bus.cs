using Gameboy.Interfaces;

namespace Gameboy.Hardware
{
    public class Bus : IHardwareBus
    {
        private readonly IGraphicsProcessingUnit _gpu;

        public Bus(IGraphicsProcessingUnit gpu)
        {
            _gpu = gpu;
        }

        public IGraphicsProcessingUnit GetGPU() => _gpu;
    }
}