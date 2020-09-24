using System;
using System.Text;
using Gameboy.Hardware;
using Gameboy.Interfaces;
using Xunit;

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
    public class MMUTests
    {
        [Fact]
        public void Memory_ValidateBytes_WithSuccess()
        {
            var headers = $"{"Address",-11}{"Expected",-12}{"Actual",-10}{"Expected",-26}{"Actual",-26}\n";
            var output = new StringBuilder(headers);
            
            var rand = new Random();
            var expectedValues = new byte[0xFFFF + 1];
            var workRamStart = 0xC000;
            var gpu = new GPU();
            var bus = new Bus(gpu);
            IMemoryUnit mmu = new MMU(bus);

            // Set up a list, equal in length to our RAM, filled with random numbers.
            //i is an int to prevent an infinite loop (ushort will overflow back to 0 on the last increment).
            for (var i = 0; i <= 0xFFFF; i++)
            {
                byte val;
                var isEcho = false;

                // Echo ram is a copy of work ram, so insert those values to the expected array.
                if (i >= 0xE000 && i <= 0xFDFF)
                {
                    val = expectedValues[workRamStart++];
                    isEcho = true;
                }

                // Unused Address Space - can only be 0 by design.
                else if (i >= 0xFEA0 && i <= 0xFEFF)
                {
                    val = 0;
                }
                
                // Interrupt Enable Flag can only be 0 or 1.
                else if (i == 0xFFFF)
                {
                    val = (byte) rand.Next(0, 2); // upper bound is exclusive
                }
                
                // For all other cases
                else
                {
                    val = (byte) rand.Next(0, 256); // upper bound is exclusive
                }

                expectedValues[i] = val;

                // Either set the byte to 0 (for unused address space),
                // 0 or 1 for Interrupt Enable Flag,
                // or the random (for everything except echo ram).
                if (!isEcho)
                {
                    mmu.SetByte((ushort) i, expectedValues[i]);
                }
            }

            try
            {
                // Validate the randoms appear in the correct address.
                for (var address = 0; address <= 0xFFFF; address++)
                {
                    var val = mmu.GetByte((ushort) address);
                    
                    // Check the memory region is correct.
                    MemoryRegion expectedRegion;
                    switch (address)
                    {
                        case var x when (x >= 0 && x <= 0x7FFF):
                            expectedRegion = MemoryRegion.ROM_BANK;
                            break;
                        case var x when (x >= 0x8000 && x <= 0x9FFF):
                            expectedRegion = MemoryRegion.VIDEO_RAM;
                            break;
                        case var x when (x >= 0xA000 && x <= 0xBFFF):
                            expectedRegion = MemoryRegion.EXTERNAL_RAM;
                            break;
                        case var x when (x >= 0xC000 && x <= 0xDFFF):
                            expectedRegion = MemoryRegion.WORK_RAM;
                            break;
                        case var x when (x >= 0xE000 && x <= 0xFDFF):
                            expectedRegion = MemoryRegion.ECHO_RAM;
                            break;
                        case var x when (x >= 0xFE00 && x <= 0xFE9F):
                            expectedRegion = MemoryRegion.SPRITE_ATTRIBUTE_TABLE;
                            break;
                        case var x when (x >= 0xFEA0 && x <= 0xFEFF):
                            expectedRegion = MemoryRegion.UNUSED;
                            break;
                        case var x when (x >= 0xFF00 && x <= 0xFF7F):
                            expectedRegion = MemoryRegion.IO_REGISTERS;
                            break;
                        case var x when (x >= 0xFF80 && x <= 0xFFFE):
                            expectedRegion = MemoryRegion.HIGH_RAM;
                            break;
                        case var x when (x == 0xFFFF):
                            expectedRegion = MemoryRegion.INTERRUPT_FLAG;
                            break;
                        default:
                            throw new Exception($"Address 0x{address:X} not found.");
                    }
                    
                    output.Append(
                        $"{$"0x{address:X4}",-11}" +
                        $"{$"{expectedValues[address]}",-12}" +
                        $"{$"{val.Data}",-10}" +
                        $"{$"{Enum.GetName(typeof(MemoryRegion), expectedRegion)}",-26}" +
                        $"{$"{Enum.GetName(typeof(MemoryRegion), val.Region)}",-26}\n"
                    );

                    Assert.Equal((ushort) address, val.Address);
                    Assert.True(val.IsByte);
                    Assert.Equal(expectedValues[address], val.Data);
                    Assert.Equal(expectedRegion, val.Region);
                }

            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}\n\n{output}");
            }

            Console.WriteLine(output);
        }
    }
}