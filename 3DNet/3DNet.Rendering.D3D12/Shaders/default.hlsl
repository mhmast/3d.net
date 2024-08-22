
cbuffer globals
{
	matrix world;
	matrix view;
	matrix projection;
};

struct VSInput
{
	float3 position : POSITION;
};

struct PSInput
{
	float4 position : SV_POSITION;
};

struct PSOutput 
{
	float4 color : SV_TARGET;
};

float4 VSMain(VSInput input) : SV_POSITION
{
    float4x4 wvp = mul(mul(world, view), projection);
	return mul(float4(input.position, 1.0), wvp);

}

// Pixel Shader
float4 PSMain(PSInput input) : SV_TARGET
{
    return float4(1.0, 1.0, 1.0, 1.0); // White color
}

// Technique
technique10 Render
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_4_0, VSMain()));
        SetPixelShader(CompileShader(ps_4_0, PSMain()));
    }
}