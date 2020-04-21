using System;
using System.Collections.Generic;

namespace Dame.Graphics.Rendering
{
    interface ITextureMap
    {
        ITexture this[ushort index] { get; }
        ITexture this[byte x, byte y] { get; }

        void WriteRawTileData(Span<byte> data);
    }
}