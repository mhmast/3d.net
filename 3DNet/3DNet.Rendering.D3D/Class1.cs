using _3DNet.Engine.Rendering;
using _3DNet.Math;

namespace _3DNet.Rendering.D3D11
{
    public class D3D11RendingEngine : IRenderEngine
    {
        private Matrix4x4 _world;

        public void SetWorld(Matrix4x4 world)
        {
            _world = world;
        }
    }
}
