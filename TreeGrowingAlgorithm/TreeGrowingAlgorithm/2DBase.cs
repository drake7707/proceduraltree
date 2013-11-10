using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TreeGrowingAlgorithm
{
    /// <summary>
    /// Represents a particle in 2D space
    /// </summary>
    public class Particle2D
    {
        /// <summary>
        /// The current location of the particle
        /// </summary>
        public Point2D Location { get; set; }

        /// <summary>
        /// The previous location of the particle
        /// </summary>
        public Point2D OldLocation { get; set; }

        /// <summary>
        /// The direction the particle is going
        /// </summary>
        public Vector2D Direction;

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
        public NextIterationHandler2D Behaviour { get; set; }

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

    public delegate void NextIterationHandler2D(Particle2D p);


    public struct Point2D
    {
        public Point2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X;
        public double Y;

    }


    public struct Size2D
    {
        public Size2D(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        public double Width;
        public double Height;

    }

    /// <summary>
    /// Represents a vector in 2D
    /// </summary>
    public struct Vector2D
    {
        public Vector2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Rotation of the vector by the specified angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector2D Rotate(double angle)
        {
            return new Vector2D(Math.Cos(angle) * X - Math.Sin(angle) * Y, Math.Sin(angle) * X + Math.Cos(angle) * Y);
        }

        /// <summary>
        /// Addition of 2 vectors
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point2D operator +(Point2D p, Vector2D v)
        {
            return new Point2D(p.X + v.X, p.Y + v.Y);
        }

        public double X;
        public double Y;
    }
}
