using System;

namespace Dame.Emulator.Graphics
{
    public enum GraphicsDataRegister
    {
        LCDC,
        
        SCY,
        SCX,

        WY,
        WX,

        BGP,

        OBP0,
        OBP1,

        // TODO: DMA
        // TODO: GBC palette data
    }
}
