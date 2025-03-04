#ifndef CUSTOM_SEGMENTATION_DEFINED
#define CUSTOM_SEGMENTATION_DEFINED

#include "UnityCG.cginc"

sampler2D _MainTex;
float4 _MainTex_ST;

float4 _SegmentedColor;

struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};

struct v2f
{
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
};

v2f segmentVert(appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	return o;
}

float alphaThreshold = 0.05;

float4 segmentFrag(v2f i, bool useAlpha): SV_TARGET
{
	float4 col = _SegmentedColor;
	if (useAlpha)
	{
		float4 texCol = tex2D(_MainTex, i.uv);
		col.a = texCol.a;
    }
	return col;
}

#endif