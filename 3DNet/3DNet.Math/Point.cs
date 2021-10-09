using System;
using System.Drawing;

namespace _3DNet.Math
{
    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
            Length = X + Y;
            Key = ((long)X << 32) + y;
        }

        public int Min() => X < Y ? X : Y;
        public int Max() => X > Y ? X : Y;
        public int X { get; }
        public int Y { get; }
        public int Length { get; }

        private long Key { get; }

        public static bool operator ==(Point left, Point right) => left.Key == right.Key;
        public static bool operator !=(Point left, Point right) => !(left == right);
        public static Point operator +(Point left, Point right) => new Point(left.X + right.X, left.Y + right.Y);
        public static Point operator +(Point left, PointF right) => new Point((int)(left.X + right.X), (int)(left.Y + right.Y));
        public static Point operator +(Point left, int right) => new Point(left.X + right, left.Y + right);
        public static Point operator -(Point left, Point right) => new Point(left.X - right.X, left.Y - right.Y);
        public static Point operator -(Point left, int right) => new Point(left.X - right, left.Y - right);
        public static Point operator /(Point left, Point right) => new Point(left.X / right.X, left.Y / right.Y);
        public static Point operator /(Point left, int right) => new Point(left.X / right, left.Y / right);
        public static Point operator *(Point left, Point right) => new Point(left.X * right.X, left.Y * right.Y);
        public static Point operator *(Point left, int right) => new Point(left.X * right, left.Y * right);

        public Point Abs() => new Point(System.Math.Abs(X), System.Math.Abs(Y));

        
        public long Sum()
        => X + Y;

        public override bool Equals(object obj)
        {
            return obj is Point point &&
                   X == point.X &&
                   Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
