using System.Collections.Generic;

namespace _3DNet.Engine.Rendering.Buffer
{
    internal class VertexBuffer<T> : List<T> ,IVertexBuffer where T : IVertex
    {
    }
}
