using System;
using Dame.Emulator.Memory.Blocks;

namespace Dame.Emulator.Graphics.Blocks
{
    public class VideoMemoryBlock : IModifyBlock, IEmulationState
    {
        private readonly IGraphics graphics;

        private byte nullByte = 0xFF;

        private int bufferOffset = 0;
        private byte[] memory;

        public bool BufferModified { get; private set; }
        public ReadOnlySpan<byte> Raw => new ReadOnlySpan<byte>(memory);

        public VideoMemoryBlock(IGraphics graphics, int size)
        {
            this.graphics = graphics;
            this.memory = new byte[size * 2];
        }

        public ref byte Get(int address)
        {
            ThrowIfOutOfBounds(address);

            if (graphics.CurrentState == GraphicsMode.PixelTransfer)
            {
                nullByte = 0xFF;
                return ref nullByte; // TODO: log for debugging purposes
            }

            return ref memory[bufferOffset + address];
        }

        public byte Read(int address)
        {
            ThrowIfOutOfBounds(address);

            if (graphics.CurrentState == GraphicsMode.PixelTransfer)
                return 0xFF; // TODO: log for debugging purposes

            return memory[bufferOffset + address];
        }

        public void Write(int address, byte value)
        {
            ThrowIfOutOfBounds(address);

            if (graphics.CurrentState == GraphicsMode.PixelTransfer)
                return; // TODO: log for debugging purposes

            // TODO: write value to an applicable buffer depending on the rendering phase

            // when VRAM is accessed during OAM search and HBlank phases of the rendering pipeline, the value should be written:
            // - to both buffers: if the value written is ahead of the currently rendered pixel
            // - to the primary buffer: if the value is behind

            BufferModified = true;

            memory[bufferOffset + address] = value;
        }

        public void BufferSwap()
        {
            var newOffset = bufferOffset == 0 ? memory.Length / 2 : 0;

            var primaryBuffer = new ReadOnlySpan<byte>(memory, bufferOffset, memory.Length / 2);
            var secondaryBuffer = new Span<byte>(memory, newOffset, memory.Length / 2);

            primaryBuffer.CopyTo(secondaryBuffer);

            bufferOffset = newOffset;
            BufferModified = false;
        }

        #region Snapshots

        public ReadOnlySpan<byte> CreateSnapshot()
            => memory; // TODO: include buffer offset in the snapshot

        public void RestoreSnapshot(ReadOnlySpan<byte> snapshot)
            => snapshot.CopyTo(memory);

        #endregion

        private void ThrowIfOutOfBounds(int address)
        {
            if (address >= memory.Length / 2)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));
        }
    }
}
