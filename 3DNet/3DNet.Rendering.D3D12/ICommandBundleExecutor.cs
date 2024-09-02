using SharpDX.Direct3D12;
using System.Collections.Generic;
using System;

namespace _3DNet.Rendering.D3D12;

internal interface ICommandBundleExecutor
{
    void ExecuteCommandBundle(string name, Queue<Action<GraphicsCommandList>> actions,long frame);
}