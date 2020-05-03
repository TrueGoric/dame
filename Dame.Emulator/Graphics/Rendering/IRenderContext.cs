using System.Collections.Generic;

namespace Dame.Emulator.Graphics.Rendering
{
    public interface IRenderContext
    {
        IRenderData TileData { get; }

        IRenderData TileMapTwo { get; }
        IRenderData TileMapOne { get; }

        void RegisterSwitch(Position position, GraphicsDataRegister data, byte value);
        void ClearSwitches();
    }
}