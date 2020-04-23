using System;

namespace Dame.Emulator
{
    public interface IEmulationState
    {
        ReadOnlySpan<byte> CreateSnapshot();

        void RestoreSnapshot(ReadOnlySpan<byte> snapshot);
    }
}
