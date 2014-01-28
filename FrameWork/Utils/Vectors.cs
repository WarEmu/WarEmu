
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameWork
{
    [Serializable]
    public class Vector2
    {
        public float X;
        public float Y;

        public Vector2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    [Serializable]
    public class Vector3 : Vector2
    {
        public float Z;

        public Vector3(float X, float Y, float Z)
            : base(X, Y)
        {
            this.Z = Z;
        }
    }

    [Serializable]
    public class Quaternion : Vector3
    {
        public float W;

        public Quaternion(float X, float Y, float Z, float W)
            : base(X, Y, Z)
        {
            this.W = W;
        }

    }
}
