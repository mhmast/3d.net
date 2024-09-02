namespace _3DNet.Rendering.D3D12.Buffer;
internal struct ViewProjectionBuffer
{
    //public SharpDX.Mathematics.Interop.RawMatrix world;
    public SharpDX.Mathematics.Interop.RawMatrix view;
    public SharpDX.Mathematics.Interop.RawMatrix projection;

    public static implicit operator ViewProjectionBuffer(Engine.Rendering.Buffer.ViewProjectionBuffer buffer) => new ViewProjectionBuffer
    {
        //world = new SharpDX.Mathematics.Interop.RawMatrix
        //{
        //    M11 = buffer.world.M11,
        //    M12 = buffer.world.M12,
        //    M13 = buffer.world.M13,
        //    M14 = buffer.world.M14,
        //    M21 = buffer.world.M21,
        //    M22 = buffer.world.M22,
        //    M23 = buffer.world.M23,
        //    M24 = buffer.world.M24,
        //    M31 = buffer.world.M31,
        //    M32 = buffer.world.M32,
        //    M33 = buffer.world.M33,
        //    M34 = buffer.world.M34,
        //    M41 = buffer.world.M41,
        //    M42 = buffer.world.M42,
        //    M43 = buffer.world.M43,
        //    M44 = buffer.world.M44
        //},
        view = new SharpDX.Mathematics.Interop.RawMatrix
        {
            M11 = buffer.view.M11,
            M12 = buffer.view.M12,
            M13 = buffer.view.M13,
            M14 = buffer.view.M14,
            M21 = buffer.view.M21,
            M22 = buffer.view.M22,
            M23 = buffer.view.M23,
            M24 = buffer.view.M24,
            M31 = buffer.view.M31,
            M32 = buffer.view.M32,
            M33 = buffer.view.M33,
            M34 = buffer.view.M34,
            M41 = buffer.view.M41,
            M42 = buffer.view.M42,
            M43 = buffer.view.M43,
            M44 = buffer.view.M44
        },
        projection = new SharpDX.Mathematics.Interop.RawMatrix
        {
            M11 = buffer.projection.M11,
            M12 = buffer.projection.M12,
            M13 = buffer.projection.M13,
            M14 = buffer.projection.M14,
            M21 = buffer.projection.M21,
            M22 = buffer.projection.M22,
            M23 = buffer.projection.M23,
            M24 = buffer.projection.M24,
            M31 = buffer.projection.M31,
            M32 = buffer.projection.M32,
            M33 = buffer.projection.M33,
            M34 = buffer.projection.M34,
            M41 = buffer.projection.M41,
            M42 = buffer.projection.M42,
            M43 = buffer.projection.M43,
            M44 = buffer.projection.M44
        }
    };
}
