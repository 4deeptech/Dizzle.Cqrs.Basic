using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dizzle.Cqrs.Portable
{
    /// <summary>
    /// Equivalent to System.Void which is not allowed to be used in the code for some reason.
    /// </summary>
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct unit
    {
        public static readonly unit it = default(unit);
    }

}
