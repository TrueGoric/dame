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

        private Queue<(Vector2 Position, GraphicsData Data)> registerSwap;

        public ITextureMap TileMap => throw new NotImplementedException();

        public ITextureMap SpriteMap => throw new NotImplementedException();

        public IDisplayMap Background => throw new NotImplementedException();

        public IDisplayMap Window => throw new NotImplementedException();

        public void RegisterSwap(Vector2 position, GraphicsData data)
        {
            lock (renderer.RenderLock)
                registerSwap.Enqueue((position, data));
        }
    }
}
