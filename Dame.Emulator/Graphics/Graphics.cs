using System;
using Dame.Emulator.Graphics.Blocks;
using Dame.Emulator.Graphics.Rendering;
using Dame.Emulator.Memory;

namespace Dame.Emulator.Graphics
{
    public class Graphics : IGraphics, ISynchronizable
    {
        private readonly IRenderer renderer;

        private GraphicsMode currentState;
        private Position currentPos;
        private int ticksSinceStateChange;

        public GraphicsMode CurrentState => currentState;

        public GraphicRegisters Registers { get; }

        public VideoMemoryBlock TileData { get; }

        public VideoMemoryBlock TileMapOne { get; }

        public VideoMemoryBlock TileMapTwo { get; }

        public Graphics(IRenderer renderer)
        {
            this.renderer = renderer;

            Registers = new GraphicRegisters(this);
            TileData = new VideoMemoryBlock(this, 3 * 16 * 128);

            TileMapOne = new VideoMemoryBlock(this, 32 * 32);
            TileMapTwo = new VideoMemoryBlock(this, 32 * 32);

            currentState = GraphicsMode.OAMSearch;
            currentPos = new Position(0, -1);
            ticksSinceStateChange = 0;
        }

        public void RegisterBlocks(MemoryController controller)
        {
            controller.AddBlock(0x8000..0x9800, TileData);

            controller.AddBlock(0x9800..0x9C00, TileMapOne);
            controller.AddBlock(0x9C00..0xA000, TileMapTwo);

            controller.AddBlock(0xFF40..0xFF4C, Registers);
        }

        public void NotifyGraphicsDataChanged(GraphicsDataRegister data, byte value)
        {
            if (CurrentState != GraphicsMode.VBlank)
                renderer.RenderContext.RegisterSwitch(currentPos, data, value);
        }

        public void Cycle(int cycles = 1)
        {
            /* state machine:
             *
             *   OAMSearch -> PixelTransfer -> HBlank -> VBlank
             *       ⋀                            |
             *       \----------------------------/
             */

            for (int i = 0; i < cycles; i++)
            {
                ++ticksSinceStateChange;

                switch (currentState)
                {
                    case GraphicsMode.OAMSearch:
                        if (ticksSinceStateChange >= 20)
                            SetPixelTransfer();

                        break;

                    case GraphicsMode.PixelTransfer:
                        currentPos += Position.OneX;

                        if (ticksSinceStateChange >= 43)
                            SetHBlank();

                        break;

                    case GraphicsMode.HBlank:
                        if (ticksSinceStateChange >= 51)
                        {
                            if (currentPos.Y >= 144)
                            {
                                SetVBlank();
                            }
                            else
                            {
                                SetOAMSearch();
                            }
                        }

                        break;

                    case GraphicsMode.VBlank:
                        if (ticksSinceStateChange % 114 == 0)
                            ++Registers.LY;
                            
                        if (ticksSinceStateChange >= 114 * 10)
                            SetOAMSearch();

                        break;
                }
            }
        }

        private void SetOAMSearch()
        {
            // TODO: handle interrupts
            currentState = GraphicsMode.OAMSearch;
            ticksSinceStateChange = 0;

            Registers.LY = (byte)(currentPos.Y + 1);
        }

        private void SetPixelTransfer()
        {
            // TODO: handle interrupts
            currentState = GraphicsMode.PixelTransfer;
            ticksSinceStateChange = 0;

            currentPos = new Position(0, currentPos.Y + 1);

            if (currentPos == Position.Zero)
            {
                renderer.RenderContext.RegisterSwitch(Position.Zero, GraphicsDataRegister.LCDC, (byte)Registers.LCDC);
                renderer.RenderContext.RegisterSwitch(Position.Zero, GraphicsDataRegister.SCY, Registers.SCY);
                renderer.RenderContext.RegisterSwitch(Position.Zero, GraphicsDataRegister.SCX, Registers.SCX);
                renderer.RenderContext.RegisterSwitch(Position.Zero, GraphicsDataRegister.WY, Registers.WY);
                renderer.RenderContext.RegisterSwitch(Position.Zero, GraphicsDataRegister.WX, Registers.WX);
                renderer.RenderContext.RegisterSwitch(Position.Zero, GraphicsDataRegister.BGP, Registers.BGP);
                renderer.RenderContext.RegisterSwitch(Position.Zero, GraphicsDataRegister.OBP0, Registers.OBP0);
                renderer.RenderContext.RegisterSwitch(Position.Zero, GraphicsDataRegister.OBP1, Registers.OBP1);
            }
        }

        private void SetHBlank()
        {
            // TODO: handle interrupts
            currentState = GraphicsMode.HBlank;
            ticksSinceStateChange = 0;
        }

        private void SetVBlank()
        {
            // TODO: handle interrupts
            currentState = GraphicsMode.VBlank;
            ticksSinceStateChange = 0;
            
            currentPos = new Position(0, -1);

            Render();
        }

        private void Render()
        {
            if (TileData.BufferModified)
                renderer.RenderContext.TileData.WriteRaw(TileData.Raw);

            if (TileMapOne.BufferModified)
                renderer.RenderContext.TileMapOne.WriteRaw(TileMapOne.Raw);
            if (TileMapTwo.BufferModified)
                renderer.RenderContext.TileMapTwo.WriteRaw(TileMapTwo.Raw);

            renderer.Render();
        }
    }
}
