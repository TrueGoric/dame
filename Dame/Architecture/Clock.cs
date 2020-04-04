using System.Diagnostics;
using System.Threading;

namespace Dame.Architecture
{
    delegate void TickHandler(long tick);

    class Clock
    {
        #region Default Frequencies

        public const int Freq4MHz = 4 * 1024 * 1024;
        public const int Freq8MHz = 8 * 1024 * 1024;
        public const int FreqUnlimited = 0;

        #endregion

        public event TickHandler Tick;

        /// <summary>
        /// Base clock frequency, used as a foundation for
        /// CPU (div 4), PPU (div 4), RAM (div 1) and VRAM (div 2) clock
        /// frequencies.
        /// </summary>
        /// <value>Clock frequency in Hz.</value>
        public uint Frequency { get; set; } = Freq4MHz;

        public void Run(CancellationToken cancellation)
        {
            var tickFrequency = Stopwatch.Frequency;
            var ticksPerCycle = Frequency != FreqUnlimited
                ? tickFrequency / (long) Frequency
                : 0;

            var spinWait = new SpinWait();
            var watch = new Stopwatch();
            
            var tick = 0L;

            while (!cancellation.IsCancellationRequested)
            {
                watch.Restart();

                Tick?.Invoke(tick);
                ++tick;

                while (ticksPerCycle < watch.ElapsedTicks)
                    spinWait.SpinOnce();
            }
        }
    }
}