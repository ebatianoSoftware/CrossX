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

LIGHT_RES CalculateLights(float4 pos, float4 normal)
{
	LIGHT_RES dir = CalculateDirLights(pos, normal);
	LIGHT_RES pt = CalculatePointLights(pos, normal);

	LIGHT_RES res = (LIGHT_RES)0;
	res.diff = dir.diff + pt.diff;
	res.spec = dir.spec + pt.spec;
	return res;
}

float4 PS(PS_IN input) : SV_Target
{
	LIGHT_RES light = CalculateLights(input.orig, input.norm);

	float4 specular = specularTexture.Sample(specularTextureSampler, input.texCoord) * light.spec;
	float4 color = g_ambientColor + colorTexture.Sample(colorTextureSampler, input.texCoord) * light.diff;

	return  color + specular;
}
