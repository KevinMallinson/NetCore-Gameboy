using System;
using System.Linq;
using System.Text.RegularExpressions;
using Gameboy.Hardware;
using Moq;
using Xunit;

namespace Gameboy.UnitTests.CPUTests
{
    public class CPUTests
    {
        private readonly CPU _cpu;
        private readonly GPU _gpu;
        private readonly MMU _mmu;
        private readonly Random _rand;

        public CPUTests()
        {
            _cpu = new CPU();
            _gpu = new GPU();
            _mmu = new MMU(_cpu, _gpu);
            _rand = new Random();
            Bus.Reset();
            Bus.Init(_cpu, _gpu, _mmu);
        }
        
        [Fact]
        public void ValidateRegisters()
        {
            var cpu = new CPU();
            
            cpu.IOHelper[RegisterId.A].Data = 5;
            Assert.Equal(5, cpu.IOHelper[RegisterId.A].Data);
            Assert.Equal(RegisterId.A, cpu.IOHelper[RegisterId.A].Id);
            
            cpu.IOHelper[RegisterId.B].Data = 10;
            Assert.Equal(10, cpu.IOHelper[RegisterId.B].Data);
            Assert.Equal(RegisterId.B, cpu.IOHelper[RegisterId.B].Id);
            
            cpu.IOHelper[RegisterId.C].Data = 15;
            Assert.Equal(15, cpu.IOHelper[RegisterId.C].Data);
            Assert.Equal(RegisterId.C, cpu.IOHelper[RegisterId.C].Id);
            
            cpu.IOHelper[RegisterId.D].Data = 20;
            Assert.Equal(20, cpu.IOHelper[RegisterId.D].Data);
            Assert.Equal(RegisterId.D, cpu.IOHelper[RegisterId.D].Id);
            
            cpu.IOHelper[RegisterId.E].Data = 25;
            Assert.Equal(25, cpu.IOHelper[RegisterId.E].Data);
            Assert.Equal(RegisterId.E, cpu.IOHelper[RegisterId.E].Id);
            
            cpu.IOHelper[RegisterId.F].Data = 30;
            Assert.Equal(30 & 0xF0, cpu.IOHelper[RegisterId.F].Data); // last 4 bits get emptied out.
            Assert.Equal(RegisterId.F, cpu.IOHelper[RegisterId.F].Id);
            
            cpu.IOHelper[RegisterId.H].Data = 35;
            Assert.Equal(35, cpu.IOHelper[RegisterId.H].Data);
            Assert.Equal(RegisterId.H, cpu.IOHelper[RegisterId.H].Id);
            
            cpu.IOHelper[RegisterId.L].Data = 40;
            Assert.Equal(40, cpu.IOHelper[RegisterId.L].Data);
            Assert.Equal(RegisterId.L, cpu.IOHelper[RegisterId.L].Id);
            
            cpu.IOHelper[RegisterId.AF].Data = 65535;
            Assert.Equal(65535 & 0xFFF0, cpu.IOHelper[RegisterId.AF].Data); // last 4 bits of F gets emptied out.
            Assert.Equal(RegisterId.AF, cpu.IOHelper[RegisterId.AF].Id);
            
            cpu.IOHelper[RegisterId.BC].Data = 65280;
            Assert.Equal(65280, cpu.IOHelper[RegisterId.BC].Data);
            Assert.Equal(RegisterId.BC, cpu.IOHelper[RegisterId.BC].Id);
            
            cpu.IOHelper[RegisterId.DE].Data = 25000;
            Assert.Equal(25000, cpu.IOHelper[RegisterId.DE].Data);
            Assert.Equal(RegisterId.DE, cpu.IOHelper[RegisterId.DE].Id);
            
            cpu.IOHelper[RegisterId.HL].Data = 12500;
            Assert.Equal(12500, cpu.IOHelper[RegisterId.HL].Data);
            Assert.Equal(RegisterId.HL, cpu.IOHelper[RegisterId.HL].Id);
            
            //Each 16 bit register is based on two 8 bit registers, first register is MSB.
            Assert.Equal(65535 >> 8, cpu.IOHelper[RegisterId.A].Data); 
            Assert.Equal(65535 & 0x00F0, cpu.IOHelper[RegisterId.F].Data); // last 4 bits of F gets emptied out.
            
            Assert.Equal(65280 >> 8, cpu.IOHelper[RegisterId.B].Data);
            Assert.Equal(65280 & 0x00FF, cpu.IOHelper[RegisterId.C].Data); 
            
            Assert.Equal(25000 >> 8, cpu.IOHelper[RegisterId.D].Data); 
            Assert.Equal(25000 & 0x00FF, cpu.IOHelper[RegisterId.E].Data); 
            
            Assert.Equal(12500 >> 8, cpu.IOHelper[RegisterId.H].Data); 
            Assert.Equal(12500 & 0x00FF, cpu.IOHelper[RegisterId.L].Data);
        }
        
