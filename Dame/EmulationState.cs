using System;
using System.Collections.Generic;

namespace Dame
{
    class EmulationState : IDisposable
    {
        private readonly Dictionary<string, byte> options;
        
        public readonly byte[] Registers;
        public readonly byte[] Memory;

        public EmulationState(byte[] memory)
        {
            Registers = new byte[12];
            Memory = memory;

            options = new Dictionary<string, byte>();
        }
        
        public byte GetOption(string option)
            => options.ContainsKey(option) ? options[option] : default;

        public void SetOption(string option, byte value)
            => options[option] = value;
        
        public EmulationState CreateSnapshot()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}