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
	float4 orig : POSITION;
	float4 norm : TEXCOORD0;
	float2 texCoord: TEXCOORD1;
};

struct DIR_LIGHT
{
	float4 dir: NORMAL;
	float4 color: COLOR;
};

struct POINT_LIGHT
{
	float4 pos: POSITION;
	float4 color: COLOR;
	float4 att;
};

struct SPOT_LIGHT
{
	POINT_LIGHT def;
	float4 dir;
	float4 cut;
};

cbuffer VertexShaderData : register(b0)
{
	float4x4 g_matWorldViewProj;
	float4x4 g_matWorld;
};

cbuffer PixelShaderData: register(b1)
{
	float4 g_ambientColor;
	float4 g_materialDiffuse;
	DIR_LIGHT g_directionalLights[2];
};

cbuffer PointLights: register(b2)
{
	POINT_LIGHT g_pointLights[8];
}

cbuffer SpotLights : register(b3)
{
	SPOT_LIGHT g_spotLights[8];
}

Texture2D<float4> colorTexture : register(t0);
sampler colorTextureSampler : register(s0);

PS_IN VS(VS_IN input)
{
  PS_IN output = (PS_IN)0;
  
  output.pos = mul(g_matWorldViewProj, input.pos); // transform Position
  output.orig = mul(g_matWorld, input.pos); // calculate original posEfition
  output.norm = normalize(mul(g_matWorld, input.norm)); // transform Normal and normalize it
  output.texCoord = input.texCoord;
  return output;
}

float4 CalculateDirLights(float4 normal)
{
	float4 col = {0,0,0,0};
	for (int i = 0; i < 2; ++i)
	{
		DIR_LIGHT light = g_directionalLights[i];
		col = col + light.color * saturate(dot(light.dir, normal));
	}
	return col;
}

float4 CalculatePointLights(float4 pos, float4 normal)
{
	float4 col = { 0,0,0,0 };
	for (int i = 0; i < 8; ++i)
	{
		POINT_LIGHT light = g_pointLights[i];

		if (light.color.a > 0.1f)
		{
			float4 dir = light.pos - pos;
			float dist = length(dir) * light.att.w;
			dir = normalize(dir);

			col = col + light.color *  
				(saturate(dot(dir, normal)) /
				 (light.att.x + dist * light.att.y + dist * dist * light.att.z));
		}
	}
	return col;
}

float4 CalculateLights(float4 pos, float4 normal)
{
	float4 col = CalculateDirLights(normal) + CalculatePointLights(pos, normal);
	col.a = 1;
	return col;
}

float4 PS(PS_IN input) : SV_Target
{
	float4 color = CalculateLights(input.orig, input.norm);
	return colorTexture.Sample(colorTextureSampler, input.texCoord) * color;
}
