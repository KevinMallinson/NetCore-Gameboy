using System.Collections.Generic;
using Gameboy.Interfaces;

namespace Gameboy.Hardware
{
    public class IOHelper
    {
        private readonly IReadOnlyDictionary<RegisterId, Register> _registers;
        
        //Own DSL, Not exposed to public
        //Created solely to help with getting data at memory address assigned in register
        private readonly IReadOnlyDictionary<RegisterId, Register> _ramRegisters;

        public IReadOnlyDictionary<RegisterId, Register> Registers => _registers;
        
        public IOHelper()
        {
            var dictionary = new Dictionary<RegisterId, Register>
            {
                {RegisterId.B, new Register(RegisterId.B)},
                {RegisterId.C, new Register(RegisterId.C)},
                {RegisterId.D, new Register(RegisterId.D)},
                {RegisterId.E, new Register(RegisterId.E)},
                {RegisterId.H, new Register(RegisterId.H)},
                {RegisterId.L, new Register(RegisterId.L)},
                {RegisterId.F, new Register(RegisterId.F, x => x & 0xF0, x => x & 0xF0)},
                {RegisterId.A, new Register(RegisterId.A)}
            };

            dictionary.Add(RegisterId.AF, new Register
            (
                RegisterId.AF,
                x => dictionary[RegisterId.A].Data << 8 | dictionary[RegisterId.F].Data,
                x =>
                {
                    //Flags last 4 bits always zero (ZNHC0000)
                    dictionary[RegisterId.A].Data = x >> 8;
                    dictionary[RegisterId.F].Data = x & 0x00F0;
                    return x & 0xFFF0;
                },
                isByte: false
            ));

            dictionary.Add(RegisterId.BC, new Register
            (
                RegisterId.BC,
                x => dictionary[RegisterId.B].Data << 8 | dictionary[RegisterId.C].Data,
                x =>
                {
                    dictionary[RegisterId.B].Data = x >> 8;
                    dictionary[RegisterId.C].Data = x & 0x00FF;
                    return x;
                },
                isByte: false
            ));

            dictionary.Add(RegisterId.DE, new Register
            (
                RegisterId.DE,
                x => dictionary[RegisterId.D].Data << 8 | dictionary[RegisterId.E].Data,
                x =>
                {
                    dictionary[RegisterId.D].Data = x >> 8;
                    dictionary[RegisterId.E].Data = x & 0x00FF;
                    return x;
                },
                isByte: false
            ));

            dictionary.Add(RegisterId.HL, new Register
            (
                RegisterId.HL,
                x => dictionary[RegisterId.H].Data << 8 | dictionary[RegisterId.L].Data,
                x =>
                {
                    dictionary[RegisterId.H].Data = x >> 8;
                    dictionary[RegisterId.L].Data = x & 0x00FF;
                    return x;
                },
                isByte: false
            ));

            /*
             * PSUEDO REGISTERS - My own DSL for the following:
             * These are for things like (HL)
             * I.E get/set the memory at the address stored in the register HL
             * Uses: (BC), (DE), (HL), (C)
             */
            var ramDictionary = new Dictionary<RegisterId, Register>();
            ramDictionary.Add(RegisterId.HL & RegisterId.FROM_RAM, new Register
            (
                RegisterId.HL & RegisterId.FROM_RAM,
                x => Bus.MMU.GetByte(dictionary[RegisterId.H].Data << 8 | dictionary[RegisterId.L].Data).Data,
                x =>
                {
                    Bus.MMU.SetByte(dictionary[RegisterId.HL].Data, x);
                    return x;
                }
            ));

            ramDictionary.Add(RegisterId.BC & RegisterId.FROM_RAM, new Register
            (
                RegisterId.BC & RegisterId.FROM_RAM,
                x => Bus.MMU.GetByte(dictionary[RegisterId.B].Data << 8 | dictionary[RegisterId.C].Data).Data,
                x =>
                {
                    Bus.MMU.SetByte(dictionary[RegisterId.BC].Data, x);
                    return x;
                }
            ));

            ramDictionary.Add(RegisterId.DE & RegisterId.FROM_RAM, new Register
            (
                RegisterId.DE & RegisterId.FROM_RAM,
                x => Bus.MMU.GetByte(dictionary[RegisterId.D].Data << 8 | dictionary[RegisterId.E].Data).Data,
                x =>
                {
                    Bus.MMU.SetByte(dictionary[RegisterId.DE].Data, x);
                    return x;
                }
            ));

            ramDictionary.Add(RegisterId.C & RegisterId.FROM_RAM, new Register
            (
                RegisterId.C & RegisterId.FROM_RAM,
                x => Bus.MMU.GetByte(0xFF00 + dictionary[RegisterId.C].Data).Data,
                x =>
                {
                    Bus.MMU.SetByte(0xFF00 + dictionary[RegisterId.DE].Data, x);
                    return x;
                }
            ));

            _registers = dictionary;
            _ramRegisters = ramDictionary;
        }
        
        public Register this[RegisterId id] => id.HasFlag(RegisterId.FROM_RAM) ? _ramRegisters[id] : _registers[id];
    }
}