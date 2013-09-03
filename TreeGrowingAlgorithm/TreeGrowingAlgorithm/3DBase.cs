using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TreeGrowingAlgorithm
{
    /// <summary>
    /// Represents a particle in 3D
    /// </summary>
    public class Particle3D
    {
        /// <summary>
        /// The current location of the particle
        /// </summary>
        public Point3D Location { get; set; }

        /// <summary>
        /// The previous location of the particle
        /// </summary>
        public Point3D OldLocation { get; set; }

        /// <summary>
        /// The direction the particle is going
        /// </summary>
        public Vector3D Direction { get; set; }

        /// <summary>
        /// The maximum life the particle has
        /// </summary>
        public int MaxLife { get; set; }

        /// <summary>
        /// The current life it has left
        /// </summary>
        public int Life { get; set; }

        /// <summary>
        /// The color of the particle
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// The behaviour of the particle, this will be evaluated on each tick
        /// </summary>
        public NextIterationHandler3D Behaviour { get; set; }

        /// <summary>
        /// Go into the next state of the particle
        /// </summary>
        public virtual void Next()
        {
            if (Life > 0)
            {
                // move the particle and reduce its life
                OldLocation = Location;
                Location = Location + Direction;
                Life--;

                // evaluate its behaviour
                if (Behaviour != null)
                    Behaviour(this);
            }
        }
    }

    public delegate void NextIterationHandler3D(Particle3D p);

    /// <summary>
    /// A point in 3D
    /// </summary>
    public struct Point3D
    {
        public Point3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double X;
        public double Y;
        public double Z;

    }

    /// <summary>
    /// A 3D size
    /// </summary>
    public struct Size3D
    {
        public Size3D(int width, int height, int depth)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
        }

        public int Width;
        public int Height;
        public int Depth;

    }

    /// <summary>
    /// A vector in 3D
    /// </summary>
    public struct Vector3D
    {
        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Rotate over Z axis
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector3D RotateZ(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Vector3D(cos * X - sin * Y, sin * X + cos * Y, Z);
        }

        /// <summary>
        /// Rotate over Y axis
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector3D RotateY(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Vector3D(cos * X + sin * Z, Y, -sin * X + cos * Z);
        }

        /// <summary>
        /// Adds a point and a vector
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point3D operator +(Point3D p, Vector3D v)
        {
            return new Point3D(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
        }

        public double X;
        public double Y;
        public double Z;
    }
}
