Shader "URP/GrassBender"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BendPosition("Bend Position", Vector) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" }
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }

        Pass
        {
            Tags { "LightMode"="GrassBending" }

            ZTest Always
            ZWrite Off
            Blend One Zero
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _BendPosition;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldPos = worldPos;
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 delta = IN.worldPos.xz - _BendPosition.xz;
                float dist = length(delta);
                float falloff = smoothstep(1.0, 0.0, dist / _BendPosition.w);

                return half4(0, 0, 0, falloff); // writes falloff to alpha
            }
            ENDHLSL
        }
    }
}


