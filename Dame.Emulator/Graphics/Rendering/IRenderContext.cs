using System.Collections.Generic;

namespace Dame.Emulator.Graphics.Rendering
{
    public interface IRenderContext
    {
        ITextureMap TileMap { get; }
        ITextureMap SpriteMap { get; }

        IDisplayMap Background { get; }
        IDisplayMap Window { get; }

        void RegisterSwap(Vector2 position, GraphicsData data);
    }
}