using System.Collections.Generic;
using System.Linq;
using Dame.Emulator.Graphics;
using SFML.Graphics;
using SFML.System;
using static Dame.Renderer.RenderingConstants;

namespace Dame.Renderer.Pipelines
{
    class SelectTiles : Pipeline<VertexArray>
    {
        private VertexArray vertices;

        public LCDControl ControlSwitch { get; }

        public SelectTiles(SFMLRenderContext context, LCDControl control)
            : base(context)
        {
            vertices = new VertexArray(PrimitiveType.Quads, DisplayMapLength * 4); // 4 edges
        }

        public override VertexArray Apply(IEnumerable<GraphicsDataSwitch> switches)
        {
            vertices.Clear();

            // get all the LCDC register switches throughout the frame
            var tilesets = switches
                .Where(s => s.Register == GraphicsDataRegister.LCDC)
                .GetEnumerator();
            
            tilesets.MoveNext();

            var currentRegister = (LCDControl)tilesets.Current.Value;

            int nextPos;

            if (tilesets.MoveNext())
                nextPos = tilesets.Current.Position.Y * ScreenWidth + tilesets.Current.Position.X;
            else
                nextPos = int.MaxValue; // if there's no change till the end of the frame, we just stick with the last tileset

            // draw vertices based on what tiles should currently be displayed
            for (uint i = 0; i < DisplayMapLength; i++)
            {
                // // handle tileset switches
                // if (i * TileWidth >= nextPos) // TODO: handle mid-line tile changes
                // {
                //     currentRegister = (LCDControl)tilesets.Current.Value;

                //     if (tilesets.MoveNext())
                //         nextPos = tilesets.Current.Position.Y * ScreenWidth + tilesets.Current.Position.X;
                //     else
                //         nextPos = int.MaxValue; // if there's no change till the end of the frame, we just stick with the last tileset
                // }

                var tile = Context.SFMLTileMapOne.Data[i];// SelectTile(currentRegister, i);

                var tileX = (tile * TileWidth) % TilesetWidth;
                var tileY = (tile * TileWidth / TilesetWidth) * TileHeight;

                var x = i * TileWidth % BackbufferWidth;
                var y = (i * TileWidth / BackbufferWidth) * TileHeight;

                vertices.Append(new Vertex(new Vector2f(x, y), new Vector2f(tileX, tileY))); // top-left
                vertices.Append(new Vertex(new Vector2f(x + TileWidth, y), new Vector2f(tileX + TileWidth, tileY))); // top-right
                vertices.Append(new Vertex(new Vector2f(x + TileWidth, y + TileHeight), new Vector2f(tileX + TileWidth, tileY + TileHeight))); // bottom-right
                vertices.Append(new Vertex(new Vector2f(x, y + TileHeight), new Vector2f(tileX, tileY + TileHeight))); // bottom-left
            }

            return vertices;
        }

        private int SelectTile(LCDControl control, uint position)
            => (control & LCDControl.TileDataSelect) == 0
                ? (control & ControlSwitch) == 0
                    ? Context.SFMLTileMapOne.Data[position]
                    : Context.SFMLTileMapTwo.Data[position]
                : (control & ControlSwitch) == 0
                    ? Context.SFMLTileMapOne.Data[position] + 128
                    : Context.SFMLTileMapTwo.Data[position] + 128;
    }
}