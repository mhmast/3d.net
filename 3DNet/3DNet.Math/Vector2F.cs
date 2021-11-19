namespace _3DNet.Math
{
    public class Vector2F : MatrixBase<Vector2F, Scalar, Vector2F>, IVector
    {
        public Vector2F(params Scalar[] rows) : base(rows)
        {

        }

        public static Vector2F Up => new Vector2F(0f, 1f );
        public float X => this[1][1];
        public float Y => this[2][1];

        public override int Rows => throw new System.NotImplementedException();

        public override int Cols => throw new System.NotImplementedException();

        public override Vector2F Instance => throw new System.NotImplementedException();

        public override Vector2F CreateColumn(params Scalar[] values)
        {
            throw new System.NotImplementedException();
        }

        public override Vector2F CreateColumnZeros()
        {
            throw new System.NotImplementedException();
        }

        public override Vector2F CreateMatrix(params Scalar[] values)
        {
            throw new System.NotImplementedException();
        }

        public override Vector2F CreateMatrixFromColumns(params IVector[] cols)
        {
            throw new System.NotImplementedException();
        }

        public override Vector2F CreateMatrixFromRows(params IVector[] rows)
        {
            throw new System.NotImplementedException();
        }

        public override Scalar CreateRow(params Scalar[] values)
        {
            throw new System.NotImplementedException();
        }

        public override Scalar CreateRowZeros()
        {
            throw new System.NotImplementedException();
        }

        public static implicit operator Point(Vector2F p) => new Point((int)p.X, (int)p.Y);
        public static implicit operator Vector2F((float,float) p) => new Vector2F(p.Item1,p.Item2);
    }
}
