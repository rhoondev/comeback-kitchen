Shader "Custom/LiquidFillShader"
{
    Properties
    {
        _FillHeight("Fill Height", Range(0, 1)) = 0.5
        _CupHeight("Cup Height", Float) = 1.0
        _CupBaseRadius("Cup Base Radius", Float) = 0.5
        _CupTopRadius("Cup Top Radius", Float) = 0.0
        _Color("Color", Color) = (0, 0, 1, 1) // Blue water
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull front 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float worldY : TEXCOORD0;
                float4 color : COLOR;
            };

            float _FillHeight;
            float _CupHeight;
            float _CupBaseRadius;
            float _CupTopRadius;
            float4 _Color;

            v2f vert(appdata v)
            {
                float fillLevel = _FillHeight * _CupHeight;
                
                v.vertex.y = min(v.vertex.y, fillLevel);

                if (v.vertex.y > 0.01 || (fillLevel <= 0.01 && v.vertex.y == fillLevel)) {
                    float r = lerp(_CupBaseRadius, _CupTopRadius, v.vertex.y / _CupHeight);
                    v.vertex.xz *= r / length(v.vertex.xz);
                }
                
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = _Color;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Clip all pixels if height is 0
                if (_FillHeight == 0) {
                    clip(-1);
                }

                return i.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}