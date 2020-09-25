using Gameboy.Hardware;
using Xunit;

namespace Gameboy.UnitTests.CPUTests
{
    public class CPUTests
    {
        [Theory]
        [InlineData(Register.B, Register.D, 111)]
        [InlineData(Register.H, Register.A, 222)]
        [InlineData(Register.L, Register.C, 123)]
        public void LD(Register dest, Register src, int value)
        {
            var cpu = new CPU();

            cpu[src] = value; 
            cpu.LD(dest, src);
            
            Assert.Equal(value, cpu[dest]);
        }
    }
}