using System;
using System.Text;
using Gameboy.Interfaces;
using LowLevelDesign.Hexify;

//Using memory map at http://gbdev.gg8.se/wiki/articles/Memory_Map
namespace Gameboy.Hardware
{
    public class MMU : IMemoryUnit
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

        private readonly byte[] _romBank = new byte[0x8000];

        private readonly byte[] _externalRam = new byte[0x2000];

        /* >>Video RAM implemented in GPU<< */
        /* >>External RAM not implemented<< */
        private readonly byte[] _workRam = new byte[0x2000];

        /* >>Echo RAM maps to work ram << */
        /* >>Sprite Attribute Table implemented in GPU<< */

        private readonly byte[] _ioRegisters = new byte[0x0080];
        private readonly byte[] _highRam = new byte[0x007F];
        private bool _interruptEnableFlag;

        //The bus will provide access to the cpu, gpu, etc.
        private readonly IHardwareBus _bus;

        public MMU(IHardwareBus bus)
        {
            _bus = bus;
        }

        public GBMemory GetByte(ushort address)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }

            return GetMemory(address);
        }

        public GBMemory GetWord(ushort address)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }

            //higher address contains most significant byte
            var msb = GetMemory((ushort)(address + 1));
            var lsb = GetMemory(address);

            if (msb.Region != lsb.Region)
            {
                throw new Exception($"The MSB had the region of {msb.Region} but the LSB had {lsb.Region}");
            }

            return new GBMemory(msb.Region, (ushort)(msb.Data << 8 | lsb.Data), address);
        }

        public void SetByte(ushort address, byte val)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }
            
            SetMemory(address, val);
        }

        public void SetWord(ushort address, ushort val)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }

            var msb = (byte)(val >> 8);
            var lsb = (byte)(val & 0x00FF);

            // little endian! lsb in lower address.
            SetMemory(address, lsb);
            SetMemory((ushort)(address + 1), msb);
        }
        
        private void SetMemory(ushort addr, byte val)
        {
            Action<ushort, byte> setMemory = addr switch
            {
                { } when addr <= 0x7FFF => SetROMBank,
                { } when addr >= 0x8000 && addr <= 0x9FFF => SetVRAM,
                { } when addr >= 0xA000 && addr <= 0xBFFF => SetExternalRAM,
                { } when addr >= 0xC000 && addr <= 0xDFFF => SetWorkRAM,
                { } when addr >= 0xE000 && addr <= 0xFDFF => SetEchoRAM,
                { } when addr >= 0xFE00 && addr <= 0xFE9F => SetSpriteAttributeTable,
                { } when addr >= 0xFEA0 && addr <= 0xFEFF => SetUnusedAddressSpace,
                { } when addr >= 0xFF00 && addr <= 0xFF7F => SetIORegisters,
                { } when addr >= 0xFF80 && addr <= 0xFFFE => SetHighRAM,
                { } when addr == 0xFFFF => SetInterruptEnableFlag,
                _ => throw new Exception($"A catastrophic error has occured in the MMU. Address: {addr}")
            };

            setMemory(addr, val);
        }
        
        private GBMemory GetMemory(ushort addr)
        {
            return addr switch
            {
                { } when addr <= 0x7FFF => GetROMBank(addr),
                { } when addr >= 0x8000 && addr <= 0x9FFF => GetVRAM(addr),
                { } when addr >= 0xA000 && addr <= 0xBFFF => GetExternalRAM(addr),
                { } when addr >= 0xC000 && addr <= 0xDFFF => GetWorkRAM(addr),
                { } when addr >= 0xE000 && addr <= 0xFDFF => GetEchoRAM(addr),
                { } when addr >= 0xFE00 && addr <= 0xFE9F => GetSpriteAttributeTable(addr),
                { } when addr >= 0xFEA0 && addr <= 0xFEFF => GetUnusedAddressSpace(addr),
                { } when addr >= 0xFF00 && addr <= 0xFF7F => GetIORegisters(addr),
                { } when addr >= 0xFF80 && addr <= 0xFFFE => GetHighRAM(addr),
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
                var val = GetByte((ushort) address);

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
                var val = GetByte((ushort) address);
                bytes[address] = (byte)val.Data;
            }

            return Hex.PrettyPrint(bytes);
        }
        
        private void SetROMBank(ushort addr, byte val) => _romBank[addr] = val;
        private GBMemory GetROMBank(ushort addr) => new GBMemory(MemoryRegion.ROM_BANK, _romBank[addr], addr);
        
        private void SetVRAM(ushort addr, byte val) => _bus.GetGPU().SetByte(addr, val, MemoryRegion.VIDEO_RAM);
        private GBMemory GetVRAM(ushort addr) => _bus.GetGPU().GetByte(addr, MemoryRegion.VIDEO_RAM);

        private void SetExternalRAM(ushort addr, byte val) => _externalRam[addr & 0x1FFF] = val;
        private GBMemory GetExternalRAM(ushort addr) => new GBMemory(MemoryRegion.EXTERNAL_RAM, _externalRam[addr & 0x1FFF], addr);

        private void SetWorkRAM(ushort addr, byte val) => _workRam[addr & 0x1FFF] = val;
        private GBMemory GetWorkRAM(ushort addr) => new GBMemory(MemoryRegion.WORK_RAM, _workRam[addr & 0x1FFF], addr);

        private void SetEchoRAM(ushort addr, byte val) => _workRam[addr & 0x1FFF] = val;
        private GBMemory GetEchoRAM(ushort addr) => new GBMemory(MemoryRegion.ECHO_RAM, _workRam[addr & 0x1FFF], addr);

        private void SetSpriteAttributeTable(ushort addr, byte val) => _bus.GetGPU().SetByte(addr, val, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
        private GBMemory GetSpriteAttributeTable(ushort addr) => _bus.GetGPU().GetByte(addr, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
        
        private void SetUnusedAddressSpace(ushort addr, byte val) { }
        private GBMemory GetUnusedAddressSpace(ushort addr) => new GBMemory(MemoryRegion.UNUSED, 0, addr);

        private void SetIORegisters(ushort addr, byte val) => _ioRegisters[addr & 0x007F] = val;
        private GBMemory GetIORegisters(ushort addr) => new GBMemory(MemoryRegion.IO_REGISTERS, _ioRegisters[addr & 0x007F], addr);

        private void SetHighRAM(ushort addr, byte val) => _highRam[addr & 0x007F] = val;
        private GBMemory GetHighRAM(ushort addr) => new GBMemory(MemoryRegion.HIGH_RAM, _highRam[addr & 0x007F], addr);

        private void SetInterruptEnableFlag(ushort addr, byte val) =>
            _interruptEnableFlag = val == 0 || val == 1
            ? Convert.ToBoolean(val)
            : throw new Exception($"Interrupt Enable Flag can only be 0 or 1. Passed value: {val}");
        private GBMemory GetInterruptEnableFlag() => new GBMemory(MemoryRegion.INTERRUPT_FLAG, (ushort) (_interruptEnableFlag ? 1 : 0), 0xFFFF);
    }
}