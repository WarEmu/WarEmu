using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldServer
{
    public interface IPoint3D : IPoint2D
    {
        int Z { get; set; }
    }
}
