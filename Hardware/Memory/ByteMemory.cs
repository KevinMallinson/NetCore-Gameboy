using Interfaces;

namespace Hardware.Memory
{
    public class ByteMemory: Memory<byte>
    {
        public ByteMemory(MemoryRegion region, byte data, ushort address) : base(region, data, address)
        {
        }
    }
}