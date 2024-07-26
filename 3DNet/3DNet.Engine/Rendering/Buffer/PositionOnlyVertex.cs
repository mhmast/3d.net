using System.Numerics;

namespace _3DNet.Engine.Rendering.Buffer
{
    public struct PositionOnlyVertex
    {
        Vector3 Position { get; }

        public PositionOnlyVertex(Vector3 position)
        {
            Position = position;
        }

        public static implicit operator PositionOnlyVertex(Vector3 vals) => new(vals);
        public static implicit operator PositionOnlyVertex((float,float,float) vals) => new(new Vector3(vals.Item1,vals.Item2,vals.Item3));
    }
}
