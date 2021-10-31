using _3DNet.Engine.Rendering;
using _3DNet.Rendering.D3D12.RenderTargets;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace _3DNet.Rendering.D3D12
{
    internal class D3DRenderTargetFactory : IRenderTargetFactory
    {
        private readonly D3DRenderEngine _engine;
        private readonly IDictionary<string, ID3DRenderTarget> _activeTargets = new Dictionary<string, ID3DRenderTarget>();

        public int NoOfCreatedTargets => _activeTargets.Count;

        public Format[] RenderTargetFormats => _activeTargets.Values.Select(t=>t.Format).ToArray();

        public D3DRenderTargetFactory(D3DRenderEngine engine)
        {
            _engine = engine;
        }

        public event Action RenderTargetCreated;
        public event Action RenderTargetDropped;

        public IRenderWindow CreateWindow(Size size, string name, bool fullScreen = false)
        {
            if (_activeTargets.ContainsKey(name))
            {
                throw new ArgumentException($"There is already a rendertarget with the name {name}");
            }
            var form = _engine.CreateRenderForm(name, fullScreen, size);
            _activeTargets.Add(name, form);
            form.FormClosed += (_, __) =>
            {
                _activeTargets.Remove(name);
                RenderTargetDropped?.DynamicInvoke();
            };
            form.Activated+= (_,__) => RenderTargetCreated?.DynamicInvoke();
            form.Show();

            return form;
        }
    }
}
