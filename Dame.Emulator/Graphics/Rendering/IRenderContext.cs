using System.Collections.Generic;

namespace Dame.Emulator.Graphics.Rendering
{
    public interface IRenderContext
    {
        ITextureMap TileData { get; }

        IDisplayMap TileMapTwo { get; }
        IDisplayMap TileMapOne { get; }

        void RegisterSwitch(Position position, GraphicsDataRegister data, byte value);
    }
}