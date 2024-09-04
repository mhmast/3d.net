using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace _3DNet.Engine.Rendering.Shader
{
    public class ShaderDescription
    {
        public string VertexShaderProfile { get; }

        public ShaderDescription(string shaderFile, string vertexShaderProfile, string vertexShaderMethod, string pixelShaderProfile, string pixelShaderMethod, IEnumerable<ShaderBufferDescription> buffers, string vpBufferName, string objectWorldBufferName)
        {
            VertexShaderProfile = vertexShaderProfile;
            PixelShaderMethod = pixelShaderMethod;
            _buffers = buffers.ToDictionary(b => b.Name, b => b);
            ViewProjectionBufferName = vpBufferName;
            CheckBufferOrThrow(vpBufferName);
            ObjectWorldBufferName = objectWorldBufferName;
            CheckBufferOrThrow(objectWorldBufferName);
            VertexShaderMethod = vertexShaderMethod;
            ShaderFile = shaderFile;
            PixelShaderProfile = pixelShaderProfile;
        }

        private void CheckBufferOrThrow(string bufferName)
        {
            if (!_buffers.ContainsKey(bufferName))
            {
                throw new ArgumentException($"Buffer list did not contain a buffer with name {bufferName}");
            }
            if (_buffers[bufferName].Type != BufferType.GPUInput)
            {
                throw new ArgumentException($"Buffer with name {bufferName} has an invalid {nameof(ShaderBufferDescription.Type)}. It should be {BufferType.GPUInput}.");
            }
            if (_buffers[bufferName].BufferUsage != BufferUsage.VertexShader)
            {
                throw new ArgumentException($"Buffer with name {bufferName} has an invalid {nameof(ShaderBufferDescription.BufferUsage)}. It should be {BufferUsage.VertexShader}.");
            }
        }

        public string PixelShaderMethod { get; }

        private readonly Dictionary<string, ShaderBufferDescription> _buffers;

        public ShaderDescription(Dictionary<string, ShaderBufferDescription> buffers) => _buffers = buffers;

        public IEnumerable<ShaderBufferDescription> Buffers => _buffers.Values;
        public string ViewProjectionBufferName { get; }
        public string ObjectWorldBufferName { get; }
        public string VertexShaderMethod { get; }
        public string ShaderFile { get; }
        public string PixelShaderProfile { get; }

        public string Signature => Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes($"{ShaderFile}|{VertexShaderMethod}|{VertexShaderProfile}|{PixelShaderMethod}|{PixelShaderProfile}")));

    }
}