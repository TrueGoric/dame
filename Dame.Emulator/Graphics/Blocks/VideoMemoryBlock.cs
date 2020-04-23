using System;
using Dame.Emulator.Memory.Blocks;

namespace Dame.Emulator.Graphics.Blocks
{
    public class VideoMemoryBlock : IModifyBlock, IWriteBatchBlock, IEmulationState
    {
        private readonly Graphics graphics;

        private byte nullByte = 0x00;
        private byte[] memory;

        public VideoMemoryBlock(Graphics graphics, int size)
        {
            this.graphics = graphics;
            this.memory = new byte[size];
        }

        public ref byte Get(int address)
        {
            ThrowIfOutOfBounds(address);
            
            if (graphics.CurrentState == GraphicsMode.PixelTransfer)
                return ref nullByte; // TODO: log for debugging purposes
            
            return ref memory[address];
        }

        public byte Read(int address)
        {
            ThrowIfOutOfBounds(address);
            
            if (graphics.CurrentState == GraphicsMode.PixelTransfer)
                return 0xFF; // TODO: log for debugging purposes
            
            return memory[address];
        }

        public void Write(int address, byte value)
        {
            ThrowIfOutOfBounds(address);

            if (graphics.CurrentState == GraphicsMode.PixelTransfer)
                return; // TODO: log for debugging purposes
            
            memory[address] = value;
        }

        public void Write(int address, Span<byte> data)
        {
            if (data.Length > memory.Length - address)
                throw new ArgumentException("Data is too large!", nameof(data));

            data.CopyTo(new Span<byte>(memory, address, data.Length));
        }

        #region Snapshots

        public ReadOnlySpan<byte> CreateSnapshot()
            => memory;

        public void RestoreSnapshot(ReadOnlySpan<byte> snapshot)
            => snapshot.CopyTo(memory);

        #endregion

        private void ThrowIfOutOfBounds(int address)
        {
            if (address >= memory.Length)
                throw new ArgumentException($"Address {address} is out of bounds!", nameof(address));
        }
    }
}
