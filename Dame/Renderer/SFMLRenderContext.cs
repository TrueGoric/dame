using System;
using System.Collections.Generic;
using Dame.Graphics;
using Dame.Graphics.Rendering;

namespace Dame.Renderer
{
    public class SFMLRenderContext : IRenderContext
    {
        public ITextureMap TileMap => throw new NotImplementedException();

        public ITextureMap SpriteMap => throw new NotImplementedException();

        public IDisplayMap Background => throw new NotImplementedException();

        public IDisplayMap Window => throw new NotImplementedException();

        public IList<(int, GraphicsFlags)> RegisterSwitches => throw new NotImplementedException();
    }
}
