using System;

namespace Interfaces
{
    public interface IMemoryManagementUnit
    {
        IMemory<byte> GetByte(ushort address);
        void SetByte(ushort address, byte val);
        IMemory<ushort> GetWord(ushort address);
        void SetWord(ushort address, ushort val);
    }
}