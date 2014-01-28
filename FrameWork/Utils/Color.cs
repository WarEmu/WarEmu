
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    [Serializable]
    public class Color
    {
        public float R = 0;
        public float G = 0;
        public float B = 0;
        public float A = 0;

        public override string ToString()
        {
            return R + ":" + G + ":" + B + ":" + A;
        }
    }
}
