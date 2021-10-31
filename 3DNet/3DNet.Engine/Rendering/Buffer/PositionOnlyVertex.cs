using _3DNet.Math;

namespace _3DNet.Engine.Rendering.Buffer
{
    public struct PositionOnlyVertex : IVertex
    {
        Vector3F Position { get; }

        public byte[] RawBuffer
        {
            get
            {
                var buffer = new byte[Position.Data.Length * sizeof(float)];
                System.Buffer.BlockCopy(Position.Data, 0,buffer,0,buffer.Length);
                return buffer;
            }
        }

        public PositionOnlyVertex(Vector3F position)
        {
            Position = position;
        }

        public static implicit operator PositionOnlyVertex(Vector3F vals) => new PositionOnlyVertex(vals);
        public static implicit operator PositionOnlyVertex((float,float,float) vals) => new PositionOnlyVertex(vals);
    }
}
