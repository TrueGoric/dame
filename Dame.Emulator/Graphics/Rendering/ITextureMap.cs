using System;
using System.Collections.Generic;

namespace Dame.Emulator.Graphics.Rendering
{
    public interface ITextureMap
    {
        ITexture this[ushort index] { get; }
        ITexture this[byte x, byte y] { get; }

        void WriteRaw(ReadOnlySpan<byte> data, int offset = 0);
    }
}