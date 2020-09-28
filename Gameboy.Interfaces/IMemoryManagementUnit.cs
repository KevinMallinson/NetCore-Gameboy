using System;

namespace Gameboy.Interfaces
{
    public interface IMemoryManagementUnit
    {
        GBMemory GetByte(int address);
        GBMemory GetWord(int address);
        void SetByte(int address, int val);
        void SetWord(int address, int val);
        string Dump();
        string HexDump();
    }
}