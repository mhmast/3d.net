using _3DNet.Engine.Rendering;
using _3DNet.Rendering.Buffer;
using SharpDX;
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
            _structSize = Utilities.SizeOf<T>();
            _bufferLength = data.Length;
            _bufferSize = Utilities.SizeOf(data);
            _vertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(_bufferSize), ResourceStates.GenericRead);
            _vertexBuffer.Name = $"vrtxbffr_${name}";
            var pVertexDataBegin = _vertexBuffer.Map(0);
            Utilities.Write(pVertexDataBegin, data, 0, data.Length);
            _vertexBuffer.Unmap(0);

        }
        private Resource _vertexBuffer;
        private readonly int _structSize;
        private readonly int _bufferSize;
        private readonly int _bufferLength;

        public int Length => _bufferLength;

        public void Load(IRenderContextInternal context) => context.SetVertexBuffer(new IntPtr(_vertexBuffer.GPUVirtualAddress), _bufferSize, _structSize);

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
