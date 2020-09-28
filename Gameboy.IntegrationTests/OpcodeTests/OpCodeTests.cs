using System;
using System.Collections.Generic;
using System.Linq;
using Gameboy.Hardware;
using Gameboy.Interfaces;
using Moq;
using Xunit;

namespace Gameboy.IntegrationTests.OpcodeTests
{
    public class OpcodeTests
    {
        private readonly GameboyDevice _gameboy;
        private readonly Mock<IROMReader> _romReader;

        public OpcodeTests()
        {
            var gpu = new GPU();
            var cpu = new CPU();
            var mmu = new MMU(cpu, gpu);
            Bus.Init(cpu, gpu, mmu);
            
            _romReader = new Mock<IROMReader>();
            _gameboy = new GameboyDevice(_romReader.Object);
        }
        
        private void ConstructTestROM(List<byte> data)
        {
            var jumpToHex600 = new byte[] {0xC3, 0x00, 0x6};
            data.AddRange(jumpToHex600);
            
            var rom = new List<byte>(data);
            rom.AddRange(Enumerable.Repeat<byte>(0, 0x600 - data.Count));
        
            // Cause an infinite loop
            rom.AddRange(jumpToHex600);
            rom.AddRange(Enumerable.Repeat<byte>(0, 0x8000 - rom.Count));

            _romReader
                .Setup(x => x.ReadRom(It.IsAny<string>()))
                .Returns(rom);
            
            _gameboy.LoadROM("<MOCKED>");
        }
        
        [Fact]
        public void LD()
        {
            ConstructTestROM(new List<byte> { 0x06, 0xFF, 0x48 } );
            _gameboy.Cycle();
        }
    }
}