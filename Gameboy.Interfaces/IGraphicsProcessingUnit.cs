namespace Gameboy.Interfaces
{
    public interface IGraphicsProcessingUnit
    {
        GBMemory GetByte(int absoluteAddress, MemoryRegion region);
        void SetByte(int absoluteAddress, int val, MemoryRegion region);
    }
}