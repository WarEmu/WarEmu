using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace FrameWork
{
    // Invoke through FrameWork.StaticRandom.Instance.next(...)
    public static class StaticRandom
    {
        private static int seed;

        private static ThreadLocal<Random> threadLocal = new ThreadLocal<Random>
            (() => new Random(Interlocked.Increment(ref seed)));

        static StaticRandom()
        {
            seed = Environment.TickCount;
        }

        public static Random Instance { get { return threadLocal.Value; } }
    }
}
