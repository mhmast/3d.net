using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace _3DNet.Math.Tests
{
    [TestFixture]
    public class MathTests
    {

        [Test]
        public void Mat_ShouldInitCorrectly()
        {
            var mat = new Matrix2x2();
            mat.Rows.Should().Be(2);
            mat.Cols.Should().Be(2);
            mat.Data.All(a => a == 0.0).Should().BeTrue();
            mat.Data.Length.Should().Be(4);
        }

        class BSVector : MatrixBase<BSVector, Scalar, Scalar>
        {
            public BSVector() : base(new Scalar[0])
            {

            }
            public override int Rows => 0;
            public override int Cols => 0;
            public override BSVector Instance => this;

            public override Scalar CreateColumn(params Scalar[] values)
            => values[0];

            public override Scalar CreateColumnZeros()
            => 0f;


            public override BSVector CreateMatrix(params Scalar[] values)
            => new BSVector();


            public override BSVector CreateMatrixFromColumns(params IMatrix[] cols)
            => new BSVector();


            public override BSVector CreateMatrixFromRows(params IMatrix[] rows)
            => new BSVector();


            public override Scalar CreateRow(params Scalar[] values)
            => 0f;


            public override Scalar CreateRowZeros()
            => 0f;

        }
        class BSMatrix : MatrixBase<BSMatrix, BSVector, BSVector>
        {
            public BSMatrix() : base(new Scalar[0])
            {

            }
            public override int Rows => 0;

            public override int Cols => 0;

            public override BSMatrix Instance => this;

            public override BSVector CreateColumn(params Scalar[] values)
            => new BSVector();


            public override BSVector CreateColumnZeros()
            => new BSVector();


            public override BSMatrix CreateMatrix(params Scalar[] values)
            => new BSMatrix();


            public override BSMatrix CreateMatrixFromColumns(params IMatrix[] cols)
            => new BSMatrix();

            public override BSMatrix CreateMatrixFromRows(params IMatrix[] rows)
            => new BSMatrix();

            public override BSVector CreateRow(params Scalar[] values)
            => new BSVector();

            public override BSVector CreateRowZeros()
            => new BSVector();
        }
        [Test]
        public void Mat_Empty_Should_Be_True_On_No_Data()
        {
            var mat = new BSMatrix();
            mat.Empty().Should().BeTrue();
        }

        [Test]
        public void Mat_Empty_Should_Be_False_On_Data()
        {
            var mat = new Matrix2x2((10, 10), (1, 10));
            mat.Empty().Should().BeFalse();
        }

        [Test]
        public void Mat_SumAll_Should_Sum()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            mat.SumAll().Should().Be(10.0f);
        }

        [Test]
        public void Mat_Min_Should_Return_Smallest()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            mat.Min().Should().Be(1.0f);
        }

        [Test]
        public void Mat_Max_Should_Return_Largest()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            mat.Max().Should().Be(4.0f);
        }

        [Test]
        public void Mat_Index_Should_Get()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            mat[1][1].Should().Be(1.0f);
            mat[1][2].Should().Be(2.0f);
            mat[2][1].Should().Be(3.0f);
            mat[2][2].Should().Be(4.0f);
        }

        [Test]
        public void Mat_Index_Should_Set()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            mat[1][1] = 7.0f;
            mat[1][1].Should().Be(7.0f);
        }


        [Test]
        public void Mat_SetAll_Should_Set_All()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            mat.SetAll(7.0f);
            mat.Data.All(a => a == 7.0f).Should().BeTrue();
        }
        [Test]
        public void Mat_Copy_Should_Copy()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var copy = mat.Copy();
            copy.Data.SequenceEqual(mat.Data).Should().BeTrue();
            copy.Rows.Should().Be(mat.Rows);
            copy.Cols.Should().Be(mat.Cols);
        }

        [Test]
        public void Mat_Plus_Scalar_Should_Plus()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var plus = mat + (Scalar)2;
            plus.Data.SequenceEqual(new Scalar[] { 3f, 4, 5, 6 }).Should().BeTrue();
        }
        [Test]
        public void Mat_Plus_Should_Plus_Vector()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var plus = mat + new Vector2F(1f, 2f);
            plus.Data.SequenceEqual(new Scalar[] { 2f, 4, 4, 6 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Plus_Should_Plus_Mat()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var min = mat + new Matrix2x2((1, 2f), (3, 4f));
            min.Data.SequenceEqual(new Scalar[] { 2f, 4, 6, 8 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Minus_Should_Min()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var min = mat - (Scalar)2f;
            min.Data.SequenceEqual(new Scalar[] { -1f, 0, 1, 2 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Minus_Should_Min_Vector()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var min = mat - new Vector2F(1, 2);
            min.Data.SequenceEqual(new Scalar[] { 0f, 1, 1, 2 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Minus_Should_Min_Mat()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var min = mat - new Matrix2x2((1, 2f), (3, 4f));
            min.Data.SequenceEqual(new Scalar[] { 0f, 0, 0, 0 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Mul_Should_Mul()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var mul = mat * (Scalar)2;
            mul.Data.SequenceEqual(new Scalar[] { 2f, 4, 6, 8 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Mul_Should_Mul_Vector()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var mul = mat * new Vector2F(1, 2);
            mul.Data.SequenceEqual(new Scalar[] { 1, 2, 6f, 8 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Mul_Should_Mul_Mat()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var mul = mat * new Matrix2x2((1, 2f), (3, 4f));
            mul.Data.SequenceEqual(new Scalar[] { 1f, 4, 9, 16 }).Should().BeTrue();
        }

        [Test]
        public void Vector_Div_Should_Div()
        {
            var mat = new Vector2F(1, 2f);
            var div = mat / (Scalar)2f;
            div.Data.SequenceEqual(new Scalar[] { 0.5f, 1}).Should().BeTrue();
        }

        [Test]
        public void Mat_Div_Should_Div()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var div = mat / (Scalar)2f;
            div.Data.SequenceEqual(new Scalar[] { 0.5f, 1, 1.5f, 2 }).Should().BeTrue();
        }
        [Test]
        public void Mat_Div_Should_Div_Vector()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var div = mat / new Vector2F(1f, 2f);
            div.Data.SequenceEqual(new Scalar[] { 1, 1f, 3, 2 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Div_Should_Div_Mat()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var div = mat / new Matrix2x2((1, 2f), (3, 4f));
            div.Data.SequenceEqual(new Scalar[] { 1f, 1, 1, 1 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Pow_Should_Pow_Mat()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var pow = mat.Pow(2);
            pow.Data.SequenceEqual(new Scalar[] { 1f, 4, 9, 16 }).Should().BeTrue();
        }

        [Test]
        public void Mat_T_Should_Transpose()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            mat.Rows.Should().Be(2);
            mat.Cols.Should().Be(2);
            mat[1][1].Should().Be(1);
            mat[1][2].Should().Be(3);
            mat[2][1].Should().Be(2);
            mat[2][2].Should().Be(4);
        }


        [Test]
        public void Mat_Size_Should_Return_Size()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var size = mat.Size;
            size.Y.Should().Be(2);
            size.X.Should().Be(2);
        }

        [Test]
        public void Mat_Columns_Should_Return_Columns()
        {
            var columns = new Matrix2x2((1, 2f), (3, 4f)).Columns().ToList();
            columns.Count.Should().Be(2);
            columns[0].Equals(new Vector2F(1f,3f)).Should().BeTrue();
            columns[1].Equals(new Vector2F(2,4f)).Should().BeTrue();
        }
    }
}