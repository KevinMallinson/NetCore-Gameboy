using System;

namespace GameboyInterfaces
{
    public interface IMemoryUnit
    {
        GBMemory GetByte(ushort address);
        GBMemory GetWord(ushort address);
        void SetByte(ushort address, byte val);
        void SetWord(ushort address, ushort val);
    }
}