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

        private IndexBufferView _buffer;
        private SharpDX.Direct3D12.Resource _indexBuffer;

        public IndexBuffer(Device device, IEnumerable<int> data)
        {
            var arr = data.ToArray();
            var bufferSize = sizeof(int) * arr.Length;
            _indexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(bufferSize), ResourceStates.GenericRead);
            var pIndexDataBegin = _indexBuffer.Map(0);
            Utilities.Write(pIndexDataBegin, arr, 0, arr.Length);
            _indexBuffer.Unmap(0);

            _buffer = new IndexBufferView
            {
                BufferLocation = _indexBuffer.GPUVirtualAddress,
                SizeInBytes = bufferSize,
                Format = Format.R32_UInt
            };

        }

        public void Load(GraphicsCommandList commandList) => commandList.SetIndexBuffer(_buffer);

        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                _indexBuffer?.Dispose();
                _indexBuffer = null;
            }

        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}
