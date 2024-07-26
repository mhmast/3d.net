using _3DNet.Engine.Rendering;
using _3DNet.Rendering.Buffer;
using SharpDX;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using Device = SharpDX.Direct3D12.Device;

namespace _3DNet.Rendering.D3D12.Buffer
{
    internal class IndexBuffer : IBuffer
    {
        private readonly int _bufferSize;
        private Resource _indexBuffer;

        public int Count { get; }

        public IndexBuffer(Device device,string name, IEnumerable<int> data)
        {
            var arr = data.ToArray();
            Count = arr.Length;
            _bufferSize = sizeof(int) * arr.Length;
            _indexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(_bufferSize), ResourceStates.GenericRead);
            _indexBuffer.Name = $"indxbffer_{name}";
            var pIndexDataBegin = _indexBuffer.Map(0);
            Utilities.Write(pIndexDataBegin, arr, 0, arr.Length);
            _indexBuffer.Unmap(0);
        }

        public void Load(IRenderWindowContext context) => context.SetIndexBuffer(new IntPtr(_indexBuffer.GPUVirtualAddress), _bufferSize, sizeof(uint));

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
            GC.SuppressFinalize(this);
        }
    }
}
