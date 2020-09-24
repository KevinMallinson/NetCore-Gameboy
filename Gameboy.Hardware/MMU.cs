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
            switch (addr & 0xF000)
            {

                case 0x0000:
                case 0x1000:
                case 0x2000:
                case 0x3000: //16KB ROM bank 00
                case 0x4000:
                case 0x5000:
                case 0x6000:
                case 0x7000: //16KB ROM Bank 01~NN
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.ROM_BANK, _romBank[addr], addr);
                    }
                    else
                    {
                        _romBank[addr] = val.Value;
                        return null;
                    }
                case 0x8000: //4kb VRAM
                case 0x9000: //4kb VRAM - total 8kb

                    if (val == null)
                    {
                        return _bus.GetGPU().GetByte(addr, MemoryRegion.VIDEO_RAM);
                    }
                    else
                    {
                        _bus.GetGPU().SetByte(addr, (byte)val, MemoryRegion.VIDEO_RAM);
                        return null;
                    }

                case 0xA000: //4kb external ram
                case 0xB000: //4kb external ram - total 8kb

                    //Note: ExternalRam has a range of 0x2000. Of course, 0xA000 or 0xB000 is too large. It is out of range.
                    //Let's say we receive 0xBFFF. This should be the last element of the array. Of course, it's out of range.
                    //We need to apply a bitmask to reduce the value. In this case, ignoring the first 3 bits serves this purpose.
                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.EXTERNAL_RAM, _externalRam[addr & 0x1FFF], addr);
                    }
                    else
                    {
                        _externalRam[addr & 0x1FFF] = val.Value;
                        return null;
                    }

                case 0xC000: //4kb work ram bank 0
                case 0xD000: //4kb work ram bank 1-N - total 8kb

                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.WORK_RAM, _workRam[addr & 0x1FFF], addr);
                    }
                    else
                    {
                        _workRam[addr & 0x1FFF] = val.Value;
                        return null;
                    }

                case 0xE000: //7.5kb echo ram. Only part, rest in 0xF000

                    if (val == null)
                    {
                        return new GBMemory(MemoryRegion.ECHO_RAM, _workRam[addr & 0x1FFF], addr);
                    }
                    else
                    {
                        _workRam[addr & 0x1FFF] = val.Value;
                        return null;
                    }

                case 0xF000:
                    switch (addr & 0x0F00)
                    {

                        case 0x000:
                        case 0x100:
                        case 0x200:
                        case 0x300:
                        case 0x400: //this is the rest of the echo ram
                        case 0x500:
                        case 0x600:
                        case 0x700:
                        case 0x800:
                        case 0x900: //this is the rest of the echo ram
                        case 0xA00:
                        case 0xB00:
                        case 0xC00:
                        case 0xD00: //this is the rest of the echo ram

                            if (val == null)
                            {
                                return new GBMemory(MemoryRegion.ECHO_RAM, _workRam[addr & 0x1FFF], addr);
                            }
                            else
                            {
                                _workRam[addr & 0x1FFF] = val.Value;
                                return null;
                            }

                        case 0xE00:

                            switch (addr & 0x00F0)
                            {

                                case 0x00:
                                case 0x10:
                                case 0x20:
                                case 0x30:
                                case 0x40: //Sprite attribute table (OAM)
                                case 0x50:
                                case 0x60:
                                case 0x70:
                                case 0x80:
                                case 0x90: //Sprite attribute table (OAM)

                                    if (val == null)
                                    {
                                        return _bus.GetGPU().GetByte(addr, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
                                    }
                                    else
                                    {
                                        _bus.GetGPU().SetByte(addr, (byte)val, MemoryRegion.SPRITE_ATTRIBUTE_TABLE);
                                        return null;
                                    }

                                case 0xA0:
                                case 0xB0:
                                case 0xC0: //Not Usable
                                case 0xD0:
                                case 0xE0:
                                case 0xF0: //Not Usable
                                    return new GBMemory(MemoryRegion.UNUSED, 0, addr);
                            }

                            break;

                        case 0xF00:

                            switch (addr & 0x00F0)
                            {
                                case 0x00:
                                case 0x10:
                                case 0x20:
                                case 0x30: //IO Registers
                                case 0x40:
                                case 0x50:
                                case 0x60:
                                case 0x70: //IO Registers

                                    if (val == null)
                                    {
                                        return new GBMemory(MemoryRegion.IO_REGISTERS, _ioRegisters[addr & 0x007F], addr);
                                    }
                                    else
                                    {
                                        _ioRegisters[addr & 0x007F] = val.Value;
                                        return null;
                                    }

                                case 0x80:
                                case 0x90:
                                case 0xA0:
                                case 0xB0: //High RAM (HRAM)
                                case 0xC0:
                                case 0xD0:
                                case 0xE0: //High RAM (HRAM)

                                    if (val == null)
                                    {
                                        return new GBMemory(MemoryRegion.HIGH_RAM, _highRam[addr & 0x007F], addr);
                                    }
                                    else
                                    {
                                        _highRam[addr & 0x007F] = val.Value;
                                        return null;
                                    }

                                case 0xF0:
                                    //if the last nibble is not F, it is still high ram, else it's the interrupts enable register
                                    if ((addr & 0x000F) == 0x000F)
                                    {
                                        if (val == null)
                                        {
                                            return new GBMemory(MemoryRegion.INTERRUPT_FLAG,
                                                (ushort)(_interruptEnableFlag ? 1 : 0), addr);
                                        }
                                        else
                                        {
                                            if (val.Value > 1)
                                            {
                                                throw new Exception($"Interrupt Enable Flag can only be 0 or 1. Passed value: {val.Value}");
                                            }
                                            
                                            _interruptEnableFlag = val.Value == 1;
                                            return null;
                                        }
                                    }
                                    else
                                    {
                                        if (val == null)
                                        {
                                            return new GBMemory(MemoryRegion.HIGH_RAM, _highRam[addr & 0x007F], addr);
                                        }
                                        else
                                        {
                                            _highRam[addr & 0x007F] = val.Value;
                                            return null;
                                        }
                                    }

                            }

                            break;

                    }

                    break;

            }

            throw new Exception($"Unable to find the memory address {addr}");
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