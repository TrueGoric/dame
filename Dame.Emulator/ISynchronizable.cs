using System;

namespace Dame.Emulator
{
    public interface ISynchronizable
    {
        void Cycle(int cycles = 1);
    }
}
