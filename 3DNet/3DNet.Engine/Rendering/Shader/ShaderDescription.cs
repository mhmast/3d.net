using _3DNet.Rendering.Buffer;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace _3DNet.Engine.Rendering.Shader
{
    public class ShaderDescription
    {
        public string VertexShaderProfile { get; }

        public ShaderDescription(string shaderFile, string vertexShaderProfile,  string vertexShaderMethod, string pixelShaderProfile,string pixelShaderMethod)
        {
            VertexShaderProfile = vertexShaderProfile;
            PixelShaderMethod = pixelShaderMethod;
            VertexShaderMethod = vertexShaderMethod;
            ShaderFile = shaderFile;
            PixelShaderProfile = pixelShaderProfile;
        }

        public string PixelShaderMethod { get; }
        public string VertexShaderMethod { get; }
        public string ShaderFile { get;  }
        public string PixelShaderProfile { get;}

        public string Signature => Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes($"{ShaderFile}|{VertexShaderMethod}|{VertexShaderProfile}|{PixelShaderMethod}|{PixelShaderProfile}")));

    }
}