using System;
using Dame.Emulator.Memory.Blocks;

namespace Dame.Emulator.Graphics.Blocks
{
    public class TileMapMemoryBlock : IModifyBlock, IWriteBatchBlock, IEmulationState
    {
        private readonly Graphics graphics;

        private byte nullByte = 0x00;
        private byte[] memory;

        public TileMapMemoryBlock(Graphics graphics)
        {
            this.graphics = graphics;
            this.memory = new byte[3 * 16 * 128]; // three banks of 128 16-byte tiles
        }

        public ref byte Get(int address)
        {
            if (graphics.CurrentState == GraphicsMode.PixelTransfer)
                return ref nullByte; // TODO: log for debugging purposes
            
            return ref memory[address];
        }

        public byte Read(int address)
        {
            if (graphics.CurrentState == GraphicsMode.PixelTransfer)
                return 0xFF; // TODO: log for debugging purposes
            
            return memory[address];
        }

        public void Write(int address, byte value)
        {
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
        {
            throw new NotImplementedException();
        }

        public void RestoreSnapshot(ReadOnlySpan<byte> snapshot)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
