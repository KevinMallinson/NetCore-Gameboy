using System;
using System.Text;
using Interfaces;
using Hardware.System;
using LowLevelDesign.Hexify;

//Using memory map at http://gbdev.gg8.se/wiki/articles/Memory_Map
namespace Hardware.Memory
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

        public IMemory<byte> GetByte(ushort address)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }
            
            return address switch
            {
                { } when address <= 0x7FFF => GetROMBank(address),
                { } when address <= 0x9FFF => GetVRAM(address),
                { } when address <= 0xBFFF => GetExternalRAM(address),
                { } when address <= 0xDFFF => GetWorkRAM(address),
                { } when address <= 0xFDFF => GetEchoRAM(address),
                { } when address <= 0xFE9F => GetSpriteAttributeTable(address),
                { } when address <= 0xFEFF => GetUnusedAddressSpace(address),
                { } when address <= 0xFF7F => GetIORegisters(address),
                { } when address <= 0xFFFE => GetHighRAM(address),
                { } when address == 0xFFFF => GetInterruptEnableFlag(),
                _ => throw new Exception($"A catastrophic error has occured in the MMU. Address: {address}")
            };
        }

        public void SetByte(ushort address, byte value)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }

            Action<ushort, byte> setMemory = address switch
            {
                { } when address <= 0x7FFF => SetROMBank,
                { } when address <= 0x9FFF => SetVRAM,
                { } when address <= 0xBFFF => SetExternalRAM,
                { } when address <= 0xDFFF => SetWorkRAM,
                { } when address <= 0xFDFF => SetEchoRAM,
                { } when address <= 0xFE9F => SetSpriteAttributeTable,
                { } when address <= 0xFEFF => SetUnusedAddressSpace,
                { } when address <= 0xFF7F => SetIORegisters,
                { } when address <= 0xFFFE => SetHighRAM,
                { } when address == 0xFFFF => SetInterruptEnableFlag,
                _ => throw new Exception($"A catastrophic error has occured in the MMU. Address: {address}")
            };

            setMemory(address, value);
        }
        

        public IMemory<ushort> GetWord(ushort address)
        {
            //higher address contains most significant byte
            var msb = GetByte((ushort)(address + 1));
            var lsb = GetByte(address);

            if (msb.Region != lsb.Region)
            {
                throw new Exception($"The MSB had the region of {msb.Region} but the LSB had {lsb.Region}");
            }

            return new WordMemory(msb.Region, (ushort)(msb.Data << 8 | lsb.Data), address);
        }

        public void SetWord(ushort address, ushort val)
        {
            var msb = (byte)(val >> 8);
            var lsb = (byte)(val & 0x00FF);

            // little endian! lsb in lower address.
            SetByte(address, lsb);
            SetByte((ushort)(address + 1), msb);
        }

        private void SetROMBank(ushort addr, byte val) => _romBank[addr] = val;
        private IMemory<byte> GetROMBank(ushort addr) => new ByteMemory(MemoryRegion.ROM_BANK, _romBank[addr], addr);
        
        private void SetVRAM(ushort addr, byte val) => Bus.GPU.SetByte(addr, val, MemoryRegion.VIDEO_RAM);
        private IMemory<byte> GetVRAM(ushort addr) => Bus.GPU.GetByte(addr, MemoryRegion.VIDEO_RAM);

        private void SetExternalRAM(ushort addr, byte val) => _externalRam[addr & 0x1FFF] = val;
        private IMemory<byte> GetExternalRAM(ushort addr) => new ByteMemory(MemoryRegion.EXTERNAL_RAM, _externalRam[addr & 0x1FFF], addr);

        private void SetWorkRAM(ushort addr, byte val) => _workRam[addr & 0x1FFF] = val;
        private IMemory<byte> GetWorkRAM(ushort addr) => new ByteMemory(MemoryRegion.WORK_RAM, _workRam[addr & 0x1FFF], addr);

        private void SetEchoRAM(ushort addr, byte val) => _workRam[addr & 0x1FFF] = val;
        private IMemory<byte> GetEchoRAM(ushort addr) => new ByteMemory(MemoryRegion.ECHO_RAM, _workRam[addr & 0x1FFF], addr);

        private void SetSpriteAttributeTable(ushort addr, byte val) => Bus.GPU.SetByte(addr, val, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
        private IMemory<byte> GetSpriteAttributeTable(ushort addr) => Bus.GPU.GetByte(addr, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
        
        private void SetUnusedAddressSpace(ushort addr, byte val) { }
        private IMemory<byte> GetUnusedAddressSpace(ushort addr) => new ByteMemory(MemoryRegion.UNUSED, 0, addr);

        private void SetIORegisters(ushort addr, byte val) => _ioRegisters[addr & 0x007F] = val;
        private IMemory<byte> GetIORegisters(ushort addr) => new ByteMemory(MemoryRegion.IO_REGISTERS, _ioRegisters[addr & 0x007F], addr);

        private void SetHighRAM(ushort addr, byte val) => _highRam[addr & 0x007F] = val;
        private IMemory<byte> GetHighRAM(ushort addr) => new ByteMemory(MemoryRegion.HIGH_RAM, _highRam[addr & 0x007F], addr);

        private void SetInterruptEnableFlag(ushort addr, byte val) =>
            _interruptEnableFlag = val == 0 || val == 1
            ? Convert.ToBoolean(val)
            : throw new Exception($"Interrupt Enable Flag can only be 0 or 1. Passed value: {val}");
        
        private IMemory<byte> GetInterruptEnableFlag() => new ByteMemory(MemoryRegion.INTERRUPT_FLAG, (byte)(_interruptEnableFlag ? 1 : 0), 0xFFFF);
        
        public string Dump()
        {
            var headers = $"{"Address",-11}{"Value",-10}{"Region",-26}\n";
            var output = new StringBuilder(headers);
            
            for (var address = 0; address <= 0xFFFF; address++)
            {
                var val = GetByte((ushort)address);

                output.Append(
                    $"{$"0x{val.Address:X4}",-11}" +
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
                var val = GetByte((ushort)address);
                bytes[address] = val.Data;
            }

            return Hex.PrettyPrint(bytes);
        }
    }
}