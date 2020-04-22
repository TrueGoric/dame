using System;
using System.Threading;
using Dame.Emulator.Graphics.Rendering;
using SFML.Graphics;

namespace Dame.Renderer
{
    internal class SFMLRenderer : Drawable, IRenderer
    {
        public readonly object RenderLock = new object(); // lock encapsulation? neverheardofit

        private bool wasRenderRequested = false;

        private RenderTexture renderTexture;
        private SFMLRenderContext context;

        public IRenderContext RenderContext => context;

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (wasRenderRequested)
            {
                renderTexture.Clear();

                // TODO: rendering

                renderTexture.Display();

                target.Draw(new Sprite(renderTexture.Texture));

                wasRenderRequested = false;

                Monitor.Exit(RenderLock);
            }
        }

        public void Render()
        {
            Monitor.Enter(RenderLock);

            wasRenderRequested = true;
        }
    }
}
