namespace Interfaces
{
    public interface IGraphicsProcessingUnit
    {
        IMemory<byte> GetByte(ushort address, MemoryRegion region);
        void SetByte(ushort address, byte val, MemoryRegion region);
    }
}