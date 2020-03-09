namespace Gameboy.Interfaces
{
    public interface IGraphicsProcessingUnit
    {
        GBMemory GetByte(ushort absoluteAddress, MemoryRegion region);
        void SetByte(ushort absoluteAddress, byte val, MemoryRegion region);
    }
}