        [Theory]
        [InlineData(0x40, "LD", "B,B")]
        [InlineData(0x41, "LD", "B,C")]
        [InlineData(0x42, "LD", "B,D")]
        [InlineData(0x43, "LD", "B,E")]
        [InlineData(0x44, "LD", "B,H")]
        [InlineData(0x45, "LD", "B,L")]
        [InlineData(0x47, "LD", "B,A")]
        
        [InlineData(0x48, "LD", "C,B")]
        [InlineData(0x49, "LD", "C,C")]
        [InlineData(0x4A, "LD", "C,D")]
        [InlineData(0x4B, "LD", "C,E")]
        [InlineData(0x4C, "LD", "C,H")]
        [InlineData(0x4D, "LD", "C,L")]
        [InlineData(0x4F, "LD", "C,A")]
        
        [InlineData(0x50, "LD", "D,B")]
        [InlineData(0x51, "LD", "D,C")]
        [InlineData(0x52, "LD", "D,D")]
        [InlineData(0x53, "LD", "D,E")]
        [InlineData(0x54, "LD", "D,H")]
        [InlineData(0x55, "LD", "D,L")]
        [InlineData(0x57, "LD", "D,A")]
        
        [InlineData(0x58, "LD", "E,B")]
        [InlineData(0x59, "LD", "E,C")]
        [InlineData(0x5A, "LD", "E,D")]
        [InlineData(0x5B, "LD", "E,E")]
        [InlineData(0x5C, "LD", "E,H")]
        [InlineData(0x5D, "LD", "E,L")]
        [InlineData(0x5F, "LD", "E,A")]
        
        [InlineData(0x60, "LD", "H,B")]
        [InlineData(0x61, "LD", "H,C")]
        [InlineData(0x62, "LD", "H,D")]
        [InlineData(0x63, "LD", "H,E")]
        [InlineData(0x64, "LD", "H,H")]
        [InlineData(0x65, "LD", "H,L")]
        [InlineData(0x67, "LD", "H,A")]
        
        [InlineData(0x68, "LD", "L,B")]
        [InlineData(0x69, "LD", "L,C")]
        [InlineData(0x6A, "LD", "L,D")]
        [InlineData(0x6B, "LD", "L,E")]
        [InlineData(0x6C, "LD", "L,H")]
        [InlineData(0x6D, "LD", "L,L")]
        [InlineData(0x6F, "LD", "L,A")]

        [InlineData(0x78, "LD", "A,B")]
        [InlineData(0x79, "LD", "A,C")]
        [InlineData(0x7A, "LD", "A,D")]
        [InlineData(0x7B, "LD", "A,E")]
        [InlineData(0x7C, "LD", "A,H")]
        [InlineData(0x7D, "LD", "A,L")]
        [InlineData(0x7F, "LD", "A,A")]
        public void LDRegisterToRegister(int opcode, string mnemonic, string registers)
        {
            var split = registers.Split(',');
            var dest = Enum.Parse<RegisterId>(split[0]);
            var src = Enum.Parse<RegisterId>(split[1]);

            var val = _rand.Next(0, 0xFF + 1);
            _mmu.SetByte(0, opcode);
            
            _cpu.IOHelper[src].Data = val;
            
            var executedOpcode = _cpu.Step();
            
            Assert.Equal($"{mnemonic} {registers}", executedOpcode.ToString());
            Assert.Equal(4, executedOpcode.Cycles);
            Assert.Equal(val, _cpu.IOHelper[src].Data);
            Assert.Equal(val, _cpu.IOHelper[dest].Data);
        }
        
