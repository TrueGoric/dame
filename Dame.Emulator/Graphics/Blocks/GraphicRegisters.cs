using System;
using Dame.Emulator.Memory.Blocks;

namespace Dame.Emulator.Graphics.Blocks
{
    public class GraphicRegisters : IReadBlock, IWriteBlock
    {
        private readonly IGraphics graphics;

        private byte[] memory;

        public LCDControl LCDC
        {
            // 0xFF40
            get => (LCDControl)memory[0];
            set => memory[0] = (byte)value;
        }

        // TODO: STAT (0xFF41) interrputs implementation

        public byte SCY
        {
            // 0xFF42
            get => memory[2]; 
            set => memory[2] = value;
        }

        public byte SCX
        {
            // 0xFF43
            get => memory[3];
            set => memory[3] = value;
        }

        public byte LY
        {
            // 0xFF44
            get => memory[4];
            set => memory[4] = value;
        }

        // TODO: LYC (0xFF44-45) v-line interrupt
        // TODO: DMA (0xFF46) transfers

        public byte BGP
        {
            // 0xFF47
            get => memory[7];
            set => memory[7] = value;
        }

        public byte OBP0
        {
            // 0xFF48
            get => memory[8];
            set => memory[8] = value;
        }

        public byte OBP1
        {
            // 0xFF49
            get => memory[9];
            set => memory[9] = value;
        }
        
        public byte WY
        {
            // 0xFF4A
            get => memory[10];
            set => memory[10] = value;
        }

        public byte WX
        {
            // 0xFF4B
            get => memory[11];
            set => memory[11] = value;
        }

        public GraphicRegisters(IGraphics graphics)
        {
            this.graphics = graphics;
            this.memory = new byte[16];
        }

        public byte Read(int address)
        {
            ThrowIfOutOfBounds(address);

            return memory[address];
        }

        public void Write(int address, byte value)
        {
            ThrowIfOutOfBounds(address);

            if (address == 4) // LY register, read-only // TODO: check what's the hardware behavior on writing to this register
                return;

            GraphicsDataRegister register;
            switch (address)
            {
                case 0: register = GraphicsDataRegister.LCDC; break;
                
                case 2: register = GraphicsDataRegister.SCY; break;
                case 3: register = GraphicsDataRegister.SCX; break;

                case 7: register = GraphicsDataRegister.BGP; break;
                case 8: register = GraphicsDataRegister.OBP0; break;
                case 9: register = GraphicsDataRegister.OBP1; break;

                case 10: register = GraphicsDataRegister.WY; break;
                case 11: register = GraphicsDataRegister.WX; break;

                default: throw new NotImplementedException();
            }

            graphics.NotifyGraphicsDataChanged(register, value);

            memory[address] = value;
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
