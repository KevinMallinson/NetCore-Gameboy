using System;
using System.Text;
using Gameboy.Interfaces;
using LowLevelDesign.Hexify;

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
        private bool _interruptEnableFlag = false;

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

            return SetOrGetMemory(address);
        }

        public GBMemory GetWord(ushort address)
        {
            if (address > 0xFFFF)
            {
                throw new Exception($"0x{address:X} out of range.");
            }

            //higher address contains most significant byte
            var msb = GetByte((ushort)(address + 1));
            var lsb = GetByte(address);

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
            
            SetOrGetMemory(address, val);
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
            SetOrGetMemory(address, lsb);
            SetOrGetMemory((ushort)(address + 1), msb);
        }

        // Note that because addr is a 16 bit number, we need to use a bitmask
        // to fit in between our internal arrays bounds.
        private GBMemory? SetOrGetMemory(ushort addr, byte? val = null)
        {
            //Using memory map at http://gbdev.gg8.se/wiki/articles/Memory_Map
            switch (addr)
            {
                case { } when addr <= 0x7FFF: //ROM Bank
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.ROM_BANK, _romBank[addr], addr);
                    }
                    else
                    {
                        _romBank[addr] = val.Value;
                        return null;
                    }
                case { } when addr >= 0x8000 && addr <= 0x9FFF: //VRAM

                    if (val == null)
                    {
                        return _bus.GetGPU().GetByte(addr, MemoryRegion.VIDEO_RAM);
                    }
                    else
                    {
                        _bus.GetGPU().SetByte(addr, (byte) val, MemoryRegion.VIDEO_RAM);
                        return null;
                    }
                case { } when addr >= 0xA000 && addr <= 0xBFFF: // External RAM
                    //We need to apply a bitmask to reduce the value. Ignore the first 3 bits.
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.EXTERNAL_RAM, _externalRam[addr & 0x1FFF], addr);
                    }
                    else
                    {
                        _externalRam[addr & 0x1FFF] = val.Value;
                        return null;
                    }
                case { } when addr >= 0xC000 && addr <= 0xDFFF: // Work RAM
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.WORK_RAM, _workRam[addr & 0x1FFF], addr);
                    }
                    else
                    {
                        _workRam[addr & 0x1FFF] = val.Value;
                        return null;
                    }

                case { } when addr >= 0xE000 && addr <= 0xFDFF: // Echo RAM
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.ECHO_RAM, _workRam[addr & 0x1FFF], addr);
                    }
                    else
                    {
                        _workRam[addr & 0x1FFF] = val.Value;
                        return null;
                    }
                case { } when addr >= 0xFE00 && addr <= 0xFE9F: // Sprite attribute table (OAM)
                    if (val == null)
                    {
                        return _bus.GetGPU().GetByte(addr, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
                    }
                    else
                    {
                        _bus.GetGPU().SetByte(addr, (byte) val, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
                        return null;
                    }
                case { } when addr >= 0xFEA0 && addr <= 0xFEFF: //Not Usable
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.UNUSED, 0, addr);
                    }
                    else
                    {
                        return null;
                    }
                case { } when addr >= 0xFF00 && addr <= 0xFF7F: //IO Registers
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.IO_REGISTERS, _ioRegisters[addr & 0x007F], addr);
                    }
                    else
                    {
                        _ioRegisters[addr & 0x007F] = val.Value;
                        return null;
                    }
                case { } when addr >= 0xFF80 && addr <= 0xFFFE: //High RAM (HRAM)

                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.HIGH_RAM, _highRam[addr & 0x007F], addr);
                    }
                    else
                    {
                        _highRam[addr & 0x007F] = val.Value;
                        return null;
                    }
                case { } when addr == 0xFFFF: //Interrupt Enable Flag
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.INTERRUPT_FLAG, (ushort) (_interruptEnableFlag ? 1 : 0), addr);
                    }
                    else
                    {
                        _interruptEnableFlag = val.Value <= 1
                            ? Convert.ToBoolean(val.Value)
                            : throw new Exception($"Interrupt Enable Flag can only be 0 or 1. Passed value: {val.Value}");
                        return null;
                    }
            }
            
            throw new Exception("A catastrophic error has occured in the MMU.");
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
    }
}