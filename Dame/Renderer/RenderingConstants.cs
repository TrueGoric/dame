using System;

namespace Dame.Renderer
{
    static class RenderingConstants
    {
        public const int ScreenWidth = 160; // pixels
        public const int ScreenHeight = 144;

        public const int BackbufferWidth = DisplayMapWidth * TileWidth;
        public const int BackbufferHeight = DisplayMapHeight * TileHeight;

        public const int TileWidth = 8; // pixels
        public const int TileHeight = 8; // pixels
        public const int TileLength = TileWidth * TileHeight;

        public const int TilesetWidth = 16 * TileWidth; // pixels
        public const int TilesetHeight = 3 * 8 * TileHeight; // pixels
        public const int TilesetLength = TilesetWidth * TilesetHeight;

        public const int DisplayMapWidth = 32; // in tiles
        public const int DisplayMapHeight = 32; // in tiles
        public const int DisplayMapLength = DisplayMapHeight * DisplayMapWidth;
    }
}
