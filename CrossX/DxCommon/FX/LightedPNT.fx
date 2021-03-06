// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

#include "Lighted/Common.fx"

struct VS_IN
{
	float4 pos : POSITION;
	float4 norm : NORMAL;
	float2 texCoord : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 orig : POSITION;
	float4 norm : TEXCOORD0;
	float2 texCoord: TEXCOORD1;
};

cbuffer VertexShaderData : register(b0)
{
	float4x4 g_matWorldViewProj;
	float4x4 g_matWorld;
};

Texture2D<float4> colorTexture : register(t0);
sampler colorTextureSampler : register(s0);

Texture2D<float4> specularTexture : register(t1);
sampler specularTextureSampler : register(s1);

PS_IN VS(VS_IN input)
{
  PS_IN output = (PS_IN)0;
  
  output.pos = mul(g_matWorldViewProj, input.pos); // transform Position
  output.orig = mul(g_matWorld, input.pos); // calculate original posEfition
  output.norm = normalize(mul(g_matWorld, input.norm)); // transform Normal and normalize it
  output.texCoord = input.texCoord;
  return output;
}

float4 PS(PS_IN input) : SV_Target
{
	LIGHT_RES light = CalculateLights(input.orig, input.norm);

	float4 specular = specularTexture.Sample(specularTextureSampler, input.texCoord) * light.spec;
	float4 color = colorTexture.Sample(colorTextureSampler, input.texCoord);

	color = color * (g_ambientColor + light.diff);
	color = color + specular;
	
	return color;
}
