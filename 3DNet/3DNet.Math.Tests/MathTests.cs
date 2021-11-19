using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace _3DNet.Math.Tests
{
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

        class BSVector : MatrixBase<BSVector, Scalar, BSVector>, IVector
        {
            public BSVector(): base(new Scalar[0])
            {

            }
            public override int Rows => 0;
            public override int Cols => 0;
            public override BSVector Instance => this;

            public override BSVector CreateColumn(params Scalar[] values)
            => new BSVector();

            public override BSVector CreateColumnZeros()
            => new BSVector();


            public override BSVector CreateMatrix(params Scalar[] values)
            => new BSVector();


            public override BSVector CreateMatrixFromColumns(params IVector[] cols)
            => new BSVector();


            public override BSVector CreateMatrixFromRows(params IVector[] rows)
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


            public override BSMatrix CreateMatrixFromColumns(params IVector[] cols)
            => new BSMatrix();

            public override BSMatrix CreateMatrixFromRows(params IVector[] rows)
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
            var mat = new Matrix2x2((10, 10),(1,10));
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
            var plus = mat + 2;
            plus.Data.SequenceEqual(new Scalar[] { 3f, 4, 5, 6 }).Should().BeTrue();
        }
        [Test]
        public void Mat_Plus_Should_Plus_Vector()
        {
            var mat = new Matrix2x2((1, 2f), (3, 4f));
            var plus = mat + (1f, 2f);
            plus.Data.SequenceEqual(new Scalar[] { 2f, 4, 4, 6 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Plus_Should_Plus_VectorV()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var plus = mat.Plus(new Matrix(2, 1, new float[][] { new[] { 1, 2f } }));
            plus.Data.SequenceEqual(new[] { 2f, 3, 5, 6 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Plus_Should_Plus_Mat()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var min = mat.Plus(new Matrix(2, 2, new float[][] { new[] { 1, 2f }, new[] { 3, 4f } }));
            min.Data.SequenceEqual(new[] { 2f, 4, 6, 8 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Minus_Should_Min()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var min = mat - 2;
            min.Data.SequenceEqual(new[] { -1f, 0, 1, 2 }).Should().BeTrue();
        }
        [Test]
        public void Mat_Minus_Should_Min_VectorH()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var min = mat - new Matrix(1, 2, new float[][] { new[] { 1, 2f } });
            min.Data.SequenceEqual(new[] { 0, 0, 2f, 2 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Minus_Should_Min_VectorV()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var min = mat - new Matrix(2, 1, new float[][] { new[] { 1f }, new[] { 2f } });
            min.Data.SequenceEqual(new[] { 0f, 1, 1, 2 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Minus_Should_Min_Mat()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var min = mat - new Matrix(2, 2, new float[][] { new[] { 1, 2f }, new[] { 3, 4f } });
            min.Data.SequenceEqual(new[] { 0f, 0, 0, 0 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Mul_Should_Mul()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var mul = mat * 2;
            mul.Data.SequenceEqual(new[] { 2f, 4, 6, 8 }).Should().BeTrue();
        }
        [Test]
        public void Mat_Mul_Should_Mul_VectorH()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var mul = mat * new Matrix(1, 2, new float[][] { new[] { 1, 2f } });
            mul.Data.SequenceEqual(new[] { 1f, 4, 3, 8 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Mul_Should_Mul_VectorV()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var mul = mat * new Vector(2, 1, new float[][] { new[] { 1f }, new[] { 2f } });
            mul.Data.SequenceEqual(new[] { 1, 2, 6f, 8 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Mul_Should_Mul_Mat()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var mul = mat * new Matrix(2, 2, new float[][] { new[] { 1, 2f }, new[] { 3, 4f } });
            mul.Data.SequenceEqual(new[] { 1f, 4, 9, 16 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Div_Should_Div()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var div = mat / 2;
            div.Data.SequenceEqual(new[] { 0.5f, 1, 1.5f, 2 }).Should().BeTrue();
        }
        [Test]
        public void Mat_Div_Should_Div_VectorH()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var div = mat / new Vector(1, 2, new float[][] { new[] { 1f, 2f } });
            div.Data.SequenceEqual(new[] { 1, 1f, 3, 2 }).Should().BeTrue();
        }

        [Test]
        public void MatDiv_Should_Div_VectorV()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var div = mat / new Matrix(2, 1, new float[][] { new[] { 1, 2f } });
            div.Data.SequenceEqual(new[] { 1, 2, 1.5f, 2 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Div_Should_Div_Mat()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var div = mat / new Matrix(2, 2, new float[][] { new[] { 1, 2f }, new[] { 3, 4f } });
            div.Data.SequenceEqual(new[] { 1f, 1, 1, 1 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Pow_Should_Pow_Mat()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var pow = mat.Pow(2);
            pow.Data.SequenceEqual(new[] { 1f, 4, 9, 16 }).Should().BeTrue();
        }

        [Test]
        public void Mat_Floor_Should_Floor_Mat()
        {
            const int cols = 2, rows = 2;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var floor = mat.Floor();
            floor.Data.SequenceEqual(new[] { 1f, 2, 3, 4 }).Should().BeTrue();
        }

        [Test]
        public void Mat_T_Should_Transpose()
        {
            const int cols = 2, rows = 3;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            mat.Rows.Should().Be(cols);
            mat.Cols.Should().Be(rows);
            mat[1, 1].Should().Be(1);
            mat[1, 2].Should().Be(3);
            mat[1, 3].Should().Be(5);
            mat[2, 1].Should().Be(2);
            mat[2, 2].Should().Be(4);
            mat[2, 3].Should().Be(6);
        }

        [Test]
        public void Mat_Point_Should_Return_Point()
        {
            const int cols = 2, rows = 3;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f } };
            var mat = new Matrix(rows, cols, data);
            var point = mat.Point(2, 2);
            point.X.Should().Be(4);
            point.Y.Should().Be(6);
        }


        [Test]
        public void Mat_Size_Should_Return_Size()
        {
            const int cols = 2, rows = 3;
            var mat = new Matrix(rows, cols);
            var size = mat.Size;
            size.Y.Should().Be(rows);
            size.X.Should().Be(cols);
        }



        [Test]
        public void Mat_Columns_Should_Return_Columns()
        {
            const int cols = 2, rows = 3;
            var data = new float[][] { new[] { 1, 2f }, new[] { 3, 4f }, new[] { 5, 6f } };
            var columns = new Matrix(rows, cols, data).Columns().ToList();
            columns.Count.Should().Be(2);
            columns[0].Equals(new Vector(3, 1, new float[][] { new[] { 1.0f }, new[] { 3f }, new[] { 5f } })).Should().BeTrue();
            columns[1].Equals(new Vector(3, 1, new float[][] { new[] { 2.0f }, new[] { 4f }, new[] { 6f } })).Should().BeTrue();
        }


    }
}