using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace _3DNet.Engine.Rendering.Shader
{
    public class ShaderDescription
    {
        public string VertexShaderProfile { get; }

        public ShaderDescription(string shaderFile, string vertexShaderProfile, string vertexShaderMethod, string pixelShaderProfile, string pixelShaderMethod, IEnumerable<ShaderBufferDescription> buffers, string wvpBufferName)
        {
            VertexShaderProfile = vertexShaderProfile;
            PixelShaderMethod = pixelShaderMethod;
            _buffers = buffers.ToDictionary(b => b.Name, b => b);
            WvpBufferName = wvpBufferName;
            CheckWVPBufferOrThrow(wvpBufferName);
            VertexShaderMethod = vertexShaderMethod;
            ShaderFile = shaderFile;
            PixelShaderProfile = pixelShaderProfile;
        }

        private void CheckWVPBufferOrThrow(string wvpBufferName)
        {
            if (!_buffers.ContainsKey(wvpBufferName))
            {
                throw new ArgumentException($"Buffer list did not contain a wvp buffer with name {wvpBufferName}");
            }
            if (_buffers[wvpBufferName].Type != BufferType.GPUInput)
            {
                throw new ArgumentException($"Wvpbuffer with name {wvpBufferName} has an invalid {nameof(ShaderBufferDescription.Type)}. It should be {BufferType.GPUInput}.");
            }
            if (_buffers[wvpBufferName].BufferUsage != BufferUsage.VertexShader)
            {
                throw new ArgumentException($"Wvpbuffer with name {wvpBufferName} has an invalid {nameof(ShaderBufferDescription.BufferUsage)}. It should be {BufferUsage.VertexShader}.");
            }
        }

        public string PixelShaderMethod { get; }

        private readonly Dictionary<string, ShaderBufferDescription> _buffers;

        public ShaderDescription(Dictionary<string, ShaderBufferDescription> buffers) => _buffers = buffers;

        public IEnumerable<ShaderBufferDescription> Buffers => _buffers.Values;
        public string WvpBufferName { get; }
        public string VertexShaderMethod { get; }
        public string ShaderFile { get; }
        public string PixelShaderProfile { get; }

        public string Signature => Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes($"{ShaderFile}|{VertexShaderMethod}|{VertexShaderProfile}|{PixelShaderMethod}|{PixelShaderProfile}")));

    }
}