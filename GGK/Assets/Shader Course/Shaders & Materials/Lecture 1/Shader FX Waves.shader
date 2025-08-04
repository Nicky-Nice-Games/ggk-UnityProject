Shader"Unlit/ScrollingTextureShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorStart ("Color Start", Range(0,1) ) = 0
        _ColorEnd ("Color End", Range(0,1) ) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Pass {
Cull Off

ZWrite Off

Blend One
One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

#define TAU 6.28318530718

sampler2D _MainTex;
float4 _MainTex_ST;

float _ColorStart;
float _ColorEnd;

struct MeshData
{
    float4 vertex : POSITION;
    float3 normals : NORMAL;
    float4 uv0 : TEXCOORD0;
};

struct Interpolators
{
    float4 vertex : SV_POSITION;
    float3 normal : TEXCOORD0;
    float2 uv : TEXCOORD1;
};

Interpolators vert(MeshData v)
{
    Interpolators o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.normal = UnityObjectToWorldNormal(v.normals);
    o.uv = TRANSFORM_TEX(v.uv0, _MainTex); // standard UV transform
    return o;
}

float4 frag(Interpolators i) : SV_Target
{
    float xOffset = cos(i.uv.x * TAU * 8) * 0.01;
    float scrollY = i.uv.y + xOffset - _Time.y * 0.5;

    float t = cos(scrollY * TAU * 5) * 0.5 + 0.5;
    t *= 2 - i.uv.y;

    float topBottomRemover = (abs(i.normal.y) < 0.999);
    float waves = t * topBottomRemover;

    float2 scrolledUV = float2(i.uv.x, scrollY);
    float4 texColor = tex2D(_MainTex, scrolledUV);

    // Preserve RGB, apply effect only to alpha
    return float4(texColor.rgb, texColor.a * waves);
}

            ENDCG
        }
    }
}
