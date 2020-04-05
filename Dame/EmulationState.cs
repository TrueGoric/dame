using System;

namespace Dame
{
    class EmulationState : IDisposable
    {
        public readonly byte[] Registers;

        public EmulationState()
        {
            Registers = new byte[12];
        }

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