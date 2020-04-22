using System;

namespace Dame.Emulator.Graphics.Rendering
{
    public interface ITexture
    {
        int Width { get; }
        int Height { get; }

        int X { get; }
        int Y { get; }

        int Position { get; }

        void WriteRawData(Span<byte> data);
    }
}