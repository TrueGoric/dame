namespace Dame.Architecture
{
    partial class Processor
    {
        #region Loads

        private static void Load8(ref byte to, ref byte from) => to = from;
        private static void Load16(ref ushort to, ref ushort from) => to = from;

        #endregion

        #region Arithmetic

        private static void Increment8(ref byte value, ref byte flags)
        {
            ++value;

            flags.RemoveFlag(ProcessorFlags.Arithmetic);

            if ((value & 0b1111) == 0)
            {
                flags.ApplyFlag(ProcessorFlags.NibbleCarry);

                if (value == 0)
                    flags.ApplyFlag(ProcessorFlags.Zero);
            }
            else
            {
                flags.RemoveFlag(ProcessorFlags.NibbleCarry);
                flags.RemoveFlag(ProcessorFlags.Zero);
            }

        }

        private static void Decrement8(ref byte value, ref byte flags)
        {
            --value;

            flags.ApplyFlag(ProcessorFlags.Arithmetic);

            if ((value & 0b1111) == 0b1111)
                flags.ApplyFlag(ProcessorFlags.NibbleCarry);
            else
                flags.RemoveFlag(ProcessorFlags.NibbleCarry);

            if (value == 0)
                flags.ApplyFlag(ProcessorFlags.Zero);
            else
                flags.RemoveFlag(ProcessorFlags.Zero);
        }

        private static void Increment16(ref ushort value) => ++value;
        private static void Decrement16(ref ushort value) => --value;

        private static void Add8(ref byte to, byte amount, ref byte flags)
        {
            if ((to & 0b1111) + amount > 0b1111)
            {
                flags.ApplyFlag(ProcessorFlags.NibbleCarry);

                if (to + amount > byte.MaxValue)
                    flags.ApplyFlag(ProcessorFlags.Carry);
            }
            else
            {
                flags.RemoveFlag(ProcessorFlags.NibbleCarry);
                flags.RemoveFlag(ProcessorFlags.Carry);
            }

            to += amount;

            flags.RemoveFlag(ProcessorFlags.Arithmetic);

            if (to == 0)
                flags.ApplyFlag(ProcessorFlags.Zero);
            else
                flags.RemoveFlag(ProcessorFlags.Zero);
        }

        private static void Subtract8(ref byte to, byte amount, ref byte flags)
        {
            if ((to & 0b1111) - amount < 0)
            {
                flags.ApplyFlag(ProcessorFlags.NibbleCarry);

                if (to - amount < 0)
                    flags.ApplyFlag(ProcessorFlags.Carry);
            }
            else
            {
                flags.RemoveFlag(ProcessorFlags.NibbleCarry);
                flags.RemoveFlag(ProcessorFlags.Carry);
            }

            to -= amount;

            flags.ApplyFlag(ProcessorFlags.Arithmetic);

            if (to == 0)
                flags.ApplyFlag(ProcessorFlags.Zero);
            else
                flags.RemoveFlag(ProcessorFlags.Zero);
        }

        #endregion

    }
}