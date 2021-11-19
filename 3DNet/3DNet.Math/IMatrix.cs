namespace _3DNet.Math
{
    public interface IMatrix<T> : IMatrix where T : IMatrix<T>
    {
        T Copy();
        T Zeros();
        T CreateMatrix(params Scalar[] values);
        T CreateMatrixFromColumns(params IVector[] cols);
        T CreateMatrixFromRows(params IVector[] rows);

    }
    public interface IMatrix
    {
        int Rows { get; }
        int Cols { get; }
        Scalar[] Data { get; }
        bool Empty();
        IVector Col(int j);
        IVector Row(int j);
        IVector this[int row] { get;set; }
    }
}