using System.Diagnostics;
using System.Threading;

namespace Dame.Architecture
{
    delegate void TickHandler();

    class ProcessorExecutionContext
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

        public event TickHandler Tick;

        /// <summary>
        /// Base clock frequency, used as a foundation for
        /// CPU and PPU clock frequencies.
        /// </summary>
        /// <value>Clock frequency in Hz.</value>
        public uint Frequency { get; set; } = Freq1MHz;

        public void Start()
        {
            tick = 0;
            lastSync = 0;
            stopwatch.Restart();
        }

        public void Step()
        {
                Tick?.Invoke();
                ++tick;
        }

        public void SyncWait()
        {
            var tickFrequency = Stopwatch.Frequency;
            var ticksPerCycle = Frequency != FreqUnlimited
                ? tickFrequency / (long) Frequency
                : 0;
            
            var syncTime = ticksPerCycle * (tick - lastSync);
            
            while (syncTime < stopwatch.ElapsedTicks)
                    spinWait.SpinOnce();
            
            stopwatch.Restart();
            lastSync = tick;
        }
    }
}