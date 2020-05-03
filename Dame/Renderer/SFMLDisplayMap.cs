using System;
using Dame.Emulator.Graphics.Rendering;
using SFML.Graphics;
using static Dame.Renderer.RenderingConstants;

namespace Dame.Renderer
{
    internal class SFMLDisplayMap : IRenderData
    {
        private readonly SFMLRenderer renderer;

        private byte[] raw;

        public byte[] Data => raw;

        public SFMLDisplayMap(SFMLRenderer renderer)
        {
            this.renderer = renderer;

            raw = new byte[DisplayMapLength];
        }

        public void WriteRaw(ReadOnlySpan<byte> map)
        {
            lock (renderer.RenderLock)
            {
                map.CopyTo(raw);
            }
        }
    }
}