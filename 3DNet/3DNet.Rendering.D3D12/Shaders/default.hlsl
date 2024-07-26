#define D3DCOMPILE_DEBUG 1
cbuffer globals : register(b0)
{
	matrix wvp;
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

	return mul(float4(input.position, 1), wvp);

}

float4 PSMain(float4 input : SV_POSITION) : SV_TARGET
{
	//PSOutput output;
	return float4(125,125,125,125);
	//return output;
}