// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Linq;
using Gameboy.Extensions;
using Gameboy.Interfaces;

namespace Gameboy.Hardware
{
    public class CPU : ICentralProcessingUnit
    {
        public int ProgramCounter { get; set; }
        public int StackPointer { get; set; }
        public IOHelper IOHelper { get; }
        public IReadOnlyDictionary<RegisterId, Register> Registers;
        private readonly LDHelper _ldHelper;

        public CPU()
        {
            IOHelper = new IOHelper();
            _ldHelper = new LDHelper(IOHelper);
            Registers = IOHelper.Registers;
        }
        
        public ExecutedOpcode LD(int opcode, int? data = null)
        {
            var mapping = _ldHelper[opcode]; 
            if (mapping.From.IsRegisterGroup && mapping.To.IsRegisterGroup && mapping.From.Id == mapping.To.Id)
            {
                throw new Exception($"From and To registers cannot both be {mapping.To.Id}.");
            }

            var executedOpcode = mapping.Parameter switch
            {
                LDParameter.NONE => LD(mapping),
                LDParameter.BYTE => LDImmediate(mapping, data),
                LDParameter.SHORT => LDImmediate(mapping, data),
                _ => throw new Exception($"Cannot understand LDParameter type {mapping.Parameter}")
            };
            
            var incrementValue = data == null ? 1 : mapping.Parameter == LDParameter.BYTE ? 2 : 3;
            ProgramCounter += incrementValue;
            return executedOpcode;
        }

        private ExecutedOpcode LD(LDMapping mapping)
        {
            IOHelper[mapping.To.Id].Data = IOHelper[mapping.From.Id].Data;
            IncrementRegister(mapping);
            ProgramCounter++;
            return new ExecutedOpcode(mapping.Cycles, mapping.Opcode, ProgramCounter, StackPointer, OpcodeType.LOAD);
        }
        
        private ExecutedOpcode LDImmediate(LDMapping mapping, int? data)
        {
            if (data == null)
            {
                throw new Exception($"Cannot have a null immediate data argument for opcode {mapping.Opcode}");
            }

            var valid16BitRegisters = new[] {RegisterId.AF, RegisterId.BC, RegisterId.DE, RegisterId.HL};
            if (mapping.Parameter == LDParameter.SHORT && !valid16BitRegisters.Contains(mapping.To.Id))
            {
                throw new Exception(
                    $"Cannot assign a short value {data.Value} to register {mapping.To.Id}. Opcode: {mapping.Opcode}");
            }
            
            IOHelper[mapping.To.Id].Data = data.Value;
            ProgramCounter += mapping.Parameter == LDParameter.BYTE ? 2 : 3;
            return new ExecutedOpcode(mapping.Cycles, mapping.Opcode, ProgramCounter, StackPointer, OpcodeType.LOAD);           
        }

        private static void IncrementRegister(LDMapping mapping)
        {
            if (mapping.From.IsRegisterGroup)
            {
                mapping.From.Data += mapping.IncrementValue;
            }
            else if (mapping.To.IsRegisterGroup)
            {
                mapping.To.Data += mapping.IncrementValue;
            }
        }

        public ExecutedOpcode JMP(GBMemory memory)
        {
            ProgramCounter = memory.Data;
            
            var opcode = $"JMP {memory.Data.ToLittleEndian()}";
            return new ExecutedOpcode(12, opcode, ProgramCounter, StackPointer, OpcodeType.JUMP);
        }
        
        public ExecutedOpcode Step()
        {
            var opcode = Bus.MMU.GetByte(ProgramCounter).Data;
            switch (opcode)
            {
                // Load Register to another Register
                case { } when opcode >= 0x40 && opcode <= 0x7f && opcode != 0x76: //0x76 is halt
                {
                    return LD(opcode);
                }
                case 0x76:
                {
                    //TODO: HALT
                    throw new NotImplementedException();
                }
                case 0x06:
                {
                    throw new NotImplementedException();
                    // var data = Bus.MMU.GetByte(ProgramCounter + 1);
                    // LastExecutedOpcode = LD( RegisterId.B, data);
                    // break;
                }
                case 0xC3:
                {
                    var data = Bus.MMU.GetWord(ProgramCounter + 1);
                    return JMP(data);
                }
            }
            
            throw new Exception($"Could not handle opcode {opcode}");
        }
    }
}