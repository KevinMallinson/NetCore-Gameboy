using System.Collections.Generic;
using System.Linq;
using Interfaces;
using MoreLinq;

namespace Hardware.System
{
    public class Gameboy
    {
        private readonly IROMReader _romReader;
        private List<byte> _rom;
        private readonly Queue<IExecutedOpcode> _opcodeHistory;
        private bool _haltAndCatchFire;

        public Gameboy(IROMReader romReader)
        {
            _romReader = romReader;
            _rom = new List<byte>();
            _opcodeHistory = new Queue<IExecutedOpcode>();
            Bus.Init();
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

        private void EnqueueOpcode(IExecutedOpcode opcode)
        {
            if (_opcodeHistory.Count == 100)
            {
                _opcodeHistory.Dequeue();
            }
            
            _opcodeHistory.Enqueue(opcode);
        }

        private void CheckForHaltAndCatchFire()
        {
            if (_opcodeHistory.Any(x => x.OpcodeType != OpcodeType.JP)) 
                return;

            var address = _opcodeHistory.First().NewProgramCounter;
            _haltAndCatchFire = _opcodeHistory.All(x => x.NewProgramCounter == address);
        }
    }
}