using System.Collections.Generic;

namespace Dame.Graphics.Rendering
{
    interface IRenderContext
    {
        ITextureMap TileMap { get; }
        ITextureMap SpriteMap { get; }

        IDisplayMap Background { get; }
        IDisplayMap Window { get; }

        IList<(int, byte)> SCXSwitches { get; }
        IList<(int, byte)> SCYSwitches { get; }


        // TODO: set properties relating to window position, etc.
    }
}