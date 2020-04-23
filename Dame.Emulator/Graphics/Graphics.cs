using System;
using Dame.Emulator.Graphics.Rendering;

namespace Dame.Emulator.Graphics
{
    public class Graphics : ISynchronizable
    {
        private readonly IRenderer renderer;

        private GraphicsMode currentState;
        private int currentLine;
        private int ticksSinceStateChange;

        public GraphicsMode CurrentState => currentState;

        public Graphics(IRenderer renderer)
        {
            this.renderer = renderer;

            currentState = GraphicsMode.OAMSearch;
            currentLine = 0;
            ticksSinceStateChange = 0;
        }

        public void Cycle(int cycles = 1)
        {
            ticksSinceStateChange += cycles;

            var machineSatisfied = false;

            /* state machine:
             *
             *   OAMSearch -> PixelTransfer -> HBlank -> VBlank
             *       ⋀                            |
             *       \----------------------------/
             */

            while (!machineSatisfied)
            {
                switch (currentState)
                {
                    case GraphicsMode.OAMSearch:
                        if (ticksSinceStateChange >= 20)
                        {
                            ticksSinceStateChange = ticksSinceStateChange % 20;
                            SetState(GraphicsMode.PixelTransfer);
                        }
                        else machineSatisfied = true;

                        break;

                    case GraphicsMode.PixelTransfer:
                        if (ticksSinceStateChange >= 43)
                        {
                            ticksSinceStateChange = ticksSinceStateChange % 43;
                            SetState(GraphicsMode.HBlank);
                        }
                        else machineSatisfied = true;

                        break;

                    case GraphicsMode.HBlank:
                        if (ticksSinceStateChange >= 51)
                        {
                            ticksSinceStateChange = ticksSinceStateChange % 51;
                            if (currentLine >= 144)
                            {
                                SetState(GraphicsMode.VBlank);
                            }
                            else
                            {
                                ++currentLine;
                                SetState(GraphicsMode.OAMSearch);
                            }
                        }
                        else machineSatisfied = true;

                        break;

                    case GraphicsMode.VBlank:
                        if (ticksSinceStateChange >= 154 * 10)
                        {
                            ticksSinceStateChange = ticksSinceStateChange % (154 * 10);
                            currentLine = 0;

                            SetState(GraphicsMode.OAMSearch);
                        }
                        else machineSatisfied = true;
                        break;
                }
            }
        }

        private void SetState(GraphicsMode state)
        {
            currentState = state;

            // TODO: handle interrupts, block VRAM access
            switch (currentState)
            {
                case GraphicsMode.OAMSearch:

                    break;

                case GraphicsMode.PixelTransfer:

                    break;

                case GraphicsMode.HBlank:

                    break;

                case GraphicsMode.VBlank:
                    Render();
                    break;
            }
        }

        private void Render()
        {
            // TODO: copy all the relevant data to the rendering context if it was modified
            renderer.Render();
        }
    }
}
