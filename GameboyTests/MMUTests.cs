using System;
using GameboyHardware;
using GameboyInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GameboyTests
{
    [TestClass]
    public class MMUTests
    {
        
        [TestMethod]
        public void Memory_ValidateRAM_WithSuccess()
        {
            var gpu = new Mock<IGraphicsProcessingUnit>();
            var bus = new Mock<IHardwareBus>();
            bus.Setup(x => x.GetGPU()).Returns(gpu.Object);
            gpu.Setup(x => x.GetByte(It.IsAny<ushort>())).Returns(
                new GBMemory(MemoryRegion.VIDEO_RAM, 1337, 9001)
            );
            gpu.Setup(x => x.SetByte(It.IsAny<ushort>(), It.IsAny<byte>()));


            const byte testVal = 237;
            IMemoryUnit mmu = new MMU(bus.Object);
            for (ushort i = 0; i < 0x10000 - 1; i++)
            {
                mmu.SetByte(i, testVal);
            }

            for (ushort i = 0; i < 0x10000 - 1; i++)
            {
                var val = mmu.GetByte(i);

                if (i >= 0xFEA0 && i <= 0xFEFF)
                {
                    //Unused RAM always returns 0 and disallows being set.
                    Assert.AreEqual(0, val.Data);
                    Assert.AreEqual(i, val.Address);
                }
                else if ((i >= 0x8000 && i <= 0x9FFF) || (i >= 0xFE00 && i <= 0xFE9F))
                {
                    //Not testing VRAM or OAM, so we expect the mocked value of 1337 and address of 9001.
                    Assert.AreEqual(1337, val.Data);
                    Assert.AreEqual(9001, val.Address);
                }
                else
                {
                    Assert.AreEqual(testVal, val.Data);
                    Assert.AreEqual(i, val.Address);
                }
            }
        }
    }
}