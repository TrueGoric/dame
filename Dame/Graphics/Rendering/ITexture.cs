using System;

namespace Dame.Graphics.Rendering
{
    interface ITexture
    {
        int Width { get; }
        int Height { get; }

        int X { get; }
        int Y { get; }

        int Position { get; }

        void WriteRawData(Span<byte> data);
    }
}