using System;
using System.Collections.Generic;
using Dame.Accessors;
using Dame.Memory.Blocks;

namespace Dame.Memory
{
    sealed class MemoryController<T>
        where T : unmanaged
    {
        private SortedList<Range, IModifyBlock<T>> modifyBlocks;
        private SortedList<Range, IReadBlock<T>> readBlocks;
        private SortedList<Range, IWriteBlock<T>> writeBlocks;

        public int AddressSpace { get; }

        public MemoryController(int addressSpace)
        {
            AddressSpace = addressSpace;
        }

        public void AddBlock<TBlock>(Range range, TBlock block)
            where TBlock : IReadBlock<T>, IModifyBlock<T>, IWriteBlock<T>
        {
            if (block is IReadBlock<T> readBlock)
                readBlocks.Add(range, readBlock);
            if (block is IWriteBlock<T> writeBlock)
                writeBlocks.Add(range, writeBlock);
            if (block is IModifyBlock<T> modifyBlock)
                modifyBlocks.Add(range, modifyBlock);
        }

        #region Memory Access

        public ref T Get(int address)
        {
            foreach (var block in modifyBlocks)
            {
                var (offset, length) = block.Key.GetOffsetAndLength(AddressSpace);

                if (RangeContains(offset, length, address))
                    return ref block.Value.Get(address - offset);
            }

            throw new AccessViolationException($"Address {address} cannot be read from!");
        }

        public T Read(int address)
        {
            foreach (var block in readBlocks)
            {
                var (offset, length) = block.Key.GetOffsetAndLength(AddressSpace);

                if (RangeContains(offset, length, address))
                    return block.Value.Read(address - offset);
            }

            throw new AccessViolationException($"Address {address} cannot be read from!");
        }

        public void Write(int address, T value)
        {
            foreach (var block in writeBlocks)
            {
                var (offset, length) = block.Key.GetOffsetAndLength(AddressSpace);

                if (RangeContains(offset, length, address))
                    block.Value.Write(address - offset, value);
            }

            throw new AccessViolationException($"Address {address} cannot be written to!");
        }

        #endregion

        public MemoryAccessor<T> CreateAccessor() => new MemoryAccessor<T>(this);

        private bool RangeContains(int offset, int length, int address)
            => address >= offset && address < offset + length;
    }
}