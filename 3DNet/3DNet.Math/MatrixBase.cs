using System;
using System.Collections.Generic;
using System.Linq;

namespace _3DNet.Math
{
    public abstract class MatrixBase<T, TVector> where T : MatrixBase<T, TVector> where TVector : MatrixBase<TVector, TVector>
    {
        private float[][] _data;
#if DEBUG
        public event Action<int, int, float> MatChanged;
#endif
        protected MatrixBase(float value)
        {
            _data = GenerateData(value);
        }

        public MatrixBase(params TVector[] rows)
        {
            _data = rows.Select(r => r._data.Select(d=>d[0]).ToArray()).ToArray();
        }

        public abstract int Rows { get;  }
        public abstract int Cols { get;  }

        private float[][] GenerateData(float value)
        {
            if (value == 0) return GenerateDataZero();
            var data = new float[Rows][];
            for (var r = 0; r < Rows; r++)
            {
                var c = new float[Cols];
                Array.Fill(c, value);
                data[r] = c;
            }
            return data;
        }

        private float[][] GenerateDataZero()
        {
            var data = new float[Rows][];
            for (var r = 0; r < Rows; r++)
            {
                data[r] = new float[Cols];
            }
            return data;
        }



        protected MatrixBase(float[] data)
        {
            _data = FormatData(data);
        }

        protected MatrixBase(float[][] data)
        {

            if (data != null)
            {
                if (data.Length != Rows || Cols > 0 && data[0].Length != Cols)
                {
                    throw new ArgumentException();
                }
                _data = data;
            }
            else
            {
                _data = GenerateDataZero();
            }
        }

        private float[][] FormatData(float[] data)
        {
            if (data == null) return null;
            var newData = new float[Rows][];
            for (int r = 0; r < Rows; r++)
            {
                newData[r] = new float[Cols];
                Array.Copy(data, r * Cols, newData[r], 0, Cols);
            }
            return newData;
        }


        public float SumRange(int startRow, int endRow, int startCol, int endCol)
        {
            if (startRow * endRow * startCol * endCol == 0)
            {
                return 0;
            }
            if (startRow > Rows || startCol > Cols)
            {
                return 0;
            }

            endRow = System.Math.Min(Rows, endRow);
            endCol = System.Math.Min(Cols, endCol);

            var sum = 0.0f;
            for (var row = startRow; row <= endRow; row++)
            {
                for (var col = startCol; col <= endCol; col++)
                {
                    sum += this[row, col];
                }
            }
            return sum;
        }

        public float SumAll()
        => SumRange(1, Rows, 1, Cols); public float Min()
        {
            var min = float.PositiveInfinity;
            for (var row = 1; row <= Rows; row++)
            {
                for (var col = 1; col <= Cols; col++)
                {
                    min = System.Math.Min(min, this[row, col]);
                }
            }
            return min;
        }

        public float Max()
        {
            var max = float.NegativeInfinity;
            for (var row = 1; row <= Rows; row++)
            {
                for (var col = 1; col <= Cols; col++)
                {
                    max = System.Math.Max(max, this[row, col]);
                }
            }
            return max;
        }

        public T Transpose()
        {
            var newData = new float[Cols][];
            for (int c = 0; c < Cols; c++)
            {
                newData[c] = _data.Select(r => r[c]).ToArray();
            }
            return (T)Activator.CreateInstance(typeof(T), (object)newData);
        }

        public bool Empty()
        => _data.Length == 0;


        public IEnumerable<Point> Find(Predicate<float> expr)
        {
            for (int row = 1; row <= Rows; row++)
            {
                for (int column = 1; column <= Cols; column++)
                {
                    if (expr(this[row, column]))
                    {
                        yield return new Point(column, row);
                    }
                }
            }
        }


        public TVector Row(int row)
        {
            if (Rows > row)
            {
                throw new IndexOutOfRangeException($"Cannot get row {row}. Not enough rows");
            }
            return (TVector)Activator.CreateInstance(typeof(TVector),(object)_data[row]);
        }

        public TVector Col(int col)
        {
            if (Rows != 3)
            {
                throw new ArgumentException("Cannot get a vector3f column from a non 3x? Mat");
            }
            if (Cols > col)
            {
                throw new IndexOutOfRangeException($"Cannot get col {col}. Not enough cols");
            }
            return (TVector)Activator.CreateInstance(typeof(TVector), (object)_data.Select(cols=>cols[col]).ToArray());
        }

        public float this[int row, int col]
        {
            get => _data[row - 1][col - 1];
            set
            {

                _data[row - 1][col - 1] = value;
#if DEBUG
                MatChanged?.Invoke(row, col, value);
#endif
            }
        }

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

