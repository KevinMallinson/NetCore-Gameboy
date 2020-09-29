using System;

namespace Gameboy.Hardware
{
    [Flags]
    public enum RegisterId
    {
        B = 0,
        C = 1,
        D = 2,
        E = 4,
        H = 8,
        L = 16,
        F = 32,
        A = 64,
        AF = 128,
        BC = 256,
        DE = 512,
        HL = 1024,
        
        // Used for instances where we want to access memory at address specified in the register
        FROM_RAM = 2048
    };
}