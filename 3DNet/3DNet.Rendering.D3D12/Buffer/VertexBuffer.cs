using _3DNet.Engine.Rendering;
using _3DNet.Rendering.Buffer;
using SharpDX.Direct3D12;
using System;
using System.Runtime.InteropServices;
using Device = SharpDX.Direct3D12.Device;


namespace _3DNet.Rendering.D3D12.Buffer
{

    internal unsafe class VertexBuffer<T> : IBuffer where T : struct
    {

        public VertexBuffer(Device device, string name,T[] data)
        {
            if (data.Length == 0) throw new ArgumentException("Vertex length cannot be 0");
            _structSize = Marshal.SizeOf(data[0]);
            _bufferCount = data.Length;
            _bufferSize = _structSize * _bufferCount;
            _vertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(_bufferSize), ResourceStates.GenericRead);
            _vertexBuffer.Name = $"vrtxbffr_${name}";
            var pVertexDataBegin = _vertexBuffer.Map(0);
            foreach (var buffer in data)
            {
                var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                System.Buffer.MemoryCopy((void*)handle.AddrOfPinnedObject(), (void*)pVertexDataBegin, _structSize,_structSize);
                handle.Free();
                pVertexDataBegin += _structSize;
            }
            _vertexBuffer.Unmap(0);

        }
        private Resource _vertexBuffer;
        private readonly int _structSize;
        private readonly int _bufferSize;
        private readonly int _bufferCount;

        public int Count =>_bufferCount;

        public void Load(IRenderWindowContext context) => context.SetVertexBuffer(new IntPtr(_vertexBuffer.GPUVirtualAddress), _bufferSize, _structSize);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _vertexBuffer?.Dispose();
                _vertexBuffer = null;
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
