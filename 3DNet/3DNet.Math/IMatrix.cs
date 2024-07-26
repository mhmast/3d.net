namespace _3DNet.Math
{
    internal interface IMatrix<T> : IMatrix where T : IMatrix<T>
    {
        T Copy();
        T Zeros();
        T CreateMatrixFromColumns(params IMatrix[] cols);
        T CreateMatrixFromRows(params IMatrix[] rows);

    }
    public interface IMatrix
    {
        IMatrix GetColForProductWith(IMatrix left, int col);
        IMatrix CreateRow(params Scalar[] values);
        IMatrix CreateColumn(params Scalar[] values);
        IMatrix CreateMatrix(params Scalar[] values);
        int Rows { get; }
        int Cols { get; }
        Scalar[] Data { get; }
        bool Empty();
        IMatrix Col(int j);
        IMatrix Row(int j);
        IMatrix this[int row] { get; set; }
        IMatrix Transpose();
    }
}