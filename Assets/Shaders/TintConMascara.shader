Shader "Custom/TintSoloAzul"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1,0,0,1)
        _Fuerza ("Fuerza Tint", Range(0,1)) = 0.8
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Fuerza;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float r = col.r;
                float g = col.g;
                float b = col.b;

                // mantener blanco
                if (r > 0.9 && g > 0.9 && b > 0.9)
                    return col;

                // mantener negro
                if (r < 0.1 && g < 0.1 && b < 0.1)
                    return col;

                // SOLO AZULES
                if (b > r && b > g)
                {
                    col.rgb = lerp(col.rgb, _Color.rgb, _Fuerza);
                }

                return col;
            }
            ENDCG
        }
    }
}