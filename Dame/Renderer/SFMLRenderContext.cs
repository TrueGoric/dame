using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dame.Emulator.Graphics;
using Dame.Emulator.Graphics.Rendering;

namespace Dame.Renderer
{
    // a tough one to crack - this must be shared between the rendering thread and emulation thread,
    // be wary of occasional changes to texture maps and more frequent changes to display maps
    //
    // solution: all constituents should be made thread safe and dependent on the rendering thread
    internal class SFMLRenderContext : IRenderContext
    {
        private readonly SFMLRenderer renderer;

        public Queue<GraphicsDataSwitch> RegisterSwapQueue = new Queue<GraphicsDataSwitch>();

        public IRenderData TileData => SFMLTileData;

        public IRenderData TileMapOne => SFMLTileMapOne;
        public IRenderData TileMapTwo => SFMLTileMapTwo;

        public SFMLTextureMap SFMLTileData { get; }
        
        public SFMLDisplayMap SFMLTileMapOne { get; }
        public SFMLDisplayMap SFMLTileMapTwo { get; }

        public SFMLRenderContext(SFMLRenderer renderer)
        {
            this.renderer = renderer;

            SFMLTileData = new SFMLTextureMap(renderer);

            SFMLTileMapOne = new SFMLDisplayMap(renderer);
            SFMLTileMapTwo = new SFMLDisplayMap(renderer);
        }

        public void RegisterSwitch(Position position, GraphicsDataRegister data, byte value)
        {
            lock (renderer.RenderLock)
                RegisterSwapQueue.Enqueue(new GraphicsDataSwitch(position, data, value));
        }

        public void ClearSwitches()
        {
            lock (renderer.RenderLock)
                RegisterSwapQueue.Clear();
        }
    }
}
