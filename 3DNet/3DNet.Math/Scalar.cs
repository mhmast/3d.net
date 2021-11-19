using System.Linq;

namespace _3DNet.Math
{
    public class Scalar : MatrixBase<Scalar, Scalar, Scalar>, IVector
    {
        private float _value;

        public override int Rows => 1;

        public override int Cols => 1;

        public override Scalar Instance => this;

        public override Scalar[] Data => new[] {this};

        public override Scalar this[int row] { get => this; set => _value = value._value; }

        public Scalar(float value) : base(new Scalar[0])
        {
            _value = value;
        }

        public static implicit operator float(Scalar s) => s._value;
        public static implicit operator Scalar(float s) => new Scalar(s);
        public static Scalar operator *(Scalar left, Scalar right) => left * right;

        public override bool Equals(object obj)
        {
            return obj is Scalar s && s._value == _value;
        }

        public override int GetHashCode() => _value.GetHashCode();

        public override Scalar CreateMatrix(params Scalar[] values)
        => this;

        public override Scalar CreateColumn(params Scalar[] values)
        => this;

        public override Scalar CreateColumnZeros()
        => this;

        public override Scalar CreateRow(params Scalar[] values)
        => this;

        public override Scalar CreateRowZeros()
        => this;

        public override Scalar CreateMatrixFromColumns(params IVector[] cols)
        => this;

        public override Scalar CreateMatrixFromRows(params IVector[] rows)
        => this;
        protected override Scalar Div(IMatrix value)
        => value.Data.Select(d=>_value/d._value).Sum();

        public override string ToString()
        =>$"{_value}"        ;
    }
}