        [Theory]
        [InlineData(0x70, "LD", "(HL),B")]
        [InlineData(0x71, "LD", "(HL),C")]
        [InlineData(0x72, "LD", "(HL),D")]
        [InlineData(0x73, "LD", "(HL),E")]
        [InlineData(0x77, "LD", "(HL),A")]
        [InlineData(0x74, "LD", "(HL),H")]
        [InlineData(0x75, "LD", "(HL),L")]
        public void LoadRegisterIntoRAM(int opcode, string mnemonic, string registers)
        {
            var split = registers.Split(',');
            var src = Enum.Parse<RegisterId>(split[1]);

            var destRegisterGroup = split[0].Replace("(", "").Replace(")", "");
            var hiRegister = Enum.Parse<RegisterId>(destRegisterGroup[0].ToString());
            var loRegister = Enum.Parse<RegisterId>(destRegisterGroup[1].ToString());

            var dest = Enum.Parse<RegisterId>(destRegisterGroup);
            _mmu.SetByte(0, opcode);
            
            // Just use bank 0 of RAM.
            var address = _rand.Next(0xC000, 0xCFFF + 1);
            _cpu.IOHelper[dest].Data = address;
            Assert.Equal(address, _cpu.IOHelper[dest].Data);
            
            var val = _rand.Next(0, 0xFF + 1);
            _cpu.IOHelper[src].Data = val;

            if (src == hiRegister)
            {
                var hi = val;
                var lo = address & 0x00FF;
                address = (hi << 8) | lo;
            }
            else if (src == loRegister)
            {
                address = (address & 0xFF00) | val;    
            }
            
            
            var executedOpcode = _cpu.Step();
            
            Assert.Equal($"{mnemonic} {registers}", executedOpcode.ToString());
            Assert.Equal(8, executedOpcode.Cycles);
            Assert.Equal(address, _cpu.IOHelper[dest].Data);
            Assert.Equal(val, _cpu.IOHelper[src].Data);
            Assert.Equal(val, Bus.MMU.GetByte(_cpu.IOHelper[dest].Data).Data);
            Assert.Equal(address >> 8, _cpu.IOHelper[hiRegister].Data);
            Assert.Equal(address & 0x00FF, _cpu.IOHelper[loRegister].Data);
        }
        
        [Theory] // (HL) means value at memory address stored in Register HL.
        [InlineData(0x46, "LD", "B,(HL)")]
        [InlineData(0x4E, "LD", "C,(HL)")]
        [InlineData(0x56, "LD", "D,(HL)")]
        [InlineData(0x5E, "LD", "E,(HL)")]
        [InlineData(0x7E, "LD", "A,(HL)")]
        [InlineData(0x66, "LD", "H,(HL)")]
        [InlineData(0x6E, "LD", "L,(HL)")]
        public void LoadRAMIntoRegister(int opcode, string mnemonic, string registers)
        {
            var split = registers.Split(',');
            var srcRegisterGroup = split[1].Replace("(", "").Replace(")", "");
            var src = Enum.Parse<RegisterId>(srcRegisterGroup);
            var dest = Enum.Parse<RegisterId>(split[0]);
            _mmu.SetByte(0, opcode);
            
            // Just use bank 0 of RAM.
            var address = _rand.Next(0xC000, 0xCFFF + 1);
            _cpu.IOHelper[src].Data = address;
            Assert.Equal(address, _cpu.IOHelper[src].Data);
            
            var val = _rand.Next(0, 0xFF + 1);
            Bus.MMU.SetByte(address, val);
            Assert.Equal(val, Bus.MMU.GetByte(address).Data);

            var hiRegister = Enum.Parse<RegisterId>(srcRegisterGroup[0].ToString());
            var loRegister = Enum.Parse<RegisterId>(srcRegisterGroup[1].ToString());

            if (dest == hiRegister)
            {
                var hi = val;
                var lo = address & 0x00FF;
                address = (hi << 8) | lo;
            }
            else if (dest == loRegister)
            {
                address = (address & 0xFF00) | val;  
            }
            
            var executedOpcode = _cpu.Step();
            
            Assert.Equal($"{mnemonic} {registers}", executedOpcode.ToString());
            Assert.Equal(8, executedOpcode.Cycles);
            Assert.Equal(val, _cpu.IOHelper[dest].Data);
            Assert.Equal(address >> 8, _cpu.IOHelper[hiRegister].Data);
            Assert.Equal(address & 0x00FF, _cpu.IOHelper[loRegister].Data);
            Assert.Equal(address, _cpu.IOHelper[src].Data);
        }
    }
}