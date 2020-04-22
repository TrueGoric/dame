using System;
using Dame.Emulator.Graphics.Rendering;

namespace Dame.Renderer
{
    public class SFMLTextureMap : ITextureMap
    {
        public ITexture this[ushort index] => throw new NotImplementedException();

        public ITexture this[byte x, byte y] => throw new NotImplementedException();

        public void WriteRawTileData(Span<byte> data, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
