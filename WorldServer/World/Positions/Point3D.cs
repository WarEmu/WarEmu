using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldServer
{
    public class Point3D : Point2D, IPoint3D
    {
        /// <summary>
        /// The Z coord of this point
        /// </summary>
        protected int m_z;

        /// <summary>
        /// Constructs a new 3D point object
        /// </summary>
        public Point3D()
            : base(0, 0)
        {
        }

        /// <summary>
        /// Constructs a new 3D point object
        /// </summary>
        /// <param name="x">The X coord</param>
        /// <param name="y">The Y coord</param>
        /// <param name="z">The Z coord</param>
        public Point3D(int x, int y, int z)
            : base(x, y)
        {
            m_z = z;
        }

        /// <summary>
        /// Constructs a new 3D point object
        /// </summary>
        /// <param name="point">2D point</param>
        /// <param name="z">Z coord</param>
        public Point3D(IPoint2D point, int z)
            : this(point.X, point.Y, z)
        {
        }

        /// <summary>
        /// Constructs a new 3D point object
        /// </summary>
        /// <param name="point">3D point</param>
        public Point3D(IPoint3D point)
            : this(point.X, point.Y, point.Z)
        {
        }

        #region IPoint3D Members

        /// <summary>
        /// Z coord of this point
        /// </summary>
        public virtual int Z
        {
            get { return m_z; }
            set { m_z = value; }
        }

        public override void Clear()
        {
            base.Clear();
            Z = 0;
        }

        #endregion

        /// <summary>
        /// Creates the string representation of this point
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", m_x.ToString(), m_y.ToString(), m_z.ToString());
        }

        /// <summary>
        /// Get the distance to a point
        /// </summary>
        /// <remarks>
        /// If you don't actually need the distance value, it is faster
        /// to use IsWithinRadius (since it avoids the square root calculation)
        /// </remarks>
        /// <param name="point">Target point</param>
        /// <returns>Distance to point</returns>
        public virtual int GetDistanceTo(IPoint3D point)
        {
            double dx = (double)(X - point.X);
            double dy = (double)(Y - point.Y);
            double Range = Math.Sqrt(dx * dx + dy * dy);
            Range = Range / Lerp(36.0, 13.50, Clamp(Range, 900));
            return (int)(Range);
        }

        /// <summary>
        /// Get the distance to a point
        /// </summary>
        /// <remarks>
        /// If you don't actually need the distance value, it is faster
        /// to use IsWithinRadius (since it avoids the square root calculation)
        /// </remarks>
        /// <param name="point">Target point</param>
        /// <returns>Distance to point</returns>
        public virtual int GetDistanceTo(float x, float y, float z)
        {
            double dx = (double)(X - x);
            double dy = (double)(Y - y);
            double Range = Math.Sqrt(dx * dx + dy * dy);
            Range = Range / Lerp(36.0, 13.50, Clamp(Range, 900));
            return (int)(Range);
        }

        /// <summary>
        /// Get the distance to a point (with z-axis adjustment)
        /// </summary>
        /// <param name="point">Target point</param>
        /// <param name="zfactor">Z-axis factor - use values between 0 and 1 to decrease influence of Z-axis</param>
        /// <returns>Adjusted distance to point</returns>
        public virtual int GetDistanceTo(IPoint3D point, double zfactor)
        {
            double dx = (double)X - point.X;
            double dy = (double)Y - point.Y;
            
            double dz = (double)((Z - point.Z) * zfactor);

            return (int)(Math.Sqrt(dx * dx + dy * dy + dz * dz) / 13.2f);
        }

        /// <summary>
        /// Determine if another point is within a given radius
        /// </summary>
        /// <param name="point">Target point</param>
        /// <param name="radius">Radius</param>
        /// <returns>True if the point is within the radius, otherwise false</returns>
        public bool IsWithinRadius(IPoint3D point, int radius)
        {
            return IsWithinRadius(point, radius, false);
        }


        /// <summary>
        /// Determine if another point is within a given radius, optionally ignoring Z values
        /// </summary>
        /// <param name="point">Target point</param>
        /// <param name="radius">Radius</param>
        /// <param name="ignoreZ">ignore Z</param>
        /// <returns>True if the point is within the radius, otherwise false</returns>
        public bool IsWithinRadius(IPoint3D point, int radius, bool ignoreZ)
        {
            int Dist = 0;
            
            if(!ignoreZ)
                Dist = GetDistanceTo(point);
            else
                Dist = GetDistance(point);

            return Dist < radius;
        }
    }
}
