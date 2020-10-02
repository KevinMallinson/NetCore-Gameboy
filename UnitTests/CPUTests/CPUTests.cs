using Hardware.System;
using Interfaces;
using Xunit;

namespace UnitTests.CPUTests
{
    public class CPUTests
    {
        public CPUTests()
        {
            Bus.Init();
        }
        
        [Fact]
        public void ValidateRegisters()
        {
            var registers = Bus.CPU.Registers;
            
            registers.A.Set(5);
            Assert.Equal(5, registers.A.Get());
            Assert.Equal(RegisterId.A, registers.A.Id);
            
            registers.B.Set(10);
            Assert.Equal(10, registers.B.Get());
            Assert.Equal(RegisterId.B, registers.B.Id);
            
            registers.C.Set(15);
            Assert.Equal(15, registers.C.Get());
            Assert.Equal(RegisterId.C, registers.C.Id);
            
            registers.D.Set(20);
            Assert.Equal(20, registers.D.Get());
            Assert.Equal(RegisterId.D, registers.D.Id);
            
            registers.E.Set(25);
            Assert.Equal(25, registers.E.Get());
            Assert.Equal(RegisterId.E, registers.E.Id);
            
            registers.F.Set(30);
            Assert.Equal(30 & 0xF0, registers.F.Get()); // last 4 bits get emptied out.
            Assert.Equal(RegisterId.F, registers.F.Id);
            
            registers.H.Set(35);
            Assert.Equal(35, registers.H.Get());
            Assert.Equal(RegisterId.H, registers.H.Id);
            
            registers.L.Set(40);
            Assert.Equal(40, registers.L.Get());
            Assert.Equal(RegisterId.L, registers.L.Id);
            
            registers.AF.Set(65535);
            Assert.Equal(65535 & 0xFFF0, registers.AF.Get()); // last 4 bits of F gets emptied out.
            Assert.Equal(RegisterId.AF, registers.AF.Id);
            
            registers.BC.Set(65280);
            Assert.Equal(65280, registers.BC.Get());
            Assert.Equal(RegisterId.BC, registers.BC.Id);
            
            registers.DE.Set(25000);
            Assert.Equal(25000, registers.DE.Get());
            Assert.Equal(RegisterId.DE, registers.DE.Id);
            
            registers.HL.Set(12500);
            Assert.Equal(12500, registers.HL.Get());
            Assert.Equal(RegisterId.HL, registers.HL.Id);
            
            //Each 16 bit register is based on two 8 bit registers, first register is MSB.
            Assert.Equal(65535 >> 8, registers.A.Get()); 
            Assert.Equal(65535 & 0x00F0, registers.F.Get()); // last 4 bits of F gets emptied out.
            
            Assert.Equal(65280 >> 8, registers.B.Get());
            Assert.Equal(65280 & 0x00FF, registers.C.Get()); 
            
            Assert.Equal(25000 >> 8, registers.D.Get()); 
            Assert.Equal(25000 & 0x00FF, registers.E.Get()); 
            
            Assert.Equal(12500 >> 8, registers.H.Get()); 
            Assert.Equal(12500 & 0x00FF, registers.L.Get());
        }
        
