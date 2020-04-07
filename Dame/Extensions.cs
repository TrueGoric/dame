using Dame.Processor;

namespace Dame
{
    static class Extensions
    {
        public static void SetFlag(this ref byte value, ProcessorFlags flags, bool condition)
        {
            if (condition)
                ApplyFlag(ref value, flags);
            else
                RemoveFlag(ref value, flags);
        }
        
        public static void ApplyFlag(this ref byte value, ProcessorFlags flag)
            => value |= (byte)flag;

        public static void RemoveFlag(this ref byte value, ProcessorFlags flag)
            => value &= (byte)~flag;
    }
}