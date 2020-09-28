namespace Gameboy.Interfaces
{
    public enum MemoryRegion
    {
        SET_ONLY,
        ROM_BANK,
        VIDEO_RAM,
        EXTERNAL_RAM,
        WORK_RAM,
        ECHO_RAM,
        SPRITE_ATTRIBUTE_TABLE,
        UNUSED,
        IO_REGISTERS,
        HIGH_RAM,
        INTERRUPT_FLAG
    }

    public class GBMemory
    {
        public bool IsByte { get; }
        public MemoryRegion Region { get; }

        public int Data { get; }
        public int Address { get; }

        public GBMemory()
        {
            Region = MemoryRegion.SET_ONLY;
            Data = 0;
            Address = 0;
            IsByte = false;
        }

        public GBMemory(MemoryRegion region, int data, int address)
        {
            Region = region;
            Data = data;
            Address = address;
            IsByte = (data >> 8 == 0);
        }
    }
}