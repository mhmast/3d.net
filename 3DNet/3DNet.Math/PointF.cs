namespace _3DNet.Math
{
    public class PointF : MatrixBase<PointF, PointF>
    {
        public PointF(float value) : base(value)
        {
        }

        public PointF(float[] data) : base(data)
        {
        }

        public static PointF Up => new PointF(new[] { 0f, 1f });
        public float X => this[1, 1];
        public float Y => this[2, 1];

        public override int Rows => 2;

        public override int Cols => 1;
        public static implicit operator Point(PointF p) => new Point((int)p.X, (int)p.Y);
    }
}
