// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD;
};

float4x4 worldViewProj;
float4 color;

Texture2D<float4> colorTexture : register(t0);
sampler colorTextureSampler : register(s0);

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(worldViewProj, input.pos);
	output.col = input.col * color;
	output.tex = input.tex;

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	return colorTexture.Sample(colorTextureSampler, input.tex) * input.col;
}
