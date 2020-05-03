using System;

namespace Dame.Emulator.Graphics.Rendering
{
    public interface IRenderData
    {
        void WriteRaw(ReadOnlySpan<byte> map);
    }
}