using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using System.Collections.Generic;
using System.Linq;
using Device = SharpDX.Direct3D12.Device;

namespace _3DNet.Rendering.D3D12.Buffer
{
    internal class IndexBuffer : IBuffer
    {
        
        private readonly IndexBufferView _buffer;
        
        public IndexBuffer(Device device, IEnumerable<int> data)
        {
            var arr = data.ToArray();
            var bufferSize = sizeof(int) * arr.Length;
            var indexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(bufferSize), ResourceStates.GenericRead);
            var pIndexDataBegin = indexBuffer.Map(0);
            Utilities.Write(pIndexDataBegin, arr, 0, arr.Length);
            indexBuffer.Unmap(0);

            _buffer = new IndexBufferView
            {
                BufferLocation = indexBuffer.GPUVirtualAddress,
                SizeInBytes = bufferSize,
                Format = Format.R32_UInt
            };

        }

        public void Load(GraphicsCommandList commandList) => commandList.SetIndexBuffer(_buffer);
    }
}
