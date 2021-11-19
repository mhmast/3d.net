using System.Linq;

namespace _3DNet.Math
{
    public class Vector2F : MatrixBase<Vector2F, Scalar, Scalar>, IVector
    {
        public Vector2F(params Scalar[] rows) : base(rows)
        {

        }

        public static Vector2F Up => new Vector2F(0f, 1f );
        public float X => this[1][1];
        public float Y => this[2][1];

        public override int Rows => 2;

        public override int Cols => 1;

        public override Vector2F Instance => this;

        public override Scalar CreateColumn(params Scalar[] values)
        => values[0];

        public override Scalar CreateColumnZeros()
        =>  0f;

        public override Vector2F CreateMatrix(params Scalar[] values)
        => new Vector2F(values);

        public override Vector2F CreateMatrixFromColumns(params IVector[] cols)
        => new Vector2F(cols[0].Data);

        public override Vector2F CreateMatrixFromRows(params IVector[] rows)
        => new Vector2F(rows.SelectMany(r=>r.Data).ToArray());

        public override Scalar CreateRow(params Scalar[] values)
        => values[0];

        public override Scalar CreateRowZeros()
        => 0f;

        public static implicit operator Point(Vector2F p) => new Point((int)p.X, (int)p.Y);
        public static implicit operator Vector2F((float,float) p) => new Vector2F(p.Item1,p.Item2);
    }
}
