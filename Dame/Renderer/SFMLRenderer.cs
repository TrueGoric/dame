using System;
using Dame.Graphics.Rendering;

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
