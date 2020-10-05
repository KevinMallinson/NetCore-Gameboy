using System;
using System.Collections.Generic;
using Extensions;
using Interfaces;
using Hardware.Instructions;
using Hardware.Memory;
using Hardware.System;

namespace Hardware.Processor
{
    public class InstructionTable : IInstructionTable
    {
        public IInstructionBuilder Get(int opcode) => _dictionary[opcode];
        private readonly Dictionary<int, IInstructionBuilder> _dictionary;

        public InstructionTable(IRegisterIO registerIO)
        {
            var registers = registerIO;
            _dictionary = new Dictionary<int, IInstructionBuilder>
            {
                // --------------------------- //
                // LD, LDH and LDHL Operations //
                // --------------------------- //
                {0x40, new InstructionBuilder(1, x => new LD(registers.B, registers.B, "LD B,B", 4))},
                {0x41, new InstructionBuilder(1, x => new LD(registers.B, registers.C, "LD B,C", 4))},
                {0x42, new InstructionBuilder(1, x => new LD(registers.B, registers.D, "LD B,D", 4))},
                {0x43, new InstructionBuilder(1, x => new LD(registers.B, registers.E, "LD B,E", 4))},
                {0x44, new InstructionBuilder(1, x => new LD(registers.B, registers.H, "LD B,H", 4))},
                {0x45, new InstructionBuilder(1, x => new LD(registers.B, registers.L, "LD B,L", 4))},
                {0x47, new InstructionBuilder(1, x => new LD(registers.B, registers.A, "LD B,A", 4))},
                {0x48, new InstructionBuilder(1, x => new LD(registers.C, registers.B, "LD C,B", 4))},
                {0x49, new InstructionBuilder(1, x => new LD(registers.C, registers.C, "LD C,C", 4))},
                {0x4A, new InstructionBuilder(1, x => new LD(registers.C, registers.D, "LD C,D", 4))},
                {0x4B, new InstructionBuilder(1, x => new LD(registers.C, registers.E, "LD C,E", 4))},
                {0x4C, new InstructionBuilder(1, x => new LD(registers.C, registers.H, "LD C,H", 4))},
                {0x4D, new InstructionBuilder(1, x => new LD(registers.C, registers.L, "LD C,L", 4))},
                {0x4F, new InstructionBuilder(1, x => new LD(registers.C, registers.A, "LD C,A", 4))},
                {0x50, new InstructionBuilder(1, x => new LD(registers.D, registers.B, "LD D,B", 4))},
                {0x51, new InstructionBuilder(1, x => new LD(registers.D, registers.C, "LD D,C", 4))},
                {0x52, new InstructionBuilder(1, x => new LD(registers.D, registers.D, "LD D,D", 4))},
                {0x53, new InstructionBuilder(1, x => new LD(registers.D, registers.E, "LD D,E", 4))},
                {0x54, new InstructionBuilder(1, x => new LD(registers.D, registers.H, "LD D,H", 4))},
                {0x55, new InstructionBuilder(1, x => new LD(registers.D, registers.L, "LD D,L", 4))},
                {0x57, new InstructionBuilder(1, x => new LD(registers.D, registers.A, "LD D,A", 4))},
                {0x58, new InstructionBuilder(1, x => new LD(registers.E, registers.B, "LD E,B", 4))},
                {0x59, new InstructionBuilder(1, x => new LD(registers.E, registers.C, "LD E,C", 4))},
                {0x5A, new InstructionBuilder(1, x => new LD(registers.E, registers.D, "LD E,D", 4))},
                {0x5B, new InstructionBuilder(1, x => new LD(registers.E, registers.E, "LD E,E", 4))},
                {0x5C, new InstructionBuilder(1, x => new LD(registers.E, registers.H, "LD E,H", 4))},
                {0x5D, new InstructionBuilder(1, x => new LD(registers.E, registers.L, "LD E,L", 4))},
                {0x5F, new InstructionBuilder(1, x => new LD(registers.E, registers.A, "LD E,A", 4))},
                {0x60, new InstructionBuilder(1, x => new LD(registers.H, registers.B, "LD H,B", 4))},
                {0x61, new InstructionBuilder(1, x => new LD(registers.H, registers.C, "LD H,C", 4))},
                {0x62, new InstructionBuilder(1, x => new LD(registers.H, registers.D, "LD H,D", 4))},
                {0x63, new InstructionBuilder(1, x => new LD(registers.H, registers.E, "LD H,E", 4))},
                {0x64, new InstructionBuilder(1, x => new LD(registers.H, registers.H, "LD H,H", 4))},
                {0x65, new InstructionBuilder(1, x => new LD(registers.H, registers.L, "LD H,L", 4))},
                {0x67, new InstructionBuilder(1, x => new LD(registers.H, registers.A, "LD H,A", 4))},
                {0x68, new InstructionBuilder(1, x => new LD(registers.L, registers.B, "LD L,B", 4))},
                {0x69, new InstructionBuilder(1, x => new LD(registers.L, registers.C, "LD L,C", 4))},
                {0x6A, new InstructionBuilder(1, x => new LD(registers.L, registers.D, "LD L,D", 4))},
                {0x6B, new InstructionBuilder(1, x => new LD(registers.L, registers.E, "LD L,E", 4))},
                {0x6C, new InstructionBuilder(1, x => new LD(registers.L, registers.H, "LD L,H", 4))},
                {0x6D, new InstructionBuilder(1, x => new LD(registers.L, registers.L, "LD L,L", 4))},
                {0x6F, new InstructionBuilder(1, x => new LD(registers.L, registers.A, "LD L,A", 4))},
                {0x78, new InstructionBuilder(1, x => new LD(registers.A, registers.B, "LD A,B", 4))},
                {0x79, new InstructionBuilder(1, x => new LD(registers.A, registers.C, "LD A,C", 4))},
                {0x7A, new InstructionBuilder(1, x => new LD(registers.A, registers.D, "LD A,D", 4))},
                {0x7B, new InstructionBuilder(1, x => new LD(registers.A, registers.E, "LD A,E", 4))},
                {0x7C, new InstructionBuilder(1, x => new LD(registers.A, registers.H, "LD A,H", 4))},
                {0x7D, new InstructionBuilder(1, x => new LD(registers.A, registers.L, "LD A,L", 4))},
                {0x7F, new InstructionBuilder(1, x => new LD(registers.A, registers.A, "LD A,A", 4))},
                
                {0xF9, new InstructionBuilder(1, x => new LD(registers.SP, registers.HL, "LD SP,HL", 8))},
                
                {0x46, new InstructionBuilder(1, x => new LD(registers.B, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD B,(HL)", 8))},
                {0x4E, new InstructionBuilder(1, x => new LD(registers.C, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD C,(HL)", 8))},
                {0x56, new InstructionBuilder(1, x => new LD(registers.D, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD D,(HL)", 8))},
                {0x5E, new InstructionBuilder(1, x => new LD(registers.E, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD E,(HL)", 8))},
                {0x66, new InstructionBuilder(1, x => new LD(registers.H, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD H,(HL)", 8))},
                {0x6E, new InstructionBuilder(1, x => new LD(registers.L, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD L,(HL)", 8))},
                {0x7E, new InstructionBuilder(1, x => new LD(registers.A, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD A,(HL)", 8))},
                {0xF2, new InstructionBuilder(1, x => new LD(registers.A, new HalfAddress(registers.C.Get(), AddressSource.Register), "LD A,(C)", 8))},
                {0x0A, new InstructionBuilder(1, x => new LD(registers.A, new FullAddress(registers.BC.Get(), AddressSource.Register), "LD A,(BC)", 8))},
                {0x1A, new InstructionBuilder(1, x => new LD(registers.A, new FullAddress(registers.DE.Get(), AddressSource.Register), "LD A,(DE)", 8))},
                {0x2A, new InstructionBuilder(1, x => new LD(registers.A, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD A,(HL+)", 8, () => registers.HL.Increment(1)))},
                {0x3A, new InstructionBuilder(1, x => new LD(registers.A, new FullAddress(registers.HL.Get(), AddressSource.Register), "LD A,(HL-)", 8, () => registers.HL.Decrement(1)))},
                {0xFA, new InstructionBuilder(3, a16 => new LD(registers.A, new FullAddress(a16, AddressSource.Immediate), "LD A,(a16)", 16))},
                
                {0x02, new InstructionBuilder(1, x => new LD(new FullAddress(registers.BC.Get(), AddressSource.Register), registers.A, "LD (BC),A", 8))},
                {0x12, new InstructionBuilder(1, x => new LD(new FullAddress(registers.DE.Get(), AddressSource.Register), registers.A, "LD (DE),A", 8))},
                {0x22, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.A, "LD (HL+),A", 8, () => registers.HL.Increment(1)))},
                {0x32, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.A, "LD (HL-),A", 8, () => registers.HL.Decrement(1)))},
                {0x70, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.B, "LD (HL), B", 8))},
                {0x71, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.C, "LD (HL), C", 8))},
                {0x72, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.D, "LD (HL), D", 8))},
                {0x73, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.E, "LD (HL), E", 8))},
                {0x74, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.H, "LD (HL), H", 8))},
                {0x75, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.L, "LD (HL), L", 8))},
                {0x77, new InstructionBuilder(1, x => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), registers.A, "LD (HL), A", 8))},
                {0xE2, new InstructionBuilder(1, x => new LD(new HalfAddress(registers.C.Get(), AddressSource.Register), registers.A, "LD (C),A", 8))},
                {0xEA, new InstructionBuilder(3, a16 => new LD(new FullAddress(a16, AddressSource.Immediate), registers.A, "LD (a16),A", 16))},
                
                {0x08, new InstructionBuilder(3, a16 => new LD(new FullAddress(a16, AddressSource.Immediate), registers.SP, "LD (a16),SP", 20))},
                
                {0x36, new InstructionBuilder(2, d8 => new LD(new FullAddress(registers.HL.Get(), AddressSource.Register), d8.ToByteWithAssert(), "LD (HL),d8", 12))},

                {0x06, new InstructionBuilder(2, d8 => new LD(registers.B, d8.ToByteWithAssert(), "LD B,d8", 8))},
                {0x16, new InstructionBuilder(2, d8 => new LD(registers.D, d8.ToByteWithAssert(), "LD D,d8", 8))},
                {0x26, new InstructionBuilder(2, d8 => new LD(registers.H, d8.ToByteWithAssert(), "LD H,d8", 8))},
                {0x0E, new InstructionBuilder(2, d8 => new LD(registers.C, d8.ToByteWithAssert(), "LD C,d8", 8))},
                {0x1E, new InstructionBuilder(2, d8 => new LD(registers.E, d8.ToByteWithAssert(), "LD E,d8", 8))},
                {0x2E, new InstructionBuilder(2, d8 => new LD(registers.L, d8.ToByteWithAssert(), "LD L,d8", 8))},
                {0x3E, new InstructionBuilder(2, d8 => new LD(registers.A, d8.ToByteWithAssert(), "LD A,d8", 8))},
                
                {0x01, new InstructionBuilder(3, d16 => new LD(registers.BC, d16, "LD BC,d16", 12))},
                {0x11, new InstructionBuilder(3, d16 => new LD(registers.DE, d16, "LD DE,d16", 12))},
                {0x21, new InstructionBuilder(3, d16 => new LD(registers.HL, d16, "LD HL,d16", 12))},
                {0xF8, new InstructionBuilder(2, r8 => new LD(registers.HL, (ushort)(registers.SP.Get() + r8), "LD HL,SP+r8", 12))},
                {0x31, new InstructionBuilder(3, d16 => new LD(registers.SP, d16, "LD SP,d16", 12))},
           
                {0xE0, new InstructionBuilder(2, a8 => new LDH(new HalfAddress(a8.ToByteWithAssert(), AddressSource.Immediate), registers.A, "LDH (a8),A", 12))},
                
                {0xF0, new InstructionBuilder(2, a8 => new LDH(registers.A, new HalfAddress(a8.ToByteWithAssert(), AddressSource.Immediate), "LDH A,(a8)", 12))},
            };
        }
    }
}