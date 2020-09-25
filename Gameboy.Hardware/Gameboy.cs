using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Gameboy.Interfaces;
using MoreLinq;

namespace Gameboy.Hardware
{
    public class GameboyDevice
    {
        private readonly IROMReader _romReader;
        private List<byte> _rom;
        private Queue<ExecutedOpcode> _opcodeHistory;
        private bool _haltAndCatchFire;

        public GameboyDevice(IROMReader romReader)
        {
            _romReader = romReader;
            _rom = new List<byte>();
            _opcodeHistory = new Queue<ExecutedOpcode>();
        }
        
        public void LoadROM(string source)
        {
            _rom = _romReader.ReadRom(source);
            _rom.ForEach((x, i) => Bus.MMU.SetByte((ushort)i, x));
        }

        public void Cycle()
        {
            while (!_haltAndCatchFire)
            {
                var opcode = Bus.CPU.Step();
                EnqueueOpcode(opcode);
                CheckForHaltAndCatchFire();
            }
        }

        private void EnqueueOpcode(ExecutedOpcode opcode)
        {
            if (_opcodeHistory.Count == 100)
            {
                _opcodeHistory.Dequeue();
            }
            
            _opcodeHistory.Enqueue(opcode);
        }

        private void CheckForHaltAndCatchFire()
        {
            if (_opcodeHistory.Any(x => x.OpcodeType != OpcodeType.JUMP)) 
                return;

            var address = _opcodeHistory.First().NewProgramCounter;
            _haltAndCatchFire = _opcodeHistory.All(x => x.NewProgramCounter == address);
        }
    }
}