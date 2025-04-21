Shader "Custom/Placement Zone"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (1, 1, 0, 1)
        _Threshold ("Edge Angle Threshold", Range(0, 180)) = 30
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Name "EdgeDetect"
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma target 4.6
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2g
            {
                float4 clipPos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            struct g2f
            {
                float4 clipPos : SV_POSITION;
                float4 color : COLOR;
            };

            float4 _EdgeColor;
            float _Threshold;

            v2g vert(appdata v)
            {
                v2g o;
                float3 world = TransformObjectToWorld(v.vertex);
                o.worldPos = world.xyz;
                o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                o.clipPos = TransformWorldToHClip(o.worldPos);
                return o;
            }

            [maxvertexcount(6)]
            void geom(triangle v2g input[3], inout LineStream<g2f> lines)
            {
                float3 faceNormal = normalize(cross(input[1].worldPos - input[0].worldPos,
                                                    input[2].worldPos - input[0].worldPos));

                for (int i = 0; i < 3; i++)
                {
                    int j = (i + 1) % 3;
                    int k = (i + 2) % 3;

                    float3 edgeVec = input[j].worldPos - input[i].worldPos;
                    float3 otherNormal = normalize(cross(edgeVec, input[k].worldPos - input[i].worldPos));
                    float angle = degrees(acos(saturate(dot(faceNormal, otherNormal))));

                    if (angle > _Threshold)
                    {
                        g2f a, b;
                        a.clipPos = input[i].clipPos;
                        b.clipPos = input[j].clipPos;
                        a.color = _EdgeColor;
                        b.color = _EdgeColor;
                        lines.Append(a);
                        lines.Append(b);
                    }
                }
            }

            half4 frag(g2f i) : SV_Target
            {
                return i.color;
            }

            ENDHLSL
        }
    }
}
