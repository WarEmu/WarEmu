
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    static public class RandomMgr
    {
        static public Random _Random = new Random();

        static public int Next()
        {
            lock (_Random)
                return _Random.Next();
        }

        static public int Next(int MaxValue)
        {
            lock (_Random)
                return _Random.Next(MaxValue);
        }

        static public int Next(int MinValue, int MaxValue)
        {
            lock (_Random)
                return _Random.Next(MinValue, MaxValue);
        }
    }
}
