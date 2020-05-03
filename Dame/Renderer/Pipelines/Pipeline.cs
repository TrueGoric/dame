using System.Collections.Generic;

namespace Dame.Renderer.Pipelines
{
    abstract class Pipeline<T>
    {
        public SFMLRenderContext Context { get; }

        public Pipeline(SFMLRenderContext context)
        {
            Context = context;
        }

        public abstract T Apply(IEnumerable<GraphicsDataSwitch> switches);
    }
}