﻿using System;

namespace Gameboy.Interfaces
{
    public interface IMemoryManagementUnit
    {
        GBMemory GetByte(ushort address);
        GBMemory GetWord(ushort address);
        void SetByte(ushort address, byte val);
        void SetWord(ushort address, ushort val);
        string Dump();
        string HexDump();
    }
}