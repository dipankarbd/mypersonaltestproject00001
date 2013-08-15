﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nPBRT.Core.Geometry
{
    public class Point
    {
        public float x, y, z;
        public Point()
        {
            this.x = this.y = this.z = 0.0f;
        }
        public Point(float x, float y, float z)
        {
            if (HasNaNs()) throw new NotFiniteNumberException();

            this.x = x;
            this.y = y;
            this.z = z;
        }
        public bool HasNaNs()
        {
            return float.IsNaN(this.x) || float.IsNaN(this.x) || float.IsNaN(this.x);
        }
        public static Point operator +(Point p, Vector v)
        {
            return new Point(p.x + v.x, p.y + v.y, p.z + v.z);
        }
        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }
        public static Vector operator -(Point p1, Point p2)
        {
            return new Vector(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        }
        public static Point operator -(Point p, Vector v)
        {
            return new Point(p.x - v.x, p.y - v.y, p.z - v.z);
        }

        public static Point operator *(Point v, float f)
        {
            return new Point(f * v.x, f * v.y, f * v.z);
        }

        public static Point operator *(float f, Point v)
        {
            return new Point(f * v.x, f * v.y, f * v.z);
        }
        public static Point operator /(Point v, float f)
        {
            if (f == 0.0f) throw new InvalidOperationException();
            float inv = 1.0f / f;
            return new Point(v.x * inv, v.y * inv, v.z * inv);
        }
        public float this[int index]
        {
            get
            {
                if (index < 0 || index > 2) throw new ArgumentException();

                if (index == 0) return x;
                else if (index == 1) return y;
                else if (index == 2) return z;
                else return 0;
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Point p = obj as Point;
            if ((System.Object)p == null)
            {
                return false;
            }

            return (this.x == p.x) && (this.y == p.y) && (this.z == p.z);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
