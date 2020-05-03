using System;
using System.Runtime.InteropServices;
using Dame.Emulator.Graphics.Rendering;
using SFML.Graphics;
using static Dame.Renderer.RenderingConstants;

namespace Dame.Renderer
{
    internal class SFMLTextureMap : IRenderData
    {
        private readonly SFMLRenderer renderer;
        private readonly uint[] ppuBuffer;
        private readonly byte[] swapBuffer;

        private Texture texture;

        public Texture Texture => texture;

        public SFMLTextureMap(SFMLRenderer renderer)
        {
            this.renderer = renderer;
            this.ppuBuffer = new uint[TilesetWidth * TilesetHeight];
            this.swapBuffer = new byte[TilesetWidth * TilesetHeight * 4];

            texture = new Texture(TilesetWidth, TilesetHeight); // 16 x 24 tiles
        }

        public void UpdateTexture()
        {
            // meh
            MemoryMarshal.Cast<uint, byte>(ppuBuffer).CopyTo(swapBuffer);

            texture.Update(swapBuffer);
        }

        public void WriteRaw(ReadOnlySpan<byte> data)
        {
            lock (renderer.RenderLock)
            {
                for (int i = 0; i < data.Length; i += 2)
                {
                    // i have so many questions
                    var tileLine = Interweave(data[i], data[i + 1]);

                    var loc = CalculateBufferLocation(i * 4);
                    
                    // TODO: apply color palletes in the fragment shader
                    ppuBuffer[loc]     = (uint)( (tileLine >> 14       ) * (uint)0x55555555);
                    ppuBuffer[loc + 1] = (uint)(((tileLine >> 12) & 0x3) * (uint)0x55555555);
                    ppuBuffer[loc + 2] = (uint)(((tileLine >> 10) & 0x3) * (uint)0x55555555);
                    ppuBuffer[loc + 3] = (uint)(((tileLine >> 8)  & 0x3) * (uint)0x55555555);
                    ppuBuffer[loc + 4] = (uint)(((tileLine >> 6)  & 0x3) * (uint)0x55555555);
                    ppuBuffer[loc + 5] = (uint)(((tileLine >> 4)  & 0x3) * (uint)0x55555555);
                    ppuBuffer[loc + 6] = (uint)(((tileLine >> 2)  & 0x3) * (uint)0x55555555);
                    ppuBuffer[loc + 7] = (uint)( (tileLine        & 0x3) * (uint)0x55555555);
                }

                //texture.Update(buffer);
            }
        }

        private int CalculateBufferLocation(int loc)
            => (loc % TileWidth)                                                       // tile width offset
             + ((loc % TileLength) / TileHeight * TilesetWidth)                        // tile height offset
             + (loc / TileLength * TileWidth)                                          // width offset
             + (loc / (TileWidth * TilesetWidth) * (TilesetWidth * (TileHeight - 1))); // height offset
        
        private ushort Interweave(byte first, byte second)
        {
            int op1 = first, op2 = second;

            op1 = (op1 | (op1 << 4)) & 0x0F0F;
            op1 = (op1 | (op1 << 2)) & 0x3333;
            op1 = (op1 | (op1 << 1)) & 0x5555;

            op2 = (op2 | (op2 << 4)) & 0x0F0F;
            op2 = (op2 | (op2 << 2)) & 0x3333;
            op2 = (op2 | (op2 << 1)) & 0x5555;

            return (ushort)(op1 << 1 | op2);
        }
    }
}
