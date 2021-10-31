using System.Security.Cryptography;
using System.Text;

namespace _3DNet.Engine.Rendering.Shader
{
    public class ShaderDescription
    {
        public string VertexShaderProfile { get; set; }
        public string PixelShaderMethod { get; set; }
        public string VertexShaderMethod { get; set; }
        public string ShaderFile { get; set; }
        public string PixelShaderProfile { get; set; }

        public string Signature => Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes($"{ShaderFile}|{VertexShaderMethod}|{VertexShaderProfile}|{PixelShaderMethod}|{PixelShaderProfile}")));
    }
}