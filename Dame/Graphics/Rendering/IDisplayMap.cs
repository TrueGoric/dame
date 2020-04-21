using System;

namespace Dame.Graphics.Rendering
{
    interface IDisplayMap
    {
        ITextureMap TextureMap { get; set; }

        ITexture this[ushort position] { get; }
        ITexture this[byte x, byte y] { get; }

        void SetRaw(int position, ushort tileRef);

        void SetRaw(Span<ushort> map);
    }
}