
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

struct LIGHT_RES
{
	float4 diff;
	float4 spec;
};

cbuffer PixelShaderData: register(b1)
{
	float4 g_ambientColor;
	float4 g_materialDiffuse;
	float4 g_cameraPosition;
	float4 g_specular;
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

float4 CalculateSpecular(float4 pos, float4 normal, float4 dir)
{
	float4 vertexToEye = normalize(g_cameraPosition - pos);
	float4 lightReflect = normalize(reflect(-dir, normal));
	float specularFactor = dot(vertexToEye, lightReflect);

	if (specularFactor > 0)
	{
		specularFactor = pow(specularFactor, g_specular.w);
		return float4(g_specular.xyz * specularFactor, 0);
	}
	return 0;
}

LIGHT_RES CalculateDirLights(float4 pos, float4 normal)
{
	LIGHT_RES res = (LIGHT_RES)0;

	for (int i = 0; i < 2; ++i)
	{
		DIR_LIGHT light = g_directionalLights[i];
		if (light.color.a > 0.1f)
		{
			float saturation = saturate(dot(light.dir, normal));
			res.diff = res.diff + light.color * g_materialDiffuse * saturation;
			res.spec = res.spec + light.color * CalculateSpecular(pos, normal, light.dir) * saturation;
		}
	}
	return res;
}

LIGHT_RES CalculatePointLights(float4 pos, float4 normal)
{
	LIGHT_RES res = (LIGHT_RES)0;

	for (int i = 0; i < 8; ++i)
	{
		POINT_LIGHT light = g_pointLights[i];

		if (light.color.a > 0.1f)
		{
			float4 dir = light.pos - pos;
			float dist = length(dir) * light.att.w;
			dir = normalize(dir);
			
			float distAtt = 1.0f / (light.att.x + dist * light.att.y + dist * dist * light.att.z);
			float saturation = saturate(dot(dir, normal)) * distAtt;

			res.diff = res.diff + light.color * g_materialDiffuse * saturation;
			res.spec = res.spec + light.color * CalculateSpecular(pos, normal, dir) * saturation;
		}
	}
	return res;
}