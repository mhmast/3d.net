using System.Linq;

namespace _3DNet.Math
{
    public class Scalar : MatrixBase<Scalar, Scalar, Scalar>
    {

        private float _value;
        private int _interactWith = 0;

        public override int Rows => 1;

        public override int Cols => 1;

        public override Scalar Instance => this;

        public override Scalar[] Data => new[] { this };

        public override Scalar this[int row] { get => this; set => _value = value._value; }

        public override IMatrix Col(int col)
        => this;

        private Scalar(float value, int interactWith = 0) : base(new Scalar[0])
        {
            _value = value;
            _interactWith = interactWith;
        }

        public Scalar(float value) : this(value, 0)
        { }

        public static implicit operator float(Scalar s) => s._value;
        public static implicit operator Scalar(float s) => new Scalar(s);
        public static Scalar operator *(Scalar left, Scalar right) => left * right;

        public override bool Equals(object obj)
        {
            return obj is Scalar s && s._value == _value;
        }

        public override IMatrix GetColForProductWith(IMatrix left, int col)
        {
            var data = Enumerable.Repeat((Scalar)0, col * left.Cols)
                .Concat(Enumerable.Repeat(this, left.Cols))
                .Concat(Enumerable.Repeat((Scalar)0, (left.Rows - col - 1) * left.Cols)).ToArray();
            return left.CreateMatrix(data);
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

        public override Scalar CreateMatrixFromColumns(params IMatrix[] cols)
        => this;

        public override Scalar CreateMatrixFromRows(params IMatrix[] rows)
        => this;
        protected internal override Scalar Div(IMatrix value)
        => value.Data.Select(x => _value == 0 || x._value == 0 ? 0 : _value / x._value).Sum();

        public override string ToString()
        => $"{_value}";
    }
}
