using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dame
{
    class RangeStartComparer : Comparer<Range>
    {
        public override int Compare([AllowNull] Range x, [AllowNull] Range y)
        {
            if (x.Start.IsFromEnd || y.Start.IsFromEnd)
                throw new NotSupportedException("This comparer doesn't support from-end indices!");
                
            return x.Start.Value - y.Start.Value;
        }
    }
}