// MIT License - Copyright © ebatianoSoftware
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

struct VS_IN
{
	float4 pos : POSITION;
	float4 norm : NORMAL;
	float2 texCoord : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 color: COLOR;
	float4 light : TEXCOORD0;
	float4 norm : TEXCOORD1;
	float2 texCoord: TEXCOORD2;
};

float4x4 matWorldViewProj;
float4x4 matWorld;
float4 vecLightDir;
float4 color;

Texture2D<float4> colorTexture : register(t0);
sampler colorTextureSampler : register(s0);


PS_IN VS(VS_IN input)
{
  PS_IN output = (PS_IN)0;
  
  output.pos = mul(matWorldViewProj, input.pos); // transform Position
  output.light = vecLightDir; // output light vector
  output.norm = normalize(mul(matWorld, input.norm)); // transform Normal and normalize it
  output.texCoord = input.texCoord;
  output.color = color;
  
  return output;
}

float4 PS(PS_IN input): SV_Target
{
  float4 diffuse = { 1.0f, 1.0f, 1.0f, 1.0f};
  float4 ambient = { 0.3f, 0.3f, 0.3f, 1.0f };
  float4 color = ambient + diffuse * saturate(dot(input.light, input.norm));
  
  return colorTexture.Sample(colorTextureSampler, input.texCoord) * color * input.color;
}


