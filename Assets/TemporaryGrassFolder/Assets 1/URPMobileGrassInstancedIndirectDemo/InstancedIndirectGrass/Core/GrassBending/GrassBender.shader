Shader "URP/GrassBender"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BendPosition("Bend Position", Vector) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" "LightMode"="GrassBending" }
        ZTest Always
        ZWrite Off
        Blend One Zero
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            float4 _BendPosition;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 posWS = mul(unity_ObjectToWorld, IN.positionOS).xyz;
                OUT.positionHCS = TransformWorldToHClip(posWS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float dist = distance(float2(IN.positionHCS.x, IN.positionHCS.y), _BendPosition.xy);
                float falloff = smoothstep(1.0, 0.0, dist / _BendPosition.w);
                return half4(0, 0, 0, falloff); // darken bend area
            }
            ENDHLSL
        }
    }
}

