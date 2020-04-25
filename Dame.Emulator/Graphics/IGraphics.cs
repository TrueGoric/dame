using Dame.Emulator.Graphics.Blocks;
using Dame.Emulator.Memory;

namespace Dame.Emulator.Graphics
{
    public interface IGraphics : IMemoryRegistrar
    {
        GraphicsMode CurrentState { get; }

        GraphicRegisters Registers { get; }

        VideoMemoryBlock TileData { get; }

        VideoMemoryBlock TileMapOne { get; }
        VideoMemoryBlock TileMapTwo { get; }

        void NotifyGraphicsDataChanged(GraphicsDataRegister data, byte value);
    }
}