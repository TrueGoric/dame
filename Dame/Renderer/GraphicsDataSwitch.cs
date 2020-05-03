using Dame.Emulator.Graphics;

namespace Dame.Renderer
{
    readonly struct GraphicsDataSwitch
    {
        public readonly Position Position;
        public readonly GraphicsDataRegister Register;
        public readonly byte Value;

        public GraphicsDataSwitch(Position position, GraphicsDataRegister register, byte value)
        {
            Position = position;
            Register = register;
            Value = value;
        }
    }
}