namespace _3DNet.Rendering.D3D12.Buffer;
internal struct WorldBuffer
{
    public SharpDX.Mathematics.Interop.RawMatrix world;
    

    public static implicit operator WorldBuffer(Engine.Rendering.Buffer.WorldBuffer buffer) => new WorldBuffer
    {
        world = new SharpDX.Mathematics.Interop.RawMatrix
        {
            M11 = buffer.world.M11,
            M12 = buffer.world.M12,
            M13 = buffer.world.M13,
            M14 = buffer.world.M14,
            M21 = buffer.world.M21,
            M22 = buffer.world.M22,
            M23 = buffer.world.M23,
            M24 = buffer.world.M24,
            M31 = buffer.world.M31,
            M32 = buffer.world.M32,
            M33 = buffer.world.M33,
            M34 = buffer.world.M34,
            M41 = buffer.world.M41,
            M42 = buffer.world.M42,
            M43 = buffer.world.M43,
            M44 = buffer.world.M44
        }
    };
}
