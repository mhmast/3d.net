using _3DNet.Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace _3DNet.Rendering.D3D11
{
    internal class D3DRenderTargetFactory : IRenderTargetFactory
    {
        private readonly D3DRenderEngine _engine;
        private readonly IDictionary<string, IRenderTarget> _createdTargets = new Dictionary<string, IRenderTarget>();

        public D3DRenderTargetFactory(D3DRenderEngine engine)
        {
            _engine = engine;
        }
        public IRenderWindow CreateWindow(Size size, string name, bool fullScreen = false)
        {
            if (_createdTargets.ContainsKey(name))
            {
                throw new ArgumentException($"There is already a rendertarget with the name {name}");
            }
            var form = new D3DRenderForm(_engine.Device, name, fullScreen) { Size = size };
            _createdTargets.Add(name, form);
            form.Show();
            return form;
        }
    }
}