        // {0x40,   x => new LD(registers.B, registers.B, "LD B,B", 4)},
        // {0x41,   x => new LD(registers.B, registers.C, "LD B,C", 4)},
        // {0x42,   x => new LD(registers.B, registers.D, "LD B,D", 4)},
        // {0x43,   x => new LD(registers.B, registers.E, "LD B,E", 4)},
        // {0x44,   x => new LD(registers.B, registers.H, "LD B,H", 4)},
        // {0x45,   x => new LD(registers.B, registers.L, "LD B,L", 4)},
        // {0x47,   x => new LD(registers.B, registers.A, "LD B,A", 4)},
        // {0x48,   x => new LD(registers.C, registers.B, "LD C,B", 4)},
        // {0x49,   x => new LD(registers.C, registers.C, "LD C,C", 4)},
        // {0x4A,   x => new LD(registers.C, registers.D, "LD C,D", 4)},
        // {0x4B,   x => new LD(registers.C, registers.E, "LD C,E", 4)},
        // {0x4C,   x => new LD(registers.C, registers.H, "LD C,H", 4)},
        // {0x4D,   x => new LD(registers.C, registers.L, "LD C,L", 4)},
        // {0x4F,   x => new LD(registers.C, registers.A, "LD C,A", 4)},
        // {0x50,   x => new LD(registers.D, registers.B, "LD D,B", 4)},
        // {0x51,   x => new LD(registers.D, registers.C, "LD D,C", 4)},
        // {0x52,   x => new LD(registers.D, registers.D, "LD D,D", 4)},
        // {0x53,   x => new LD(registers.D, registers.E, "LD D,E", 4)},
        // {0x54,   x => new LD(registers.D, registers.H, "LD D,H", 4)},
        // {0x55,   x => new LD(registers.D, registers.L, "LD D,L", 4)},
        // {0x57,   x => new LD(registers.D, registers.A, "LD D,A", 4)},
        // {0x58,   x => new LD(registers.E, registers.B, "LD E,B", 4)},
        // {0x59,   x => new LD(registers.E, registers.C, "LD E,C", 4)},
        // {0x5A,   x => new LD(registers.E, registers.D, "LD E,D", 4)},
        // {0x5B,   x => new LD(registers.E, registers.E, "LD E,E", 4)},
        // {0x5C,   x => new LD(registers.E, registers.H, "LD E,H", 4)},
        // {0x5D,   x => new LD(registers.E, registers.L, "LD E,L", 4)},
        // {0x5F,   x => new LD(registers.E, registers.A, "LD E,A", 4)},
        // {0x60,   x => new LD(registers.H, registers.B, "LD H,B", 4)},
        // {0x61,   x => new LD(registers.H, registers.C, "LD H,C", 4)},
        // {0x62,   x => new LD(registers.H, registers.D, "LD H,D", 4)},
        // {0x63,   x => new LD(registers.H, registers.E, "LD H,E", 4)},
        // {0x64,   x => new LD(registers.H, registers.H, "LD H,H", 4)},
        // {0x65,   x => new LD(registers.H, registers.L, "LD H,L", 4)},
        // {0x67,   x => new LD(registers.H, registers.A, "LD H,A", 4)},
        // {0x68,   x => new LD(registers.L, registers.B, "LD L,B", 4)},
        // {0x69,   x => new LD(registers.L, registers.C, "LD L,C", 4)},
        // {0x6A,   x => new LD(registers.L, registers.D, "LD L,D", 4)},
        // {0x6B,   x => new LD(registers.L, registers.E, "LD L,E", 4)},
        // {0x6C,   x => new LD(registers.L, registers.H, "LD L,H", 4)},
        // {0x6D,   x => new LD(registers.L, registers.L, "LD L,L", 4)},
        // {0x6F,   x => new LD(registers.L, registers.A, "LD L,A", 4)},
        // {0x78,   x => new LD(registers.A, registers.B, "LD A,B", 4)},
        // {0x79,   x => new LD(registers.A, registers.C, "LD A,C", 4)},
        // {0x7A,   x => new LD(registers.A, registers.D, "LD A,D", 4)},
        // {0x7B,   x => new LD(registers.A, registers.E, "LD A,E", 4)},
        // {0x7C,   x => new LD(registers.A, registers.H, "LD A,H", 4)},
        // {0x7D,   x => new LD(registers.A, registers.L, "LD A,L", 4)},
        // {0x7F,   x => new LD(registers.A, registers.A, "LD A,A", 4)},
        //
        // {0xF9,   x => new LD(registers.SP, registers.HL, "LD SP,HL", 8)},
        //
        // {0x46,   x => new LD(registers.B, new FullAddress(registers.HL.Get()), "LD B,(HL)", 8)},
        // {0x4E,   x => new LD(registers.C, new FullAddress(registers.HL.Get()), "LD C,(HL)", 8)},
        // {0x56,   x => new LD(registers.D, new FullAddress(registers.HL.Get()), "LD D,(HL)", 8)},
        // {0x5E,   x => new LD(registers.E, new FullAddress(registers.HL.Get()), "LD E,(HL)", 8)},
        // {0x66,   x => new LD(registers.H, new FullAddress(registers.HL.Get()), "LD H,(HL)", 8)},
        // {0x6E,   x => new LD(registers.L, new FullAddress(registers.HL.Get()), "LD L,(HL)", 8)},
        // {0x7E,   x => new LD(registers.A, new FullAddress(registers.HL.Get()), "LD A,(HL)", 8)},
        // {0xF2,   x => new LD(registers.A, new FullAddress(registers.C.Get()), "LD A,(C)", 8)},
        // {0x0A,   x => new LD(registers.A, new FullAddress(registers.BC.Get()), "LD A,(BC)", 8)},
        // {0x1A,   x => new LD(registers.A, new FullAddress(registers.DE.Get()), "LD A,(DE)", 8)},
        // {0x2A,   x => new LD(registers.A, new FullAddress(registers.HL.Get()), "LD A,(HL+)", 8, () => registers.HL.Increment(1)) },
        // {0x3A,   x => new LD(registers.A, new FullAddress(registers.HL.Get()), "LD A,(HL-)", 8, () => registers.HL.Decrement(1)) },
        // {0xFA, a16 => new LD(registers.A, new FullAddress(a16), "LD A,(a16)", 16)},
        //
        // {0x02,   x => new LD(new FullAddress(registers.BC.Get()), registers.A, "LD (BC),A", 8)},
        // {0x12,   x => new LD(new FullAddress(registers.DE.Get()), registers.A, "LD (DE),A", 8)},
        // {0x22,   x => new LD(new FullAddress(registers.HL.Get()), registers.A, "LD (HL+),A", 8, () => registers.HL.Increment(1))},
        // {0x32,   x => new LD(new FullAddress(registers.HL.Get()), registers.A, "LD (HL-),A", 8, () => registers.HL.Decrement(1))},
        // {0x70,   x => new LD(new FullAddress(registers.HL.Get()), registers.B, "LD (HL), B", 8)},
        // {0x71,   x => new LD(new FullAddress(registers.HL.Get()), registers.C, "LD (HL), C", 8)},
        // {0x72,   x => new LD(new FullAddress(registers.HL.Get()), registers.D, "LD (HL), D", 8)},
        // {0x73,   x => new LD(new FullAddress(registers.HL.Get()), registers.E, "LD (HL), E", 8)},
        // {0x74,   x => new LD(new FullAddress(registers.HL.Get()), registers.H, "LD (HL), H", 8)},
        // {0x75,   x => new LD(new FullAddress(registers.HL.Get()), registers.L, "LD (HL), L", 8)},
        // {0x77,   x => new LD(new FullAddress(registers.HL.Get()), registers.A, "LD (HL), A", 8)},
        // {0xE2,   x => new LD(new FullAddress(registers.C.Get()), registers.A, "LD (C),A", 8)},
        // {0xEA, a16 => new LD(new FullAddress(a16), registers.A, "LD (a16),A", 16)},
        // {0x08, a16 => new LD(new FullAddress(a16), registers.SP, "LD (a16),SP", 20)},
        //
        // {0x36,  d8 => new LD(new FullAddress(registers.HL.Get()), d8.ToByteWithAssert(), "LD (HL),d8", 12)},
        //
        // {0x06,  d8 => new LD(registers.B, d8.ToByteWithAssert(), "LD B,d8", 8)},
        // {0x16,  d8 => new LD(registers.D, d8.ToByteWithAssert(), "LD D,d8", 8)},
        // {0x26,  d8 => new LD(registers.H, d8.ToByteWithAssert(), "LD H,d8", 8)},
        // {0x0E,  d8 => new LD(registers.A, d8.ToByteWithAssert(), "LD C,d8", 8)},
        // {0x1E,  d8 => new LD(registers.A, d8.ToByteWithAssert(), "LD E,d8", 8)},
        // {0x2E,  d8 => new LD(registers.A, d8.ToByteWithAssert(), "LD L,d8", 8)},
        // {0x3E,  d8 => new LD(registers.A, d8.ToByteWithAssert(), "LD A,d8", 8)},
        //
        // {0x01, d16 => new LD(registers.BC, d16, "LD BC,d16", 12)},
        // {0x11, d16 => new LD(registers.DE, d16, "LD DE,d16", 12)},
        // {0x21, d16 => new LD(registers.HL, d16, "LD HL,d16", 12)},
        // {0xF8,  r8 => new LD(registers.HL, (ushort)(registers.SP.Get() + r8), "LD HL,SP+r8", 12)},
        // {0x31, d16 => new LD(registers.SP, d16, "LD SP,d16", 12)},
        //
        // {0xE0,  a8 => new LDH(new HalfAddress(a8.ToByteWithAssert()), registers.A, "LDH (a8),A", 12)},
        //
        // {0xF0,  a8 => new LDH(registers.A, new HalfAddress(a8.ToByteWithAssert()), "LDH A,(a8)", 12)},
    }
}