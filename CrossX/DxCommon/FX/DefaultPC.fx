// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

float4x4 worldViewProj;
float4 color;

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;
	float4 pos = input.pos;
	output.pos = mul(worldViewProj, pos);
	output.col = input.col * color;

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	return input.col;
}
