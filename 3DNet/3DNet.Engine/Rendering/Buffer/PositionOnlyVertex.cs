using _3DNet.Math;

namespace _3DNet.Engine.Rendering.Buffer
{
    internal class PositionOnlyVertex : IVertex
    {
        Vector3F Position { get; }

        public PositionOnlyVertex(Vector3F position)
        {
            Position = position;
        }

        public static implicit operator PositionOnlyVertex(Vector3F vals) => new PositionOnlyVertex(vals);
        public static implicit operator PositionOnlyVertex((float,float,float) vals) => new PositionOnlyVertex(vals);
    }
}
