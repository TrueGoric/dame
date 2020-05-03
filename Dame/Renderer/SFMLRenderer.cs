using System;
using System.Threading;
using Dame.Emulator.Graphics;
using Dame.Emulator.Graphics.Rendering;
using Dame.Renderer.Pipelines;
using SFML.Graphics;
using SFML.System;

namespace Dame.Renderer
{
    internal class SFMLRenderer : Drawable, IRenderer
    {
        private bool wasRenderRequested = false;

        private RenderTexture renderTexture;
        private SFMLRenderContext context;

        private SelectTiles selectBackgroundTilesPipeline;

        public readonly object RenderLock = new object(); // lock encapsulation? neverheardofit

        public IRenderContext RenderContext => context;

        public SFMLRenderer()
        {
            context = new SFMLRenderContext(this);

            selectBackgroundTilesPipeline = new SelectTiles(context, LCDControl.BackgroundMapSelect);
            // renderTexture = new RenderTexture()
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Thread.MemoryBarrier();

            if (wasRenderRequested)
            {
                Thread.MemoryBarrier();

                lock (RenderLock)
                {
                    var sfmlContext = context.SFMLTileData;

                    sfmlContext.UpdateTexture();

                    var backgroundVertices = selectBackgroundTilesPipeline.Apply(context.RegisterSwapQueue);

                    // var spr = new Sprite(sfmlContext.Texture);
                    // spr.Scale = new Vector2f(3f ,3f);
                    // target.Draw(spr);

                    states.Transform.Scale(3f, 3f);
                    states.Texture = sfmlContext.Texture;
                    target.Draw(backgroundVertices, states);

                    context.RegisterSwapQueue.Clear(); // temp

                    wasRenderRequested = false;
                }
            }
        }

        public void Render()
        {
            Thread.MemoryBarrier();

            wasRenderRequested = true;

            Thread.MemoryBarrier();
        }
    }
}
