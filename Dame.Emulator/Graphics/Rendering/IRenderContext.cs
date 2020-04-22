using System.Collections.Generic;

namespace Dame.Graphics.Rendering
{
    public interface IRenderContext
    {
        ITextureMap TileMap { get; }
        ITextureMap SpriteMap { get; }

        IDisplayMap Background { get; }
        IDisplayMap Window { get; }

        IList<(int, GraphicsFlags)> RegisterSwitches { get; }


        // TODO: set properties relating to window position, etc.
    }
}