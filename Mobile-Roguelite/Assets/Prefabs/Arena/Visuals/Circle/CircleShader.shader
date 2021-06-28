Shader "Sprites/CircleShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Distance ("Distance", Float) = 0.5
        _Width ("Width", Float) = 0.1

        _Pixelize ("Pixelize", Float) = 5
        _InnerColor ("Inner Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct VERTEX_INFO
            {
                float4 vertex   : POSITION;
                float4 uv       : TEXCOORD0;
            };

            struct FRAG_INFO
            {
                float4 vertex   : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Distance;
            float _Width;
            float _Pixelize;

            float4 _InnerColor;

            FRAG_INFO vert(VERTEX_INFO v)
            {
                FRAG_INFO o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            //sampler2D _MainTex;

            fixed4 frag(FRAG_INFO i) : SV_Target
            {
                // Pixelize
                float2 coords = float2(abs(0.5 - i.uv.x), abs(0.5 - i.uv.y));
                float2 pix = floor(coords * _Pixelize) / _Pixelize;

                float d = sqrt(pow(abs(pix.x), 2) + pow(abs(pix.y), 2));

                // Cirle
                float outer = step(_Distance, d + _Width * 0.5);
                float inner = 1 - step(_Distance, d - _Width * 0.5);

                float t = outer * inner;
                
                float4 col = t + inner * _InnerColor;

                return float4(col.xyz, step(1, t) + col.w);
            }

            ENDCG
        }
    }
}