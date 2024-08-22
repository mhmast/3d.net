// cube.hlsl
cbuffer ConstantBuffer : register(b0)
{
    float4x4 WorldViewProjection;
};

struct VS_INPUT
{
    float3 Pos : POSITION;
    float4 Color : COLOR;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float4 Color : COLOR;
};

PS_INPUT VSMain(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 1.0f), WorldViewProjection);
    output.Color = input.Color;
    return output;
}

float4 PSMain(PS_INPUT input) : SV_Target
{
    return input.Color;
}
