using System;
using Gameboy.Interfaces;

namespace Gameboy.Hardware
{
    public static class Bus
    {
        public static IGraphicsProcessingUnit GPU { get; private set; }
        public static ICentralProcessingUnit CPU { get; private set; }
        public static IMemoryManagementUnit MMU { get; private set; }

        public static void Init(IGraphicsProcessingUnit gpu, ICentralProcessingUnit cpu, IMemoryManagementUnit mmu)
        {
            if (GPU != null)
                throw new Exception("GPU is already initialized.");

            if (CPU != null)
                throw new Exception("CPU is already initialized.");
            
            if (MMU != null)
                throw new Exception("MMU is already initialized.");
            
            GPU = gpu;
            CPU = cpu;
            MMU = mmu;
        }

        public static void Reset()
        {
            GPU = null;
            CPU = null;
            MMU = null;
        }
    }
}