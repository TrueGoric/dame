namespace Dame.Emulator.Graphics
{
    public enum LCDControl : byte
    {
        DisplayEnable = 1 << 7,
        WindowMapSelect = 1 << 6,
        WindowDisplayEnable = 1 << 5,
        TileDataSelect = 1 << 4,
        BackgroundMapSelect = 1 << 3,
        SpriteSize = 1 << 2,
        SpriteEnable = 1 << 1,
        BackgroundWindowSwitch = 1 << 0
    }
}