Shader "Sprites/Character"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		[MaterialToggle] _Targeted ("Targeted", Float) = 0
		_TargetColor ("Target Color", Color) = (1,1,1,1)
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
				float4 color    : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct FRAG_INFO
			{
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				float2 uv  : TEXCOORD0;
			};
			
			float4 _Color;
			
			float _Targeted;
			float4 _TargetColor;

			FRAG_INFO vert(VERTEX_INFO v)
			{
				FRAG_INFO o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color * _Color;

				return o;
			}

			sampler2D _MainTex;

			float4 frag(FRAG_INFO i) : SV_Target
			{
				float4 c = tex2D(_MainTex, i.uv) * i.color;

				// Some bug with Unity texture sampling
				c *= c.a; 

				float4 targetedCol = (_Targeted * _TargetColor * c.a);

				return float4(c.rgb + targetedCol.rgb, c.a);
			}
			ENDCG
		}
	}
}