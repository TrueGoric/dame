namespace Dame.Emulator.Graphics
{
    public enum GraphicsMode
    {
        HBlank = 0b00,
        VBlank = 0b01,
        OAMSearch = 0b10,
        PixelTransfer = 0b11
    }
}