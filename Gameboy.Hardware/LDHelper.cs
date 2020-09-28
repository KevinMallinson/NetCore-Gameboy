using System.Collections.Generic;

namespace Gameboy.Hardware
{
    public class LDMapping
    {
        public Register From { get; }
        public Register To { get; }

        public LDMapping(Register to, Register from)
        {
            From = from;
            To = to;
        }
    }
    
    public class LDHelper : Dictionary<int, LDMapping>
    {
        public LDHelper(RegisterDictionary registers)
        { 
            Add(0x40, new LDMapping(registers[RegisterId.B], registers[RegisterId.B]));
            Add(0x41, new LDMapping(registers[RegisterId.B], registers[RegisterId.C]));
            Add(0x42, new LDMapping(registers[RegisterId.B], registers[RegisterId.D]));
            Add(0x43, new LDMapping(registers[RegisterId.B], registers[RegisterId.E]));
            Add(0x44, new LDMapping(registers[RegisterId.B], registers[RegisterId.H]));
            Add(0x45, new LDMapping(registers[RegisterId.B], registers[RegisterId.L]));
            Add(0x46, new LDMapping(registers[RegisterId.B], registers[RegisterId.HL]));
            Add(0x47, new LDMapping(registers[RegisterId.B], registers[RegisterId.A]));
            Add(0x48, new LDMapping(registers[RegisterId.C], registers[RegisterId.B]));
            Add(0x49, new LDMapping(registers[RegisterId.C], registers[RegisterId.C]));
            Add(0x4A, new LDMapping(registers[RegisterId.C], registers[RegisterId.D]));
            Add(0x4B, new LDMapping(registers[RegisterId.C], registers[RegisterId.E]));
            Add(0x4C, new LDMapping(registers[RegisterId.C], registers[RegisterId.H]));
            Add(0x4D, new LDMapping(registers[RegisterId.C], registers[RegisterId.L]));
            Add(0x4E, new LDMapping(registers[RegisterId.C], registers[RegisterId.HL]));
            Add(0x4F, new LDMapping(registers[RegisterId.C], registers[RegisterId.A]));
            Add(0x50, new LDMapping(registers[RegisterId.D], registers[RegisterId.B]));
            Add(0x51, new LDMapping(registers[RegisterId.D], registers[RegisterId.C]));
            Add(0x52, new LDMapping(registers[RegisterId.D], registers[RegisterId.D]));
            Add(0x53, new LDMapping(registers[RegisterId.D], registers[RegisterId.E]));
            Add(0x54, new LDMapping(registers[RegisterId.D], registers[RegisterId.H]));
            Add(0x55, new LDMapping(registers[RegisterId.D], registers[RegisterId.L]));
            Add(0x56, new LDMapping(registers[RegisterId.D], registers[RegisterId.HL]));
            Add(0x57, new LDMapping(registers[RegisterId.D], registers[RegisterId.A]));
            Add(0x58, new LDMapping(registers[RegisterId.E], registers[RegisterId.B]));
            Add(0x59, new LDMapping(registers[RegisterId.E], registers[RegisterId.C]));
            Add(0x5A, new LDMapping(registers[RegisterId.E], registers[RegisterId.D]));
            Add(0x5B, new LDMapping(registers[RegisterId.E], registers[RegisterId.E]));
            Add(0x5C, new LDMapping(registers[RegisterId.E], registers[RegisterId.H]));
            Add(0x5D, new LDMapping(registers[RegisterId.E], registers[RegisterId.L]));
            Add(0x5E, new LDMapping(registers[RegisterId.E], registers[RegisterId.HL]));
            Add(0x5F, new LDMapping(registers[RegisterId.E], registers[RegisterId.A]));
            Add(0x60, new LDMapping(registers[RegisterId.H], registers[RegisterId.B]));
            Add(0x61, new LDMapping(registers[RegisterId.H], registers[RegisterId.C]));
            Add(0x62, new LDMapping(registers[RegisterId.H], registers[RegisterId.D]));
            Add(0x63, new LDMapping(registers[RegisterId.H], registers[RegisterId.E]));
            Add(0x64, new LDMapping(registers[RegisterId.H], registers[RegisterId.H]));
            Add(0x65, new LDMapping(registers[RegisterId.H], registers[RegisterId.L]));
            Add(0x66, new LDMapping(registers[RegisterId.H], registers[RegisterId.HL]));
            Add(0x67, new LDMapping(registers[RegisterId.H], registers[RegisterId.A]));
            Add(0x68, new LDMapping(registers[RegisterId.L], registers[RegisterId.B]));
            Add(0x69, new LDMapping(registers[RegisterId.L], registers[RegisterId.C]));
            Add(0x6A, new LDMapping(registers[RegisterId.L], registers[RegisterId.D]));
            Add(0x6B, new LDMapping(registers[RegisterId.L], registers[RegisterId.E]));
            Add(0x6C, new LDMapping(registers[RegisterId.L], registers[RegisterId.H]));
            Add(0x6D, new LDMapping(registers[RegisterId.L], registers[RegisterId.L]));
            Add(0x6E, new LDMapping(registers[RegisterId.L], registers[RegisterId.HL]));
            Add(0x6F, new LDMapping(registers[RegisterId.L], registers[RegisterId.A]));
            Add(0x70, new LDMapping(registers[RegisterId.HL], registers[RegisterId.B]));
            Add(0x71, new LDMapping(registers[RegisterId.HL], registers[RegisterId.C]));
            Add(0x72, new LDMapping(registers[RegisterId.HL], registers[RegisterId.D]));
            Add(0x73, new LDMapping(registers[RegisterId.HL], registers[RegisterId.E]));
            Add(0x74, new LDMapping(registers[RegisterId.HL], registers[RegisterId.H]));
            Add(0x75, new LDMapping(registers[RegisterId.HL], registers[RegisterId.L]));
            Add(0x77, new LDMapping(registers[RegisterId.HL], registers[RegisterId.A]));
            Add(0x78, new LDMapping(registers[RegisterId.A], registers[RegisterId.B]));
            Add(0x79, new LDMapping(registers[RegisterId.A], registers[RegisterId.C]));
            Add(0x7A, new LDMapping(registers[RegisterId.A], registers[RegisterId.D]));
            Add(0x7B, new LDMapping(registers[RegisterId.A], registers[RegisterId.E]));
            Add(0x7C, new LDMapping(registers[RegisterId.A], registers[RegisterId.H]));
            Add(0x7D, new LDMapping(registers[RegisterId.A], registers[RegisterId.L]));
            Add(0x7E, new LDMapping(registers[RegisterId.A], registers[RegisterId.HL]));
            Add(0x7F, new LDMapping(registers[RegisterId.A], registers[RegisterId.A]));
            
            // Add(0x02);
            // Add(0x12);
            // Add(0x22);
            // Add(0x32);
            //
            // Add(0x06);
            // Add(0x16);
            // Add(0x26);
            // Add(0x36);
            //
            // Add(0x0A);
            // Add(0x1A);
            // Add(0x2A);
            // Add(0x3A);
            //
            // Add(0x0E);
            // Add(0x1E);
            // Add(0x2E);
            // Add(0x3E);
            //
            // Add(0xE0);
            // Add(0xF0);
            //
            // Add(0xE2);
            // Add(0xF2);
            //
            // Add(0xEA);
            // Add(0xFA);
        }
    }
}