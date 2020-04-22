using System;
using Dame.Emulator.Graphics.Rendering;

namespace Dame.Renderer
{
    public class SFMLTexture : ITexture
    {
        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public int X => throw new NotImplementedException();

        public int Y => throw new NotImplementedException();

        public int Position => throw new NotImplementedException();

        public void WriteRawData(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }
}
