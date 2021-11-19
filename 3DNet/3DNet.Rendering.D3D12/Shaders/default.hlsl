cbuffer globals : register(b0)
{
	matrix wvp;
};

struct PSInput
{
	float4 position : SV_POSITION;
};

PSInput VSMain(float4 position : SV_POSITION)
{
	PSInput result;

	result.position = mul(position,wvp);

	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
	return float4(255,255,255,255);
}