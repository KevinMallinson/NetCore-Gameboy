using System;
using System.Text;
using Gameboy.Interfaces;
using LowLevelDesign.Hexify;

//Using memory map at http://gbdev.gg8.se/wiki/articles/Memory_Map
namespace Gameboy.Hardware
{
    public class MMU : IMemoryManagementUnit
    {
        //Start		End		Description						Notes																	//Implementation
        //0000		3FFF	16KB ROM bank 00				From cartridge, fixed bank												MMU
        //4000		7FFF	16KB ROM Bank 01~NN				From cartridge, switchable bank via MBC(if any)							MMU
        //8000		9FFF	8KB Video RAM(VRAM)				Only bank 0 in Non - CGB mode, Switchable bank 0 / 1 in CGB mode		GPU
        //A000		BFFF	8KB External RAM				In cartridge, switchable bank if any									MMU
        //C000		CFFF	4KB Work RAM(WRAM) bank 0																				MMU
        //D000		DFFF	4KB Work RAM(WRAM) bank 1~N		Only bank 1 in Non - CGB mode, Switchable bank 1~7 in CGB mode			MMU
        //E000		FDFF	Mirror of C000~DDFF(ECHO RAM)	Typically not used														MMU
        //FE00		FE9F	Sprite attribute table(OAM)																				GPU
        //FEA0		FEFF	Not Usable																								MMU
        //FF00		FF7F	I / O Registers																							MMU
        //FF80		FFFE	High RAM(HRAM)																							MMU
        //FFFF		FFFF	Interrupts Enable Register(IE)	

        private readonly ICentralProcessingUnit _cpu;
        private readonly IGraphicsProcessingUnit _gpu;
        
        private readonly int[] _romBank = new int[0x8000];

        private readonly int[] _externalRam = new int[0x2000];

        /* >>Video RAM implemented in GPU<< */
        /* >>External RAM not implemented<< */
        private readonly int[] _workRam = new int[0x2000];

        /* >>Echo RAM maps to work ram << */
        /* >>Sprite Attribute Table implemented in GPU<< */

        private readonly int[] _ioRegisters = new int[0x0080];
        private readonly int[] _highRam = new int[0x007F];
        private bool _interruptEnableFlag;

        public MMU(ICentralProcessingUnit cpu, IGraphicsProcessingUnit gpu)
        {
            _cpu = cpu;
            _gpu = gpu;
        }

        public GBMemory GetByte(int address)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }

