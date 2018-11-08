Shader "Personal/ColourSwap"
{
	// Unity Properties
	Properties
	{
		_MainTex ("Greyscale Texture", 2D) = "white" {}
		_Cutoff("Colour Cutoff", Range(0, 1)) = 0.5
		_Colour1("Colour1", Color) = (1,1,1,1)
		_Colour2("Colour2", Color) = (1,1,1,1)
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always
			// Alpha Blending
			Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			// Input
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 colour : COLOR;
			};

			// Vert to Frag
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 colour : COLOR;
			};

			// Vertex
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.colour = v.colour;
				return o;
			}
			
			// Texture
			sampler2D _MainTex;
			// Greyscale Cutoff 
			fixed _Cutoff;
			// Tint Colour 1
			fixed4 _Colour1;
			// Tint Colour 2
			fixed4 _Colour2;

			// Frag
			fixed4 frag (v2f i) : SV_Target
			{
				// Colour Value
				fixed4 col = tex2D(_MainTex, i.uv) * i.colour;
				// Resulting Colour Value
				fixed4 result;

				// If red is over cutoff, change to colour 2
				if (col.r <= _Cutoff)
				{
					result = col * _Colour1;
				}
				else
				{
					result = col * _Colour2;
				}

				return result;
			}
			ENDCG
		}
	}
}
