using System;
using Dame.Emulator.Graphics.Rendering;

namespace Dame.Renderer
{
    internal class SFMLTextureMap : ITextureMap
    {
        private readonly SFMLRenderer renderer;

        public ITexture this[ushort index] => throw new NotImplementedException();

        public ITexture this[byte x, byte y] => throw new NotImplementedException();

        public void WriteRawTileData(Span<byte> data, int offset = 0)
        {
            lock (renderer.RenderLock)
            {
                
            }
        }
    }
}
