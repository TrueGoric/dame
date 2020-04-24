using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dame.Emulator.Accessors;
using Dame.Emulator.Memory.Blocks;

namespace Dame.Emulator.Memory
{
    public sealed class MemoryController
    {
        private readonly RangeStartComparer rangeComparer = new RangeStartComparer();

        private readonly SortedList<Range, IModifyBlock> modifyBlocks;
        private readonly SortedList<Range, IReadBlock> readBlocks;
        private readonly SortedList<Range, IWriteBlock> writeBlocks;

        public int AddressSpace { get; }

        public MemoryController(int addressSpace)
        {
            AddressSpace = addressSpace;

            modifyBlocks = new SortedList<Range, IModifyBlock>(rangeComparer);
            readBlocks = new SortedList<Range, IReadBlock>(rangeComparer);
            writeBlocks = new SortedList<Range, IWriteBlock>(rangeComparer);
        }

        public void AddBlock<TBlock>(Range range, TBlock block)
            where TBlock : class
        {
            if (block is IReadBlock readBlock)
                readBlocks.Add(range, readBlock);
            if (block is IWriteBlock writeBlock)
                writeBlocks.Add(range, writeBlock);
            if (block is IModifyBlock modifyBlock)
                modifyBlocks.Add(range, modifyBlock);
        }

        #region Memory Access

        public ref byte Get(int address)
        {
            IModifyBlock block;
            int offset;

            if (!GetBlock(modifyBlocks, address, out block, out offset, out _))
                throw new AccessViolationException($"Address {address} cannot be read from!");

            return ref block.Get(address - offset);
        }

        public byte Read(int address)
        {
            IReadBlock block;
            int offset;

            if (!GetBlock(readBlocks, address, out block, out offset, out _))
                throw new AccessViolationException($"Address {address} cannot be read from!");

            return block.Read(address - offset);
        }

        public ushort ReadDouble(int address)
        {
            IReadBlock block1, block2;
            int offset;

            if (!GetBlock(readBlocks, address, out block1, out offset, out _)
                || !GetBlock(readBlocks, address + 1, out block2, out offset, out _))
                throw new AccessViolationException($"Address (16-bit) {address} cannot be read from!");

            Span<byte> toBeCast = stackalloc byte[2];

            toBeCast[0] = block1.Read(address - offset);
            toBeCast[1] = block2.Read(address + 1 - offset);
            
            // GB stores 16-bit values as little-endian, so we need to 
            // accomodate if the host prefers it the other way
            if (!BitConverter.IsLittleEndian)
                toBeCast.Reverse();
            
            return MemoryMarshal.Cast<byte, ushort>(toBeCast)[0];
        }

        public void Write(int address, byte value)
        {
            IWriteBlock block;
            int offset;

            if (!GetBlock(writeBlocks, address, out block, out offset, out _))
                throw new AccessViolationException($"Address {address} cannot be written to!");

            block.Write(address - offset, value);
        }

        public void WriteDouble(int address, ushort value)
        {
            IWriteBlock block1, block2;
            int offset;

            if (!GetBlock(writeBlocks, address, out block1, out offset, out _)
                || !GetBlock(writeBlocks, address + 1, out block2, out offset, out _))
                throw new AccessViolationException($"Address (16-bit) {address} cannot be written to!");

            block1.Write(address - offset, (byte)(value & 0x00FF));
            block2.Write(address + 1 - offset, (byte)(value >> 8));
        }

        #endregion

        public MemoryAccessor CreateAccessor() => new MemoryAccessor(this);

        private bool GetBlock<TBlock>(SortedList<Range, TBlock> blocks, int address, out TBlock block, out int offset, out int length)
            where TBlock : class
        {
            // TODO: optimize using binary search trees and custom range lookup classes (OR, even better, compiled jumptables!)
            foreach (var currentBlock in blocks)
            {
                (offset, length) = currentBlock.Key.GetOffsetAndLength(AddressSpace);

                if (RangeContains(offset, length, address))
                {
                    block = currentBlock.Value;

                    return true;
                }
            }

            block = null;
            offset = 0;
            length = 0;

            return false;
        }

        private bool RangeContains(int offset, int length, int address)
            => address >= offset && address < offset + length;
    }
}