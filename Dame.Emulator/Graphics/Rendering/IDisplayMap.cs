using System;

namespace Dame.Emulator.Graphics.Rendering
{
    public interface IDisplayMap
    {
        ITexture this[byte position] { get; }
        ITexture this[byte x, byte y] { get; }

        void SetRaw(int position, byte tileRef);

        void WriteRaw(ReadOnlySpan<byte> map);
    }
}