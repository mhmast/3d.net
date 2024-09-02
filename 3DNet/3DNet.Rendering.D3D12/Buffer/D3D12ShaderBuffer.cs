using _3DNet.Engine.Rendering;
using _3DNet.Engine.Rendering.Shader;
using _3DNet.Rendering.Buffer;
using SharpDX;
using SharpDX.Direct3D12;
using System;
using System.Runtime.InteropServices;

namespace _3DNet.Rendering.D3D12.Buffer
{
    internal class D3D12ShaderBuffer : IWritableBuffer, IBufferWriter
    {
        private readonly Resource _buffer;
        private readonly ShaderBufferDescription _bufferDesc;
        private readonly D3DRenderEngine _d3DRenderEngine;
        private readonly DescriptorHeap _shaderHeap;
        public int Length { get; }
        public long SizeInBytes { get; }
        private object _lastWritenValue;

        private readonly IShaderBufferDataAdapter _dataAdapter;

        public D3D12ShaderBuffer(D3DRenderEngine d3DRenderEngine, DescriptorHeap shaderHeap, ShaderBufferDescription bufferDesc)
        {
            _bufferDesc = bufferDesc;
            var size = Marshal.SizeOf(bufferDesc.DataType);
            var amountof256 = size / 256;
            amountof256 += size % 256 > 0 ? 1 : 0;
            size = amountof256 * 256;

            _d3DRenderEngine = d3DRenderEngine;
            _shaderHeap = shaderHeap;

            _buffer = _d3DRenderEngine.CreateCommittedResource(new HeapProperties { Type = GetHeapType(_bufferDesc.Type), CPUPageProperty = CpuPageProperty.Unknown }, HeapFlags.None,
               ResourceDescription.Buffer(size), GetResourceState(_bufferDesc.Type, GetResourceState(_bufferDesc.BufferUsage)));
            _buffer.Name = bufferDesc.Name;
            var cbvDesc = new ConstantBufferViewDescription()
            {
                BufferLocation = _buffer.GPUVirtualAddress,
                SizeInBytes = size
            };
            _d3DRenderEngine.CreateConstantBufferView(cbvDesc, _shaderHeap.CPUDescriptorHandleForHeapStart);
            Length = 1;
            SizeInBytes = size;
            _dataAdapter = bufferDesc.DataAdapter;
        }

        public D3D12ShaderBuffer()
        {
        }

        private static ResourceStates GetResourceState(BufferUsage bufferUsage)
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

        private static ResourceStates GetResourceState(BufferType type, ResourceStates extra)
        => type switch
        {
            BufferType.GPUInput => ResourceStates.GenericRead | extra,
            BufferType.GPUOutput => ResourceStates.CopyDestination | extra,
            _ => ResourceStates.Common | extra
        };

        private static HeapType GetHeapType(BufferType type)
        => type switch
        {
            BufferType.GPUOutput => HeapType.Readback,
            BufferType.GPUInput => HeapType.Upload,
            _ => HeapType.Default
        };

        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                _buffer?.Dispose();
            }

        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        public void Load(IRenderContextInternal context) => context.LoadShaderBuffer(_bufferDesc.Slot, new IntPtr(_buffer.GPUVirtualAddress));

        public unsafe void Write<T>(T value)
            where T : struct
        {
            if (!value.Equals(_lastWritenValue))
            {
                _dataAdapter.ConvertAndWrite(value, this);
                _lastWritenValue = value;
            }
        }

        unsafe void IBufferWriter.Write<T>(T o) where T : struct
        {
            var addr = _buffer.Map(0);
            Utilities.Write(addr, ref o);
            _buffer.Unmap(0);
        }
    }
}
