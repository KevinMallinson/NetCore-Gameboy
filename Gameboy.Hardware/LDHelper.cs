using System.Collections.Generic;

namespace Gameboy.Hardware
{
    public enum LDParameter
    {
        NONE,
        BYTE,
        SHORT
    }

    public class LDMapping
    {
        public Register From { get; }
        public Register To { get; }
        public int Cycles { get; }
        public int IncrementValue { get; }
        public LDParameter Parameter { get; }
        public string Opcode { get; }

        public LDMapping(Register to, Register from, string opcode, int cycles, int incrementValue = 0,
            LDParameter parameter = LDParameter.NONE)
        {
            From = from;
            To = to;
            Cycles = cycles;
            IncrementValue = incrementValue;
            Opcode = opcode;
            Parameter = parameter;
        }
    }

    public class LDHelper : Dictionary<int, LDMapping>
    {
        public LDHelper(IOHelper registers)
        {
            Add(0x40, new LDMapping(registers[RegisterId.B], registers[RegisterId.B], "LD B,B", 4));
            Add(0x41, new LDMapping(registers[RegisterId.B], registers[RegisterId.C], "LD B,C", 4));
            Add(0x42, new LDMapping(registers[RegisterId.B], registers[RegisterId.D], "LD B,D", 4));
            Add(0x43, new LDMapping(registers[RegisterId.B], registers[RegisterId.E], "LD B,E", 4));
            Add(0x44, new LDMapping(registers[RegisterId.B], registers[RegisterId.H], "LD B,H", 4));
            Add(0x45, new LDMapping(registers[RegisterId.B], registers[RegisterId.L], "LD B,L", 4));
            Add(0x46, new LDMapping(registers[RegisterId.B], registers[RegisterId.HL], "LD B,(HL)", 8));
            Add(0x47, new LDMapping(registers[RegisterId.B], registers[RegisterId.A], "LD B,A", 4));
            Add(0x48, new LDMapping(registers[RegisterId.C], registers[RegisterId.B], "LD C,B", 4));
            Add(0x49, new LDMapping(registers[RegisterId.C], registers[RegisterId.C], "LD C,C", 4));
            Add(0x4A, new LDMapping(registers[RegisterId.C], registers[RegisterId.D], "LD C,D", 4));
            Add(0x4B, new LDMapping(registers[RegisterId.C], registers[RegisterId.E], "LD C,E", 4));
            Add(0x4C, new LDMapping(registers[RegisterId.C], registers[RegisterId.H], "LD C,H", 4));
            Add(0x4D, new LDMapping(registers[RegisterId.C], registers[RegisterId.L], "LD C,L", 4));
            Add(0x4E, new LDMapping(registers[RegisterId.C], registers[RegisterId.HL], "LD C,(HL)", 8));
            Add(0x4F, new LDMapping(registers[RegisterId.C], registers[RegisterId.A], "LD C,A", 4));
            Add(0x50, new LDMapping(registers[RegisterId.D], registers[RegisterId.B], "LD D,B", 4));
            Add(0x51, new LDMapping(registers[RegisterId.D], registers[RegisterId.C], "LD D,C", 4));
            Add(0x52, new LDMapping(registers[RegisterId.D], registers[RegisterId.D], "LD D,D", 4));
            Add(0x53, new LDMapping(registers[RegisterId.D], registers[RegisterId.E], "LD D,E", 4));
            Add(0x54, new LDMapping(registers[RegisterId.D], registers[RegisterId.H], "LD D,H", 4));
            Add(0x55, new LDMapping(registers[RegisterId.D], registers[RegisterId.L], "LD D,L", 4));
            Add(0x56, new LDMapping(registers[RegisterId.D], registers[RegisterId.HL], "LD D,(HL)", 8));
            Add(0x57, new LDMapping(registers[RegisterId.D], registers[RegisterId.A], "LD D,A", 4));
            Add(0x58, new LDMapping(registers[RegisterId.E], registers[RegisterId.B], "LD E,B", 4));
            Add(0x59, new LDMapping(registers[RegisterId.E], registers[RegisterId.C], "LD E,C", 4));
            Add(0x5A, new LDMapping(registers[RegisterId.E], registers[RegisterId.D], "LD E,D", 4));
            Add(0x5B, new LDMapping(registers[RegisterId.E], registers[RegisterId.E], "LD E,E", 4));
            Add(0x5C, new LDMapping(registers[RegisterId.E], registers[RegisterId.H], "LD E,H", 4));
            Add(0x5D, new LDMapping(registers[RegisterId.E], registers[RegisterId.L], "LD E,L", 4));
            Add(0x5E, new LDMapping(registers[RegisterId.E], registers[RegisterId.HL], "LD E,(HL)", 8));
            Add(0x5F, new LDMapping(registers[RegisterId.E], registers[RegisterId.A], "LD E,A", 4));
            Add(0x60, new LDMapping(registers[RegisterId.H], registers[RegisterId.B], "LD H,B", 4));
            Add(0x61, new LDMapping(registers[RegisterId.H], registers[RegisterId.C], "LD H,C", 4));
            Add(0x62, new LDMapping(registers[RegisterId.H], registers[RegisterId.D], "LD H,D", 4));
            Add(0x63, new LDMapping(registers[RegisterId.H], registers[RegisterId.E], "LD H,E", 4));
            Add(0x64, new LDMapping(registers[RegisterId.H], registers[RegisterId.H], "LD H,H", 4));
            Add(0x65, new LDMapping(registers[RegisterId.H], registers[RegisterId.L], "LD H,L", 4));
            Add(0x66, new LDMapping(registers[RegisterId.H], registers[RegisterId.HL], "LD H,(HL)", 8));
            Add(0x67, new LDMapping(registers[RegisterId.H], registers[RegisterId.A], "LD H,A", 4));
            Add(0x68, new LDMapping(registers[RegisterId.L], registers[RegisterId.B], "LD L,B", 4));
            Add(0x69, new LDMapping(registers[RegisterId.L], registers[RegisterId.C], "LD L,C", 4));
            Add(0x6A, new LDMapping(registers[RegisterId.L], registers[RegisterId.D], "LD L,D", 4));
            Add(0x6B, new LDMapping(registers[RegisterId.L], registers[RegisterId.E], "LD L,E", 4));
            Add(0x6C, new LDMapping(registers[RegisterId.L], registers[RegisterId.H], "LD L,H", 4));
            Add(0x6D, new LDMapping(registers[RegisterId.L], registers[RegisterId.L], "LD L,L", 4));
            Add(0x6E, new LDMapping(registers[RegisterId.L], registers[RegisterId.HL], "LD L,(HL)", 8));
            Add(0x6F, new LDMapping(registers[RegisterId.L], registers[RegisterId.A], "LD L,A", 4));
            Add(0x70, new LDMapping(registers[RegisterId.HL], registers[RegisterId.B], "LD (HL), B", 8));
            Add(0x71, new LDMapping(registers[RegisterId.HL], registers[RegisterId.C], "LD (HL), C", 8));
            Add(0x72, new LDMapping(registers[RegisterId.HL], registers[RegisterId.D], "LD (HL), D", 8));
            Add(0x73, new LDMapping(registers[RegisterId.HL], registers[RegisterId.E], "LD (HL), E", 8));
            Add(0x74, new LDMapping(registers[RegisterId.HL], registers[RegisterId.H], "LD (HL), H", 8));
            Add(0x75, new LDMapping(registers[RegisterId.HL], registers[RegisterId.L], "LD (HL), L", 8));
            Add(0x77, new LDMapping(registers[RegisterId.HL], registers[RegisterId.A], "LD (HL), A", 8));
            Add(0x78, new LDMapping(registers[RegisterId.A], registers[RegisterId.B], "LD A,B", 4));
            Add(0x79, new LDMapping(registers[RegisterId.A], registers[RegisterId.C], "LD A,C", 4));
            Add(0x7A, new LDMapping(registers[RegisterId.A], registers[RegisterId.D], "LD A,D", 4));
            Add(0x7B, new LDMapping(registers[RegisterId.A], registers[RegisterId.E], "LD A,E", 4));
            Add(0x7C, new LDMapping(registers[RegisterId.A], registers[RegisterId.H], "LD A,H", 4));
            Add(0x7D, new LDMapping(registers[RegisterId.A], registers[RegisterId.L], "LD A,L", 4));
            Add(0x7E, new LDMapping(registers[RegisterId.A], registers[RegisterId.HL], "LD A,(HL)", 8));
            Add(0x7F, new LDMapping(registers[RegisterId.A], registers[RegisterId.A], "LD A,A", 4));

            Add(0x02, new LDMapping(registers[RegisterId.BC], registers[RegisterId.A], "LD (BC),A", 8));
            Add(0x12, new LDMapping(registers[RegisterId.DE], registers[RegisterId.A], "LD (DE),A", 8));
            Add(0x22,
                new LDMapping(registers[RegisterId.HL], registers[RegisterId.A], "LD (HL+),A", 8, incrementValue: 1));
            Add(0x32,
                new LDMapping(registers[RegisterId.HL], registers[RegisterId.A], "LD (HL-),A", 8, incrementValue: -1));

            // Add(0x06, "LD B,d8");
            // Add(0x16, "LD D,d8");
            // Add(0x26, "LD H,d8");
            // Add(0x36, "LD (HL),d8");

            Add(0x0A, new LDMapping(registers[RegisterId.A], registers[RegisterId.BC], "LD A,(BC)", 8));
            Add(0x1A, new LDMapping(registers[RegisterId.A], registers[RegisterId.DE], "LD A,(DE)", 8));
            Add(0x2A,
                new LDMapping(registers[RegisterId.A], registers[RegisterId.HL], "LD A,(HL+)", 8, incrementValue: 1));
            Add(0x3A,
                new LDMapping(registers[RegisterId.A], registers[RegisterId.HL], "LD A,(HL-)", 8, incrementValue: -1));


            // Add(0x0E);
            // Add(0x1E);
            // Add(0x2E);
            // Add(0x3E);
            //
            // Add(0xE0);
            // Add(0xF0);
            //
            Add(0xE2, new LDMapping(registers[RegisterId.C], registers[RegisterId.A], "LD (C),A", 8));
            Add(0xF2, new LDMapping(registers[RegisterId.A], registers[RegisterId.C], "LD A,(C)", 8));
            //
            // Add(0xEA);
            // Add(0xFA);
        }
    }
}