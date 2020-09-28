using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using Gameboy.Hardware;
using Gameboy.Interfaces;
using Moq;
using Xunit;
using Xunit.Abstractions;

//Start		End		Description
//0000		3FFF	16KB ROM Bank
//4000		7FFF	16KB ROM Bank
//8000		9FFF	8KB Video RAM(VRAM)
//A000		BFFF	8KB External RAM
//C000		CFFF	4KB Work RAM(WRAM)
//D000		DFFF	4KB Work RAM(WRAM)
//E000		FDFF	Mirror of C000~DDFF(ECHO RAM)
//FE00		FE9F	Sprite attribute table(OAM)						GPU
//FEA0		FEFF	Not Usable										MMU
//FF00		FF7F	I / O Registers									MMU
//FF80		FFFE	High RAM(HRAM)									MMU
//FFFF		FFFF	Interrupts Enable Register(IE)

namespace Gameboy.ApprovalTests.MMUTests
{
    [UseReporter(typeof(DiffReporter))]
    public class MMUTests
    {
        private readonly ITestOutputHelper _output;

        public MMUTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void MMU_ValidateHumanReadableDump()
        {
            var mmu = FillMMU();
            Approvals.Verify(mmu.Dump());
        }
        
        [Fact]
        public void MMU_ValidateHexDump()
        {
            var mmu = FillMMU();
            Approvals.Verify(mmu.HexDump());
        }

        private static IMemoryManagementUnit FillMMU()
        {
            var cpu = new CPU();
            var gpu = new GPU();
            var mmu = new MMU(cpu, gpu);
            Bus.Reset();
            Bus.Init(cpu, gpu, mmu);
            var expectedValues = new int[0xFFFF + 1];

            var i = 0;
            while (i < 0xFFFF)
            {
                if (i == 0xE000) // Skip Echo RAM
                    i = 0xFE00;

                expectedValues[i] = i % 256;
                mmu.SetByte(i, expectedValues[i]);
                i++;
            }

            Array.ConstrainedCopy(
                expectedValues,
                0xC000,
                expectedValues,
                0xE000,
                7680
            ); // Copy the work ram into the echo ram
            
            Array.Fill(expectedValues, 0, 0xFEA0, 96); // Unusable Address Space

            expectedValues[0xFFFF] = 1; // Interrupt Enable Flag
            mmu.SetByte(0xFFFF, expectedValues[0xFFFF]);
            
            return mmu;
        }
    }
}