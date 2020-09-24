using System;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using Gameboy.Hardware;
using Gameboy.Interfaces;
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
//FE00		FE9F	Sprite attribute table(OAM)																				GPU
//FEA0		FEFF	Not Usable																								MMU
//FF00		FF7F	I / O Registers																							MMU
//FF80		FFFE	High RAM(HRAM)																							MMU
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
        public void Memory_ValidateBytes_WithSuccess()
        {
            // SET UP THE MEMORY
            var headers = $"{"Address",-11}{"Expected",-12}{"Actual",-10}{"Expected",-26}{"Actual",-26}\n";
            var output = new StringBuilder(headers);
            var gpu = new GPU();
            var bus = new Bus(gpu);
            IMemoryUnit mmu = new MMU(bus);
            var expectedValues = new byte[0xFFFF + 1];
            
            var i = 0;
            while(i < 0xFFFF)
            {
                if (i == 0xE000) // Skip Echo RAM
                    i = 0xFE00;
                
                expectedValues[i] = (byte) (i % 256);
                mmu.SetByte((ushort) i, expectedValues[i]);
                i++;
            }
            
            Array.ConstrainedCopy(
                expectedValues, 
                0xC000, 
                expectedValues, 
                0xE000, 
                7680
            ); // Copy the work ram into the echo ram
            Array.Fill(expectedValues, (byte)0, 0xFEA0, 96); // Unusable Address Space
            
            expectedValues[0xFFFF] = 1; // Interrupt Enable Flag
            mmu.SetByte(0xFFFF, expectedValues[0xFFFF]);
            
            
            //VALIDATE THE MEMORY IN THE APPROVAL TEST
            for (var address = 0; address <= 0xFFFF; address++)
            {
                var val = mmu.GetByte((ushort) address);

                var expectedRegion = address switch
                {
                    var x when (x >= 0 && x <= 0x7FFF) => MemoryRegion.ROM_BANK,
                    var x when (x >= 0x8000 && x <= 0x9FFF) => MemoryRegion.VIDEO_RAM,
                    var x when (x >= 0xA000 && x <= 0xBFFF) => MemoryRegion.EXTERNAL_RAM,
                    var x when (x >= 0xC000 && x <= 0xDFFF) => MemoryRegion.WORK_RAM,
                    var x when (x >= 0xE000 && x <= 0xFDFF) => MemoryRegion.ECHO_RAM,
                    var x when (x >= 0xFE00 && x <= 0xFE9F) => MemoryRegion.SPRITE_ATTRIBUTE_TABLE,
                    var x when (x >= 0xFEA0 && x <= 0xFEFF) => MemoryRegion.UNUSED,
                    var x when (x >= 0xFF00 && x <= 0xFF7F) => MemoryRegion.IO_REGISTERS,
                    var x when (x >= 0xFF80 && x <= 0xFFFE) => MemoryRegion.HIGH_RAM,
                    var x when (x == 0xFFFF) => MemoryRegion.INTERRUPT_FLAG,
                    _ => throw new Exception($"Address 0x{address:X} not found.")
                };

                output.Append(
                    $"{$"0x{address:X4}",-11}" +
                    $"{$"{expectedValues[address]}",-12}" +
                    $"{$"{val.Data}",-10}" +
                    $"{$"{Enum.GetName(typeof(MemoryRegion), expectedRegion)}",-26}" +
                    $"{$"{Enum.GetName(typeof(MemoryRegion), val.Region)}",-26}\n"
                );
            }
            
            Approvals.Verify(output);
        }
    }
}