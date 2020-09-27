using Gameboy.Hardware;
using Xunit;

namespace Gameboy.UnitTests.CPUTests
{
    public class CPUTests
    {
        [Fact]
        public void ValidateRegistersGetAndSet()
        {
            var cpu = new CPU();
            
            cpu.Registers[RegisterId.A].Data = 5;
            Assert.Equal(5, cpu.Registers[RegisterId.A].Data);
            Assert.Equal(RegisterId.A, cpu.Registers[RegisterId.A].Id);
            
            cpu.Registers[RegisterId.B].Data = 10;
            Assert.Equal(10, cpu.Registers[RegisterId.B].Data);
            Assert.Equal(RegisterId.B, cpu.Registers[RegisterId.B].Id);
            
            cpu.Registers[RegisterId.C].Data = 15;
            Assert.Equal(15, cpu.Registers[RegisterId.C].Data);
            Assert.Equal(RegisterId.C, cpu.Registers[RegisterId.C].Id);
            
            cpu.Registers[RegisterId.D].Data = 20;
            Assert.Equal(20, cpu.Registers[RegisterId.D].Data);
            Assert.Equal(RegisterId.D, cpu.Registers[RegisterId.D].Id);
            
            cpu.Registers[RegisterId.E].Data = 25;
            Assert.Equal(25, cpu.Registers[RegisterId.E].Data);
            Assert.Equal(RegisterId.E, cpu.Registers[RegisterId.E].Id);
            
            cpu.Registers[RegisterId.F].Data = 30;
            Assert.Equal(30 & 0xF0, cpu.Registers[RegisterId.F].Data); // last 4 bits get emptied out.
            Assert.Equal(RegisterId.F, cpu.Registers[RegisterId.F].Id);
            
            cpu.Registers[RegisterId.H].Data = 35;
            Assert.Equal(35, cpu.Registers[RegisterId.H].Data);
            Assert.Equal(RegisterId.H, cpu.Registers[RegisterId.H].Id);
            
            cpu.Registers[RegisterId.L].Data = 40;
            Assert.Equal(40, cpu.Registers[RegisterId.L].Data);
            Assert.Equal(RegisterId.L, cpu.Registers[RegisterId.L].Id);
            
            cpu.Registers[RegisterId.AF].Data = 65535;
            Assert.Equal(65535 & 0xFFF0, cpu.Registers[RegisterId.AF].Data); // last 4 bits of F gets emptied out.
            Assert.Equal(RegisterId.AF, cpu.Registers[RegisterId.AF].Id);
            
            cpu.Registers[RegisterId.BC].Data = 65280;
            Assert.Equal(65280, cpu.Registers[RegisterId.BC].Data);
            Assert.Equal(RegisterId.BC, cpu.Registers[RegisterId.BC].Id);
            
            cpu.Registers[RegisterId.DE].Data = 25000;
            Assert.Equal(25000, cpu.Registers[RegisterId.DE].Data);
            Assert.Equal(RegisterId.DE, cpu.Registers[RegisterId.DE].Id);
            
            cpu.Registers[RegisterId.HL].Data = 12500;
            Assert.Equal(12500, cpu.Registers[RegisterId.HL].Data);
            Assert.Equal(RegisterId.HL, cpu.Registers[RegisterId.HL].Id);
            
            //Each 16 bit register is based on two 8 bit registers, first register is MSB.
            Assert.Equal(65535 >> 8, cpu.Registers[RegisterId.A].Data); 
            Assert.Equal(65535 & 0x00F0, cpu.Registers[RegisterId.F].Data); // last 4 bits of F gets emptied out.
            
            Assert.Equal(65280 >> 8, cpu.Registers[RegisterId.B].Data);
            Assert.Equal(65280 & 0x00FF, cpu.Registers[RegisterId.C].Data); 
            
            Assert.Equal(25000 >> 8, cpu.Registers[RegisterId.D].Data); 
            Assert.Equal(25000 & 0x00FF, cpu.Registers[RegisterId.E].Data); 
            
            Assert.Equal(12500 >> 8, cpu.Registers[RegisterId.H].Data); 
            Assert.Equal(12500 & 0x00FF, cpu.Registers[RegisterId.L].Data);
        }
        
        [Theory]
        [InlineData(RegisterId.B, RegisterId.D, 111)]
        [InlineData(RegisterId.H, RegisterId.A, 222)]
        [InlineData(RegisterId.L, RegisterId.C, 123)]
        public void LD(RegisterId dest, RegisterId src, int value)
        {
            var cpu = new CPU();

            cpu.Registers[src].Data = value; 
            cpu.LD(dest, src);
            
            Assert.Equal(value, cpu.Registers[dest].Data);
        }
    }
}