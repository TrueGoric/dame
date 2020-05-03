using System.Collections.Generic;
using SFML.Graphics;

namespace Dame.Renderer.Pipelines
{
    class ProcessTileData : Pipeline<byte[]>
    {
        public ProcessTileData(SFMLRenderContext context)
            : base(context)
        { }
        
        public override byte[] Apply(IEnumerable<GraphicsDataSwitch> switches)
        {
            throw new System.NotImplementedException();
        }
    }
}