using SharpDX;
using SharpDX.Direct3D12;

using SharpDX.Windows;
using System.Runtime.InteropServices;
using SharpDX.Direct3D;

using Device = SharpDX.Direct3D12.Device;
using SharpDX.DXGI;
using Resource = SharpDX.Direct3D12.Resource;
using Vector3 = SharpDX.Vector3;
using Viewport = SharpDX.Viewport;
using System;
using System.Diagnostics;
using InfoQueue = SharpDX.Direct3D12.InfoQueue;
using SharpDX.D3DCompiler;
using System.Threading;
using Example;

class Program
{
    static void Main()
    {

        var form = new RenderForm("Hello Bundles")
        {
            Width = 1280,
            Height = 800
        };
        form.Show();

        using (var app = new HelloBundles())
        {
            app.Initialize(form);

            using (var loop = new RenderLoop(form))
            {
                while (loop.NextFrame())
                {
                    app.Update();
                    app.Render();
                }
            }
        } 
    }
}