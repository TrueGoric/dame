using Dame.Processor;

namespace Dame
{
    static class Extensions
    {
        public static void ApplyFlag(this ref byte value, ProcessorFlags flag)
            => value |= (byte)flag;

        public static void RemoveFlag(this ref byte value, ProcessorFlags flag)
            => value &= (byte)~flag;
    }
}