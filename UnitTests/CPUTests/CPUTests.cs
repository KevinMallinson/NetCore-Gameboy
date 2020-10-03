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
    }
}