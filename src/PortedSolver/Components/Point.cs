using System;

namespace PortedSolver.Components
{
    public class Point
    {

        public double x, y, z;

        public Point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool equals(Point p)
        {
            return (this.x == p.x && this.y == p.y && this.z == p.z);
        }


        public double getX()
        {
            return x;
        }

        public void setX(double x)
        {
            this.x = x;
        }

        public double getY()
        {
            return y;
        }

        public void setY(double y)
        {
            this.y = y;
        }

        public double getZ()
        {
            return z;
        }

        public void setZ(double z)
        {
            this.z = z;
        }

        public override string ToString()
        {
            return "Point{" +
                   "x=" + x +
                   ", y=" + y +
                   ", z=" + z +
                   '}';
        }

        public double distance(Point position)
        {
            return Math.Sqrt(Math.Pow((position.getX() - x), 2)
                             + Math.Pow((position.getY() - y), 2)
                             + Math.Pow((position.getZ() - z), 2));
        }


    }
}