            return GetMemory(address);
        }

        public GBMemory GetWord(int address)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }

            //higher address contains most significant byte
            var msb = GetMemory(address + 1);
            var lsb = GetMemory(address);

            if (msb.Region != lsb.Region)
            {
                throw new Exception($"The MSB had the region of {msb.Region} but the LSB had {lsb.Region}");
            }

            return new GBMemory(msb.Region, msb.Data << 8 | lsb.Data, address);
        }

        public void SetByte(int address, int val)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }
            
            SetMemory(address, val);
        }

        public void SetWord(int address, int val)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }

            var msb = val >> 8;
            var lsb = val & 0x00FF;

            // little endian! lsb in lower address.
            SetMemory(address, lsb);
            SetMemory(address + 1, msb);
        }
        
        private void SetMemory(int addr, int val)
        {
            Action<int, int> setMemory = addr switch
            {
                { } when addr <= 0x7FFF => SetROMBank,
                { } when addr <= 0x9FFF => SetVRAM,
                { } when addr <= 0xBFFF => SetExternalRAM,
                { } when addr <= 0xDFFF => SetWorkRAM,
                { } when addr <= 0xFDFF => SetEchoRAM,
                { } when addr <= 0xFE9F => SetSpriteAttributeTable,
                { } when addr <= 0xFEFF => SetUnusedAddressSpace,
                { } when addr <= 0xFF7F => SetIORegisters,
                { } when addr <= 0xFFFE => SetHighRAM,
                { } when addr == 0xFFFF => SetInterruptEnableFlag,
                _ => throw new Exception($"A catastrophic error has occured in the MMU. Address: {addr}")
            };

            setMemory(addr, val);
        }
        
        private GBMemory GetMemory(int addr)
        {
            return addr switch
            {
                { } when addr <= 0x7FFF => GetROMBank(addr),
                { } when addr <= 0x9FFF => GetVRAM(addr),
                { } when addr <= 0xBFFF => GetExternalRAM(addr),
                { } when addr <= 0xDFFF => GetWorkRAM(addr),
                { } when addr <= 0xFDFF => GetEchoRAM(addr),
                { } when addr <= 0xFE9F => GetSpriteAttributeTable(addr),
                { } when addr <= 0xFEFF => GetUnusedAddressSpace(addr),
                { } when addr <= 0xFF7F => GetIORegisters(addr),
                { } when addr <= 0xFFFE => GetHighRAM(addr),
                { } when addr == 0xFFFF => GetInterruptEnableFlag(),
                _ => throw new Exception($"A catastrophic error has occured in the MMU. Address: {addr}")
            };
        }

        public string Dump()
        {
            var headers = $"{"Address",-11}{"Value",-10}{"Region",-26}\n";
            var output = new StringBuilder(headers);
            
            for (var address = 0; address <= 0xFFFF; address++)
            {
                var val = GetByte(address);

                output.Append(
                    $"{$"0x{address:X4}",-11}" +
                    $"{$"{val.Data}",-10}" +
                    $"{$"{Enum.GetName(typeof(MemoryRegion), val.Region)}",-26}\n"
                );
            }

            return output.ToString();
        }
        
        public string HexDump()
        {
            var bytes = new byte[0xFFFF + 1]; // Range is 0 - 0xFFFF inclusive, hence + 1
            for (var address = 0; address <= 0xFFFF; address++)
            {
                var val = GetByte(address);
                bytes[address] = (byte)val.Data;
            }

            return Hex.PrettyPrint(bytes);
        }
        
        private void SetROMBank(int addr, int val) => _romBank[addr] = val;
        private GBMemory GetROMBank(int addr) => new GBMemory(MemoryRegion.ROM_BANK, _romBank[addr], addr);
        
        private void SetVRAM(int addr, int val) => _gpu.SetByte(addr, val, MemoryRegion.VIDEO_RAM);
        private GBMemory GetVRAM(int addr) => _gpu.GetByte(addr, MemoryRegion.VIDEO_RAM);

        private void SetExternalRAM(int addr, int val) => _externalRam[addr & 0x1FFF] = val;
        private GBMemory GetExternalRAM(int addr) => new GBMemory(MemoryRegion.EXTERNAL_RAM, _externalRam[addr & 0x1FFF], addr);

        private void SetWorkRAM(int addr, int val) => _workRam[addr & 0x1FFF] = val;
        private GBMemory GetWorkRAM(int addr) => new GBMemory(MemoryRegion.WORK_RAM, _workRam[addr & 0x1FFF], addr);

        private void SetEchoRAM(int addr, int val) => _workRam[addr & 0x1FFF] = val;
        private GBMemory GetEchoRAM(int addr) => new GBMemory(MemoryRegion.ECHO_RAM, _workRam[addr & 0x1FFF], addr);

        private void SetSpriteAttributeTable(int addr, int val) => _gpu.SetByte(addr, val, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
        private GBMemory GetSpriteAttributeTable(int addr) => _gpu.GetByte(addr, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
        
        private void SetUnusedAddressSpace(int addr, int val) { }
        private GBMemory GetUnusedAddressSpace(int addr) => new GBMemory(MemoryRegion.UNUSED, 0, addr);

        private void SetIORegisters(int addr, int val) => _ioRegisters[addr & 0x007F] = val;
        private GBMemory GetIORegisters(int addr) => new GBMemory(MemoryRegion.IO_REGISTERS, _ioRegisters[addr & 0x007F], addr);

        private void SetHighRAM(int addr, int val) => _highRam[addr & 0x007F] = val;
        private GBMemory GetHighRAM(int addr) => new GBMemory(MemoryRegion.HIGH_RAM, _highRam[addr & 0x007F], addr);

        private void SetInterruptEnableFlag(int addr, int val) =>
            _interruptEnableFlag = val == 0 || val == 1
            ? Convert.ToBoolean(val)
            : throw new Exception($"Interrupt Enable Flag can only be 0 or 1. Passed value: {val}");
        private GBMemory GetInterruptEnableFlag() => new GBMemory(MemoryRegion.INTERRUPT_FLAG, (_interruptEnableFlag ? 1 : 0), 0xFFFF);
    }
}