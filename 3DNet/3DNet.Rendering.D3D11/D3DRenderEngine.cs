using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Buffer;
using SharpDX.Direct3D11;
using Device = SharpDX.Direct3D11.Device;
using System;
using System.Drawing;
using SharpDX.Direct3D;

namespace _3DNet.Rendering.D3D11
{
    internal class D3DRenderEngine : IRenderEngine
    {
        
        public Device Device { get ; set ; }

        public void BeginScene(IRenderTarget target,Color clearColor)
        { 
            target.Clear(clearColor);
        }

        public void EndScene(IRenderTarget target)
        {
            target.Present();
        }

        public void Initialize()
        {
            Device = new Device(SharpDX.Direct3D.DriverType.Warp, DeviceCreationFlags.Debug, FeatureLevel.Level_11_0);
        }

        public void SetIndexBuffer(IIndexBuffer indexBuffer)
        {
    //        throw new NotImplementedException();
        }

        public void SetVertexBuffer(IVertexBuffer buffer)
        {
  //          throw new NotImplementedException();
        }

        public void SetWorld(_3DNet.Math.Matrix4x4 world)
        {
//throw new NotImplementedException();
        }
    }
}
