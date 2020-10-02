using System;

namespace Interfaces
{
    public enum MemoryRegion
    {
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

    public interface IMemory<T> where T : struct
    {
        public MemoryRegion Region { get; }
        public T Data { get; }
        public ushort Address { get; }
    }

    public abstract class Memory<T> : IMemory<T> where T : struct
    {
        public MemoryRegion Region { get; }
        public T Data { get; }
        public ushort Address { get; }

        protected Memory(MemoryRegion region, T data, ushort address)
        {
            Region = region;
            Data = data;
            Address = address;
        }
    }
}