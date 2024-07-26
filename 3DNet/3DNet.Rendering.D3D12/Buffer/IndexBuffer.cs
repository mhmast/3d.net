﻿using _3DNet.Engine.Rendering;
using _3DNet.Rendering.Buffer;
using SharpDX;
using SharpDX.Direct3D12;
using System;
using Device = SharpDX.Direct3D12.Device;

namespace _3DNet.Rendering.D3D12.Buffer
{
    internal class IndexBuffer : IBuffer<uint>
    {
        private int _bufferLength;
        private Resource _indexBuffer;
        private readonly string _name;
        private readonly Device _device;

        public int Length { get; private set; }

        public IndexBuffer(Device device,string name, uint[] data)
        {
            _name = name;
            _device = device;
            InitializeBuffer(data);
        }

        private void InitializeBuffer(uint[] data)
        {
            Length = data.Length;

            _bufferLength = Utilities.SizeOf(data);
            _indexBuffer = _device.CreateCommittedResource(new HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(_bufferLength), ResourceStates.GenericRead);
            _indexBuffer.Name = $"indxbffer_{_name}";

            var pIndexDataBegin = _indexBuffer.Map(0);
            Utilities.Write(pIndexDataBegin, data, 0, data.Length);
            _indexBuffer.Unmap(0);
        }

        public void Load(IRenderContextInternal context) => context.SetIndexBuffer(new IntPtr(_indexBuffer.GPUVirtualAddress), _bufferLength, Utilities.SizeOf<uint>());

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

        public void Write(uint[] values)
        {
            InitializeBuffer(values);
        }
    }
}
