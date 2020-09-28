// ReSharper disable MemberCanBePrivate.Global

using System;
using Gameboy.Extensions;
using Gameboy.Interfaces;

namespace Gameboy.Hardware
{
    public class CPU : ICentralProcessingUnit
    {
        public int ProgramCounter { get; set; }
        public int StackPointer { get; set; }
        public RegisterDictionary Registers { get; }
        public LDHelper LDHelper { get; }

        public CPU()
        {
            Registers = new RegisterDictionary();
            LDHelper = new LDHelper(Registers);
        }
        
        public ExecutedOpcode LD(int opcode, int? data = null)
        {
            var mapping = LDHelper[opcode]; 
            if (mapping.From.IsRegisterGroup && mapping.To.IsRegisterGroup && mapping.From.Id == mapping.To.Id)
            {
                throw new Exception($"From and To registers cannot both be {mapping.To.Id}.");
            }

            return mapping.From.IsRegisterGroup ? LDFromMemory(mapping.To.Id, mapping.From.Id) :
                mapping.To.IsRegisterGroup ? LDToMemory(mapping.To.Id, mapping.From.Id) :
                LDFromRegister(mapping.To.Id, mapping.From.Id);
        }

        private ExecutedOpcode LDFromRegister(RegisterId dest, RegisterId src)
        {
            Registers[dest] = Registers[src];
            ProgramCounter++;
            
            var opcode = $"LD {dest.ToString()},{src.ToString()}";
            return new ExecutedOpcode(4, opcode, ProgramCounter, StackPointer, OpcodeType.LOAD);           
        }
        
        private ExecutedOpcode LDFromMemory(RegisterId dest, RegisterId src)
        {
            var address = Registers[src].Data;
            var memory = Bus.MMU.GetByte(address);
            Registers[dest].Data = memory.Data;

            var opcode = $"LD {dest.ToString()},(HL)";
            return new ExecutedOpcode(8, opcode, ProgramCounter, StackPointer, OpcodeType.LOAD);
        }

        private ExecutedOpcode LDToMemory(RegisterId dest, RegisterId src)
        {
            var address = Registers[dest].Data;
            var val = Registers[src].Data;
            Bus.MMU.SetByte(address, val);
 
            var opcode = $"LD (HL),{src.ToString()}";
            return new ExecutedOpcode(8, opcode, ProgramCounter, StackPointer, OpcodeType.LOAD);  
        }
        

        // public ExecutedOpcode LD(RegisterId dest, GBMemory memory)
        // {
        //     if (memory.IsByte)
        //     {
        //         Registers[dest].Data = memory.Data;
        //     }
        //     else
        //     {
        //         throw new NotImplementedException();
        //     }
        //     
        //     ProgramCounter += 2;
        //     
        //     var opcode = $"LD {dest.ToString()}, {memory.Data.ToLittleEndian()}";
        //     return new ExecutedOpcode(8, opcode, ProgramCounter, StackPointer, OpcodeType.LOAD);
        // }

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