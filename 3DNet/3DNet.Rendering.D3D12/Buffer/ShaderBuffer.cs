using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.Buffer;
using SharpDX.Direct3D12;
using System;
using System.Runtime.InteropServices;

namespace _3DNet.Rendering.D3D12.Buffer
{
    internal unsafe class ShaderBuffer : IBuffer
    {
        private readonly Resource _buffer;
        private readonly ShaderBufferDescription _bufferDesc;

        public int Count { get; private set; }

        public ShaderBuffer(string name, D3DRenderEngine d3DRenderEngine, DescriptorHeap shaderHeap, ShaderBufferDescription bufferDesc)
        {
            var realSize = (bufferDesc.SizeInBytes / 256 + bufferDesc.SizeInBytes % 256 > 0 ? 1 : 0) * 256;
            _buffer = d3DRenderEngine.CreateCommittedResource(new HeapProperties { Type = GetHeapType(bufferDesc.Type), CPUPageProperty = CpuPageProperty.Unknown }, HeapFlags.None,
                ResourceDescription.Buffer(realSize), GetResourceState(bufferDesc.Type, GetResourceState(bufferDesc.BufferUsage)));
            _buffer.Name = name;
            _bufferDesc = bufferDesc;
            var cbvDesc = new ConstantBufferViewDescription()
            {
                BufferLocation = _buffer.GPUVirtualAddress,
                SizeInBytes = realSize
            };
            d3DRenderEngine.CreateConstantBufferView(cbvDesc, shaderHeap.CPUDescriptorHandleForHeapStart);

        }

        private ResourceStates GetResourceState(BufferUsage bufferUsage)
        {
            var states = ResourceStates.Common;
            if ((bufferUsage & BufferUsage.VertexShader) != 0)
            {
                states |= ResourceStates.NonPixelShaderResource;
            }
            if ((bufferUsage & BufferUsage.PixelShader) != 0)
            {
                states |= ResourceStates.PixelShaderResource;
            }
            return states;
        }

        private ResourceStates GetResourceState(BufferType type, ResourceStates extra)
        => type switch
        {
            BufferType.GPUInput => ResourceStates.GenericRead | extra,
            BufferType.GPUOutput => ResourceStates.CopyDestination | extra,
            _ => ResourceStates.Common | extra
        };

        private HeapType GetHeapType(BufferType type)
        => type switch
        {
            BufferType.GPUOutput => HeapType.Readback,
            BufferType.GPUInput => HeapType.Upload,
            _ => HeapType.Default
        };

        public void Dispose()
        {

        }

        public void Load(IRenderWindowContext context)
        {
            context.QueueAction(() =>
            {
                var addr = _buffer.Map(0);
                var data = _bufferDesc.Data(context);
                Count = data is Array a ? a.Length : 1;
                var dataSize = Marshal.SizeOf(data);
                var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                System.Buffer.MemoryCopy((void*)handle.AddrOfPinnedObject(), (void*)addr, dataSize, dataSize);
                handle.Free();
                _buffer.Unmap(0);
            });
            context.LoadShaderBuffer(_bufferDesc.Slot, new IntPtr(_buffer.GPUVirtualAddress));
        }
    }
}
