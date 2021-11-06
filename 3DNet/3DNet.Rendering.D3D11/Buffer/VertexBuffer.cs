using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Buffer;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Device = SharpDX.Direct3D12.Device;


namespace _3DNet.Rendering.D3D12.Buffer
{

    internal abstract class VertexBuffer : IBuffer
    {

        public VertexBuffer(Device device, IVertex[] data)
        {
            if (data.Length == 0) throw new ArgumentException("Vertex length cannot be 0");
            _structSize = data[0].RawBuffer.Length;
            _bufferSize = _structSize * data.Length;
            _vertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(_bufferSize), ResourceStates.GenericRead);
            var pVertexDataBegin = _vertexBuffer.Map(0);
            foreach (var buffer in data.Select(v => v.RawBuffer))
            {
                Marshal.Copy(buffer, 0, pVertexDataBegin, buffer.Length);
                pVertexDataBegin += data.Length;
            }
            _vertexBuffer.Unmap(0);

        }
        private Resource _vertexBuffer;
        private int _structSize;
        private int _bufferSize;

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
    internal class VertexBuffer<T> : VertexBuffer
        where T : IVertex
    {
        private static GCHandle _handle;

        public VertexBuffer(Device device, IEnumerable<T> data) : base(device, data.Cast<IVertex>().ToArray())
        {

        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _handle.Free();
            }
        }

    }
}
