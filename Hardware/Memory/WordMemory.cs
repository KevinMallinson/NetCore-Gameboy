using Interfaces;

namespace Hardware.Memory
{
    public class WordMemory : Memory<ushort>
    {
        public WordMemory(MemoryRegion region, ushort data, ushort address) : base(region, data, address)
        {
        }
    }
}