using System;

namespace Dame.Emulator.Graphics
{
    public readonly struct Vector2
    {
        public readonly int X;
        public readonly int Y;

        public Vector2(int xy)
        {
            X = xy;
            Y = xy;
        }

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
