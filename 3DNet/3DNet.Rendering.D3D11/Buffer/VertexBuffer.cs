using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Device = SharpDX.Direct3D12.Device;


namespace _3DNet.Rendering.D3D12.Buffer
{

    internal unsafe abstract class VertexBuffer : IBuffer
    {

        public VertexBuffer(Device device, Array data, int structSize)
        {
            var arr = data;
            var bufferSize = structSize * arr.Length;
            var vertexBuffer = device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(bufferSize), ResourceStates.GenericRead);
            IntPtr pVertexDataBegin = vertexBuffer.Map(0);
            var byteptr = GCHandle.Alloc(data);
            System.Buffer.MemoryCopy((void*)byteptr.AddrOfPinnedObject(), (void*)pVertexDataBegin, bufferSize, bufferSize);
            vertexBuffer.Unmap(0);
            byteptr.Free();
            _buffer = new VertexBufferView
            {
                BufferLocation = vertexBuffer.GPUVirtualAddress,
                StrideInBytes = structSize,
                SizeInBytes = bufferSize
            };

        }
        private readonly VertexBufferView _buffer;

        public void Load(GraphicsCommandList commandList) => commandList.SetVertexBuffer(0, _buffer);

    }
    internal class VertexBuffer<T> : VertexBuffer
        where T : struct//,IVertex
    {
        public VertexBuffer(Device device, IEnumerable<T> data) : base(device, data.ToArray(), Marshal.SizeOf<T>())
        {

        }

    }
}
