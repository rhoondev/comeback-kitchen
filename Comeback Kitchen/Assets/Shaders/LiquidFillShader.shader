Shader "Custom/LiquidFillShader"
{
    Properties
    {
        _FillAmount("Fill Amount", Range(0, 1)) = 0.5
        _MaxHeight("Max Height", Float) = 1.0
        _Color("Color", Color) = (0, 0, 1, 1) // Blue water
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
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
                float localY : TEXCOORD0;
                float4 color : COLOR;
            };

            float _FillAmount;
            float _MaxHeight;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = _Color;
                o.localY = v.vertex.y;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                clip(_FillAmount * _MaxHeight - i.localY);

                return i.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}