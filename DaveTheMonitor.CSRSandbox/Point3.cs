using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaveTheMonitor.CSRSandbox
{
    internal struct Point3 : IEquatable<Point3>
    {
        public int X;
        public int Y;
        public int Z;

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else if (obj is Point3 p)
            {
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Point3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public Point3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
