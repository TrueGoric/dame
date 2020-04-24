using System;

namespace Dame.Emulator.Graphics
{
    public readonly struct Position
    {
        public static readonly Position Zero = new Position(0);
        public static readonly Position OneX = new Position(1, 0);
        public static readonly Position OneY = new Position(0, 1);
        
        public readonly int X;
        public readonly int Y;

        public Position(int xy)
        {
            X = xy;
            Y = xy;
        }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position position &&
                   X == position.X &&
                   Y == position.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Position first, Position second)
            => first.X == second.X && first.Y == second.Y;
        
        public static bool operator !=(Position first, Position second)
            => first.X != second.X || first.Y != second.Y;

        public static Position operator +(Position first, Position second)
            => new Position(first.X + second.X, first.Y + second.Y);
        
        public static Position operator -(Position first, Position second)
            => new Position(first.X - second.X, first.Y - second.Y);
        
        public static implicit operator Position(ValueTuple<int, int> source)
            => new Position(source.Item1, source.Item2);
    }
}
