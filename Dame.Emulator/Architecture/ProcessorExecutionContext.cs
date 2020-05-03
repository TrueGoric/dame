using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Dame.Emulator.Architecture
{
    public class ProcessorExecutionContext
    {
        #region Default Frequencies

        public const int Freq1MHz = 1 * 1024 * 1024;
        public const int Freq2MHz = 2 * 1024 * 1024;
        public const int FreqUnlimited = 0;

        #endregion

        private readonly Stopwatch stopwatch = new Stopwatch();
        private readonly SpinWait spinWait = new SpinWait();
        private long lastSync;
        private long tick; // TODO: handle overflow

        private List<ISynchronizable> synchronizables = new List<ISynchronizable>();

        /// <summary>
        /// Base clock frequency, used as a foundation for
        /// CPU and PPU clock frequencies.
        /// </summary>
        /// <value>Clock frequency in Hz.</value>
        public uint Frequency { get; set; } = Freq1MHz;

        public long Ticks => tick;

        public void Register(ISynchronizable synchronizable)
        {
            if (!synchronizables.Contains(synchronizable))
                synchronizables.Add(synchronizable);
        }

        public void Start()
        {
            tick = 0;
            lastSync = 0;
            stopwatch.Restart();
        }

        public void Cycle(int cycles = 1)
        {
            // TODO: use compiled expressions for faster execution (eliminating callvirt)

            foreach (var synchronizable in synchronizables)
                synchronizable.Cycle(cycles);

            ++tick;
        }

        public void SyncWait()
        {
            var tickFrequency = Stopwatch.Frequency;
            var ticksPerCycle = Frequency != FreqUnlimited
                ? tickFrequency / (long)Frequency
                : 0;

            var syncTime = ticksPerCycle * (tick - lastSync);

            while (syncTime < stopwatch.ElapsedTicks)
                spinWait.SpinOnce();

            stopwatch.Restart();
            lastSync = tick;
        }
    }
}