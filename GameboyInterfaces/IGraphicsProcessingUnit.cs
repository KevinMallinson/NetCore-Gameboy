namespace GameboyInterfaces
{
    public interface IGraphicsProcessingUnit
    {
        //OAM bitmask: addr & 0x00FF
        //VRAM bitmask: addr & 0x1FFF
        GBMemory GetByte(ushort absoluteAddress);
        void SetByte(ushort absoluteAddress, byte val);
    }
}