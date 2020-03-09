using System;
using Gameboy.Hardware;
using Gameboy.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

namespace Gameboy.Tests
{
    [TestClass]
    public class MMUTests
    {
        [TestMethod]
        public void Memory_ValidateBytes_WithSuccess()
        {
            /*
             * Notes:
             * Using int in the for loop, to prevent an infite loop (ushort will overflow back to 0, causing infinite loop)
             * In setting the bytes, we can't pass a 16 bit address to it, so for our test values...
             * ...we pass in alternating msb/lsb of the address, in the form of...
             * i % 2 == pass msb, !(i % 2) == pass lsb
             */

            var gpu = new GPU();
            var bus = new Bus(gpu);
            IMemoryUnit mmu = new MMU(bus);

            var workRam = new byte[0x2000];
            var index = 0;
            for (var i = 0xC000; i <= 0xDFFF; i++)
            {
                var val = i % 2 == 0 ? i >> 8 : i & 0x00FF;
                workRam[index] = (byte)val;
                index++;
            }
            index = 0;

            for (var i = 0; i <= 0xFFFF; i++)
            {
                var val = i % 2 == 0 ? i >> 8 : i & 0x00FF;

                // Ignore Echo Ram, it's already set in Work Ram.
                if (i < 0xE000 || i > 0xFDFF)
                {
                    mmu.SetByte((ushort)i, (byte)val);
                }
            }

            for (var i = 0; i < 0xFFFF; i++)
            {
                var val = mmu.GetByte((ushort)i);
                Assert.AreEqual((ushort)i, val.Address);
                Assert.AreEqual(true, val.IsByte);

                // Check the data stored at this address
                if (i >= 0xE000 && i <= 0xFDFF)
                {
                    Assert.AreEqual(workRam[index], val.Data);
                    index++;
                }
                else if (i >= 0xFEA0 && i <= 0xFEFF)
                {
                    Assert.AreEqual(0, val.Data);
                }
                else
                {
                    try
                    {
                        Assert.AreEqual(i % 2 == 0 ? i >> 8 : i & 0x00FF, val.Data);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw new Exception($"Value of i: {i}", e);
                    }
                }

                // Check the memory region is correct.
                switch (i)
                {
                    case var x when (x >= 0 && x <= 0x7FFF):
                        Assert.AreEqual(MemoryRegion.ROM_BANK, val.Region);
                        break;
                    case var x when (x >= 0x8000 && x <= 0x9FFF):
                        Assert.AreEqual(MemoryRegion.VIDEO_RAM, val.Region);
                        break;
                    case var x when (x >= 0xA000 && x <= 0xBFFF):
                        Assert.AreEqual(MemoryRegion.EXTERNAL_RAM, val.Region);
                        break;
                    case var x when (x >= 0xC000 && x <= 0xDFFF):
                        Assert.AreEqual(MemoryRegion.WORK_RAM, val.Region);
                        break;
                    case var x when (x >= 0xE000 && x <= 0xFDFF):
                        Assert.AreEqual(MemoryRegion.ECHO_RAM, val.Region);
                        break;
                    case var x when (x >= 0xFE00 && x <= 0xFE9F):
                        Assert.AreEqual(MemoryRegion.SPRITE_ATTRIBUTE_TABLE, val.Region);
                        break;
                    case var x when (x >= 0xFEA0 && x <= 0xFEFF):
                        Assert.AreEqual(MemoryRegion.UNUSED, val.Region);
                        break;
                    case var x when (x >= 0xFF00 && x <= 0xFF7F):
                        Assert.AreEqual(MemoryRegion.IO_REGISTERS, val.Region);
                        break;
                    case var x when (x >= 0xFF80 && x <= 0xFFFE):
                        Assert.AreEqual(MemoryRegion.HIGH_RAM, val.Region);
                        break;
                    case var x when (x == 0xFFFF):
                        Assert.AreEqual(MemoryRegion.INTERRUPT_FLAG, val.Region);
                        break;
                    default:
                        Assert.Fail($"Address 0x{i:X} not found.");
                        break;
                }
            }
        }
    }
}