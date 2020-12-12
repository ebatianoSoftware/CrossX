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
	float4 color : COLOR;
	float4 spec : COLOR1;
	float2 tex : TEXCOORD;
};

cbuffer VertexShaderData : register(b3)
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

  float4 orig = mul(g_matWorld, input.pos); // calculate original posEfition
  float4 norm = normalize(mul(g_matWorld, input.norm)); // transform Normal and normalize it

  LIGHT_RES light = CalculateLights(orig, norm);

  output.color = (g_ambientColor + light.diff);
  output.spec = light.spec;
  output.tex = input.texCoord;

  return output;
}

float4 PS(PS_IN input) : SV_Target
{
	float4 specular = specularTexture.Sample(specularTextureSampler, input.tex) * input.spec;
	float4 color = colorTexture.Sample(colorTextureSampler, input.tex) * input.color;
	
	color = color + specular;
	return color;
}
