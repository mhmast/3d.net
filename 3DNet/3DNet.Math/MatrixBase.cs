using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3DNet.Math
{


    public abstract class MatrixBase<TMatrix, TRowVector, TColumnVector> : IMatrix<TMatrix>
        where TMatrix : MatrixBase<TMatrix, TRowVector, TColumnVector>
        where TRowVector : MatrixBase<TRowVector, Scalar, Scalar>, IMatrix
        where TColumnVector : MatrixBase<TColumnVector, Scalar, Scalar>, IMatrix
    {
        public MatrixBase(params TRowVector[] rows)
        {
            _rows = rows;
        }

        public MatrixBase(params Scalar[] values)
        {
            _rows = ScalarsToRows(values);
        }

        public abstract int Rows { get; }
        public abstract int Cols { get; }
        public abstract TMatrix Instance { get; }

        private TRowVector[] ScalarsToRows(Scalar[] values)
        {
            if (values.Length % Cols > 0)
            {
                throw new ArgumentException($"Number of scalars must be dividable by {Cols}");
            }
            IEnumerable<TRowVector> Convert(Scalar[] values)
            {
                for (int i = 0; i < Rows; i++)
                {
                    yield return CreateRow(values.Skip(i * Cols).Take(Cols).ToArray());
                }
            }
            return Convert(values).ToArray();
        }

        private TRowVector[] _rows;

        protected MatrixBase(Scalar value) => _rows = GenerateData(value);

        public float Min() => Data.Min(s => s);

        public float Max() => Data.Max(s => s);

        public virtual TRowVector this[int row]
        {
            get => _rows[row];
            set
            => _rows[row] = value;
        }


        public virtual Scalar[] Data
        {
            get
            {
                var dta = new Scalar[Rows * Cols];
                for (var row = 0; row < Rows; row++)
                {
                    Array.Copy(_rows[row].Data, 0, dta, row * Cols, Cols);
                }
                return dta;
            }
        }

        //public static Mat FromCV(OpenCvSharp.Mat cvMat)
        //{
        //    return new Mat(cvMat.Rows, cvMat.Cols, ResolveCvData(cvMat));
        //}

        //public static float[] ResolveCvData(OpenCvSharp.Mat m)
        //    => m.ElemSize() switch
        //    {
        //        1 when m.Channels() == 1 => GetByteArray(m),
        //        8 when m.Channels() == 1 => GetArray<float>(m),
        //        _ => throw new ArgumentException()
        //    };

        //private unsafe static float[] GetByteArray(OpenCvSharp.Mat m)
        //{
        //    var bytes = GetArray<byte>(m);
        //    return bytes.Select(Convert.Tofloat).ToArray();
        //}
        //private unsafe static float[] GetfloatArray(OpenCvSharp.Mat m)
        //=> GetArray<float>(m);
        //private unsafe static T[] GetArray<T>(OpenCvSharp.Mat m) where T : unmanaged
        //{
        //    var buffer = new T[m.Rows * m.Cols];
        //    fixed (void* dtaPtr = &buffer[0])
        //    {
        //        var len = buffer.Length * sizeof(T);
        //        Buffer.MemoryCopy(m.DataPointer, dtaPtr, len, len);
        //        return buffer;
        //    }
        //}

        public Point Size
        => new Point(Cols, Rows);

        
        IMatrix IMatrix.this[int row] {get =>this[row];set=> this[row] = CreateRow(value.Data); }

        public virtual IMatrix Col(int col) => CreateColumn(_rows.Select(row => row[col]).ToArray());

        //public Matrix Range(int startRow, int endRow, int startCol, int endCol)
        //{
        //    if (startRow * endRow * startCol * endCol == 0)
        //    {
        //        throw new ArgumentException();
        //    }
        //    var noRows = endRow - startRow + 1;
        //    var noCols = endCol - startCol + 1;
        //    var overlapCols = System.Math.Min(Cols, endCol) - startCol + 1;
        //    var overlapRows = System.Math.Min(Rows, endRow) - startRow + 1;
        //    var retMat = Zeros(noRows, noCols);
        //    if (overlapCols == 0 || overlapRows == 0)
        //    {
        //        return retMat;
        //    }

        //    for (int row = 0; row < overlapRows; row++)
        //    {
        //        Array.Copy(_data[row + startRow - 1], startCol - 1, retMat._data[row], 0, overlapCols);
        //    }

        //    return retMat;

        //    //for (var row = startRow; row <= minRow; row++)
        //    //{
        //    //    var addr = GetArrAddr(row, startCol);
        //    //    Array.Copy(_data, addr, newData, (row - startRow) * noCols, minCol - startCol + 1);
        //    //    //for (var col = startCol; col <= minCol; col++)
        //    //    //{
        //    //    //    var addr2 = GetArrAddr(row, col);
        //    //    //    newData2[row*noCols+(col - startCol)] = _data[addr2];
        //    //    //}
        //    //}
        //}

        public IEnumerable<IMatrix> Columns()
        {
            for (int c = 0; c < Cols; c++)
            {
                yield return Col(c);
            }
        }


        public TMatrix Copy() => CreateMatrixFromRows(_rows);

        public bool Empty()
        => _rows.Length == 0;

        public override bool Equals(object obj)
        {
            if (obj is MatrixBase<TMatrix, TRowVector, TColumnVector> other)
            {
                return Data.SequenceEqual(other.Data);
            }
            return false;
        }



        public TMatrix Pow(Scalar power) => ProductTemplate((TMatrix)null, (r, l, j) => MaskCol(j), (l, r) => (float)System.Math.Pow((l * r).SumAll(), power));

        private IMatrix MaskCol(int row)
        {
            var col = CreateColumnZeros();
            col[row] = 1;
            return col;
        }

        public void SetAll(float val)
        {
            _rows = GenerateData(val);
        }

        public Scalar SumAll() => Data.Sum(s=>s);
        public TMatrix Transpose()
        {
            var newData = new TColumnVector[Cols];
            for (int c = 0; c < Cols; c++)
            {
                newData[c] = CreateColumn(_rows.Select(r => r[c]).ToArray());
            }
            return CreateMatrixFromColumns(newData);
        }


        private TRowVector[] GenerateData(float value)
        {
            if (value == 0)
            {
                return GenerateDataZero();
            }
            var data = new TRowVector[Rows];
            for (var r = 0; r < Rows; r++)
            {
                var row = CreateRowZeros();
                for(int col=0;col<row.Cols;col++)
                {
                    row[col] = value;
                }
                data[r] = row;
            }
            return data;
        }

        private TRowVector[] GenerateDataZero()
        {
            var data = new TRowVector[Rows];
            for (var r = 0; r < Rows; r++)
            {
                data[r] = CreateRowZeros();
            }
            return data;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var row in _rows)
            {
                builder.AppendLine($"{{{string.Join(',', (object[])row.Data)}}}");
            }
            return builder.ToString();
        }

        private TMatrix ProductTemplate<TRight>(TRight right, Func<TRight, IMatrix, int, IMatrix> colSelector, Func<TRowVector, IMatrix, Scalar> @operator)
        where TRight: IMatrix
        {
            if (Empty() || right.Empty())
            {
                return Copy();
            }
            var retVal = Zeros();

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    retVal[r][c] = @operator(this[r], colSelector(right, this[r], c));
                }
            }
            return retVal;
        }

        //private static T CreateRowVector<T>(Scalar[] data) where T : IMatrix<T> => (T)Activator.CreateInstance(typeof(T), data);
        //private static T CreateRowVector<T>(Scalar data) where T : IMatrix<T> => (T)Activator.CreateInstance(typeof(T), data);
        //private static T CreateColVector<T>(Scalar[] data) where T : IMatrix<T> => (T)Activator.CreateInstance(typeof(T), data);
        //private static T CreateColVector<T>(Scalar data) where T : IMatrix<T> => (T)Activator.CreateInstance(typeof(T), data);
        public abstract TMatrix CreateMatrix
           (params Scalar[] values);

        public abstract TColumnVector CreateColumn(params Scalar[] values);
        public abstract TColumnVector CreateColumnZeros();
        public abstract TRowVector CreateRow(params Scalar[] values);
        public abstract TRowVector CreateRowZeros();

        public abstract TMatrix CreateMatrixFromColumns(params IMatrix[] cols);
        public abstract TMatrix CreateMatrixFromRows(params IMatrix[] rows);

        public IEnumerable<Point> Find(Predicate<float> expr)
        {
            for (int row = 1; row <= Rows; row++)
            {
                for (int column = 1; column <= Cols; column++)
                {
                    if (expr(this[row][column]))
                    {
                        yield return new Point(column, row);
                    }
                }
            }
        }
        internal protected virtual TMatrix Times(IMatrix value) => ProductTemplate( value, (r, l, c) => r.GetColForProductWith(l,c), (r, c) => r.Times(c).SumAll());
        //protected virtual TMatrix Times(Scalar value) => ProductTemplate(CreateColumnZeros(), (r, i, j) => MaskCol<TColumnVector>(i), (r, c) => r.Times(c).SumAll() * value);
        // protected virtual TMatrix Times(TColumnVector value) => ProductTemplate(value, (r, i, j) => r, (r, c) => r.Times(c).SumAll());
        internal protected virtual TMatrix Minus(IMatrix value) => ProductTemplate( value, (r, l, c) => r.GetColForProductWith(l, c), (r, c) => r.Minus(c).SumAll());
        //protected virtual TMatrix Minus(Scalar value) => ProductTemplate(CreateColumnZeros(), (r, i, j) => MaskCol<TColumnVector>(i), (r, c) => r.Minus(c).SumAll() - value);
        //protected virtual TMatrix Minus(TColumnVector value) => ProductTemplate(value, (r, i, j) => r, (r, c) => r.Minus(c).SumAll());
        internal protected virtual TMatrix Div(IMatrix value) => ProductTemplate( value, (r, l, c) => r.GetColForProductWith(l, c), (r, c) => r.Div(c).SumAll());
        // protected virtual TMatrix Div(Scalar value) => ProductTemplate(CreateColumnZeros(), (r, i, j) => MaskCol<TColumnVector>(i), (r, c) => r.Div(c).SumAll() / value);
        //protected virtual TMatrix Div(TColumnVector value) => ProductTemplate(value, (r, i, j) => r, (r, c) => r.Div(c).SumAll());
        internal protected virtual TMatrix Plus(IMatrix value) => ProductTemplate( value, (r, l, c) => r.GetColForProductWith(l, c), (r, c) => r.Plus(c).SumAll());
        //protected virtual TMatrix Plus(Scalar value) => ProductTemplate(CreateColumnZeros(), (r, i, j) => MaskCol<TColumnVector>(i), (r, c) => r.Plus(c).SumAll() + value);
        //protected virtual TMatrix Plus(TColumnVector value) => ProductTemplate(value, (r, i, j) => r, (r, c) => r.Plus(c).SumAll());

        public TMatrix Zeros() => CreateMatrixFromRows(GenerateDataZero());

      
        public override int GetHashCode()
        {
            return HashCode.Combine(Rows, Cols, Data);
        }

        public virtual IMatrix GetColForProductWith(IMatrix left,int col)
        => Col(col);

        public IMatrix Row(int j)
        => this[j];

        IMatrix IMatrix.CreateRow(params Scalar[] values)
        => CreateRow(values);

        IMatrix IMatrix.CreateColumn(params Scalar[] values)
        => CreateColumn(values);

        IMatrix IMatrix.CreateMatrix(params Scalar[] values)
        => CreateMatrix(values);

        IMatrix IMatrix.Transpose()
        => Transpose();

        //public static TMatrix operator *(MatrixBase<TMatrix, TRowVector, TColumnVector> m, Scalar value) => m.Times(value);
        public static TMatrix operator *(MatrixBase<TMatrix, TRowVector, TColumnVector> m, IMatrix value) => m.Times(value);
      //  public static TMatrix operator *(MatrixBase<TMatrix, TRowVector, TColumnVector> m, TColumnVector value) => m.Times(value);
        public static TRowVector operator *(TRowVector value, MatrixBase<TMatrix, TRowVector, TColumnVector> m) => value.Times(m);

        //public static TMatrix operator -(MatrixBase<TMatrix, TRowVector, TColumnVector> m, Scalar value) => m.Minus(value);
        public static TMatrix operator -(MatrixBase<TMatrix, TRowVector, TColumnVector> m, IMatrix value) => m.Minus(value);
       // public static TMatrix operator -(MatrixBase<TMatrix, TRowVector, TColumnVector> m, TColumnVector value) => m.Minus(value);
        //public static TRowVector operator -(TRowVector value, MatrixBase<TMatrix, TRowVector, TColumnVector> m) => value.Minus(m);

       // public static TMatrix operator /(MatrixBase<TMatrix, TRowVector, TColumnVector> m, Scalar value) => m.Div(value);
        public static TMatrix operator /(MatrixBase<TMatrix, TRowVector, TColumnVector> m, IMatrix value) => m.Div(value);
      //  public static TMatrix operator /(MatrixBase<TMatrix, TRowVector, TColumnVector> m, TColumnVector value) => m.Div(value);
        //public static TRowVector operator /(TRowVector value, MatrixBase<TMatrix, TRowVector, TColumnVector> m) => value.Div(m);

        //public static TMatrix operator +(MatrixBase<TMatrix, TRowVector, TColumnVector> m, Scalar value) => m.Plus(value);
        public static TMatrix operator +(MatrixBase<TMatrix, TRowVector, TColumnVector> m, IMatrix value) => m.Plus(value);
       // public static TMatrix operator +(MatrixBase<TMatrix, TRowVector, TColumnVector> m, TColumnVector value) => m.Plus(value);
        //public static TRowVector operator +(TRowVector value, MatrixBase<TMatrix, TRowVector, TColumnVector> m) => value.Plus(m);
    }



}
