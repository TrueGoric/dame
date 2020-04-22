using System;
using Dame.Emulator.Graphics.Rendering;

namespace Dame.Renderer
{
    internal class SFMLTexture : ITexture
    {
        private readonly SFMLRenderer renderer;
        public int Width => throw new NotImplementedException();

        public int Height => throw new NotImplementedException();

        public int X => throw new NotImplementedException();

        public int Y => throw new NotImplementedException();

        public int Position => throw new NotImplementedException();

        public void WriteRawData(Span<byte> data)
        {
            lock (renderer.RenderLock)
            {
                
            }
        }
    }
}
