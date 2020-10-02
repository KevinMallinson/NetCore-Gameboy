using Interfaces;
using Hardware.Graphics;
using Hardware.Memory;
using Hardware.Processor;

namespace Hardware.System
{
    public static class Bus
    {
        public static ICentralProcessingUnit CPU { get; private set; }
        public static IGraphicsProcessingUnit GPU { get; private set; }
        public static IMemoryManagementUnit MMU { get; private set; }

        public static void Init(ICentralProcessingUnit cpu, IGraphicsProcessingUnit gpu, IMemoryManagementUnit mmu)
        {
            CPU = cpu;
            GPU = gpu;
            MMU = mmu;
        }

        public static void Init()
        {
            CPU = new CPU();
            GPU = new GPU();
            MMU = new MMU();
        }

        private static void Reset()
        {
            GPU = null;
            CPU = null;
            MMU = null;
        }
    }
}