using System;

namespace Dame.Emulator.Graphics
{
    public readonly struct GraphicsData : IEquatable<GraphicsData>
    {
        public byte SCY { get; }
        public byte SCX { get; }

        public byte WY { get; }
        public byte WX { get; }

        public byte BGP { get; }

        public byte OBP0 { get; }
        public byte OBP1 { get; }

        public bool Equals(GraphicsData other)
        {
            return ((SCY ^ other.SCY)
                  | (SCX ^ other.SCX)
                  | (WY ^ other.WY)
                  | (WX ^ other.WX)
                  | (BGP ^ other.BGP)
                  | (OBP0 ^ other.OBP0)
                  | (OBP1 ^ other.OBP1)) == 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphicsData data)
                return Equals(data);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (SCY + (SCX << 8) + (WY << 16) + (WX << 24)) ^ (BGP + (OBP0 << 8) + (OBP1 << 16));
        }

        public static bool operator ==(GraphicsData first, GraphicsData second)
            => first.Equals(second);

        public static bool operator !=(GraphicsData first, GraphicsData second)
            => !first.Equals(second);

        // TODO: GBC palette data
    }
}