        public IEnumerable<TVector> Columns()
        {
            for (int c = 0; c < Cols; c++)
            {
                yield return (TVector)Activator.CreateInstance(typeof(TVector), (object)_data.Select(r => r[c]).ToArray());
            }
        }

        public float this[int pos]
        {
            get
            {
                var (row, col) = GetRowCol(pos);
                return this[row, col];
            }
            set
            {
                var (row, col) = GetRowCol(pos);
                this[row, col] = value;
            }
        }

        public float this[Point pos]
        {
            get => this[pos.X, pos.Y];
            set => this[pos.X, pos.Y] = value;
        }



        public void SetAll(float val)
        {
            _data = GenerateData(val);
        }

        public T Copy()
        => (T)Activator.CreateInstance(typeof(T), (object)_data);


        public T BinaryTresh(float tresh)
        => ProductTemplate(this, (_, __) => tresh, (v, t) => v <= t ? 0 : 1);
        private T ProductTemplate(MatrixBase<T,TVector> other, Func<float, float, float> @operator)
        {
            if (Empty())
            {
                return other.Copy();
            }
            if (other.Empty())
            {
                return Copy();
            }

            if (other.Size == Size)
            {
                return ProductTemplate(this, (row, col) => other[row, col], @operator);
            }
            if (Cols == other.Cols)
            {
                if (Rows == 1)
                {
                    return ProductTemplate(other, (row, col) => this[1, col], @operator);
                }
                if (other.Rows == 1)
                {
                    return ProductTemplate(this, (row, col) => other[1, col], @operator);
                }
            }
            else if (Rows == other.Rows)
            {
                if (Cols == 1)
                {
                    return ProductTemplate(other, (row, col) => this[row, 1], @operator);
                }
                if (other.Cols == 1)
                {
                    return ProductTemplate(this, (row, col) => other[row, 1], @operator);
                }
            }
            throw new ArgumentException();
        }

        public Point Point(int row, int col) => new Point((int)this[row, col], (int)this[row + 1, col]);

        private T ProductTemplate(MatrixBase<T,TVector> m, Func<int, int, float> valueSelector, Func<float, float, float> @operator)
        {
            var retVal = new float[m.Rows][];

            for (int i = 1; i <= m.Rows; i++)
            {
                var col = new float[m.Cols];
                for (int c = 1; c <= Cols; c++)
                {
                    col[c - 1] = @operator(m[i, c], valueSelector(i, c));
                }
                retVal[i - 1] = col;
            }
            return (T)Activator.CreateInstance(typeof(T), (object)retVal);
        }

        public T Mul(float value)
            => ProductTemplate(this, (row, col) => value, (l, r) => l * r);

        public T Mul(MatrixBase<T,TVector> value)
            => ProductTemplate(value, (l, r) => l * r);

        public static T operator *(MatrixBase<T,TVector> m, float value) => m.Mul(value);
        public static T operator *(T m, MatrixBase<T, TVector> value) => m.Mul(value);

        public T Minus(float value)
            => ProductTemplate(this, (row, col) => value, (l, r) => l - r);
        public T Minus(T value)
            => ProductTemplate(value, (l, r) => l - r);

        public static T operator -(MatrixBase<T, TVector> m, float value) => m.Minus(value);
        public static T operator -(MatrixBase<T, TVector> m, T value) => m.Minus(value);

        public T Div(float value)
            => ProductTemplate(this, (row, col) => value, (l, r) => l / r);
        public T Div(T value)
            => ProductTemplate(value, (l, r) => l / r);

        public static T operator /(MatrixBase<T, TVector> m, float value) => m.Div(value);
        public static T operator /(MatrixBase<T, TVector> m, T value) => m.Div(value);


        public T Plus(float value)
            => ProductTemplate(this, (row, col) => value, (l, r) => l + r);
        public T Plus(T value)
            => ProductTemplate(value, (l, r) => l + r);

        public static T operator +(MatrixBase<T, TVector> m, float value) => m.Plus(value);
        public static T operator +(MatrixBase<T, TVector> m, T value) => m.Plus(value);


        public unsafe float[] Data
        {
            get
            {
                var dta = new float[Rows * Cols];
                for (var row = 0; row < Rows; row++)
                {
                    Array.Copy(_data[row], 0, dta, row * Cols, Cols);
                }
                return dta;
            }
        }

        private (int, int) GetRowCol(int pos)
        {
            var row = pos / Cols + (pos % Cols > 0 ? 1 : 0);
            return (row, pos - (row - 1) * Cols);
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


        public T Pow(float power) => ProductTemplate(this, (r, c) => power, (l, r) => (float)System.Math.Pow(l, r));
        public T Floor() => ProductTemplate(this, (r, c) => 0.0f, (l, r) => (float)System.Math.Floor(l));

        
    }
}
