using System;
using Dame.Emulator.Graphics.Rendering;

namespace Dame.Renderer
{
    public class SFMLRenderer : IRenderer
    {
        private SFMLRenderContext context;

        public IRenderContext RenderContext => context;

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
