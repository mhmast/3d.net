namespace _3DNet.Rendering.D3D12
{
    internal interface ID3DObject
    {
        void Begin(D3DRenderWindowContext context);
        void End(D3DRenderWindowContext context);
    }
}
