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
	float2 tex : TEXCOORD0;
};

float4x4 worldViewProj;
float4 color;
float4 bias;
float4 onePixelSizeAndBlur;

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
	if (onePixelSizeAndBlur.w > 0)
	{
		float4 color = (float4)0;
		float blurSize = onePixelSizeAndBlur.w;

		float weight = 0;

		for (float x = 1-blurSize; x < blurSize; x++)
		{
			for (float y = 1-blurSize; y < blurSize; y++)
			{
				float factor = max(0, blurSize - sqrt(x * x + y * y));
				factor = factor / blurSize;
				factor = factor * factor;

				weight += factor * onePixelSizeAndBlur.z;
				color = color + colorTexture.Sample(colorTextureSampler, input.tex + float2(onePixelSizeAndBlur.x * x, onePixelSizeAndBlur.y * y)) * factor;
			}
		}

		color /= weight;

		color.w = 1;
		return color * input.col + bias;
	}
	else
	{
		return colorTexture.Sample(colorTextureSampler, input.tex) * input.col + bias;
	}
}
