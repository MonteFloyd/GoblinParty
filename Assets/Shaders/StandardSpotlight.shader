Shader "Unlit/StandardSpotlight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_spotlightRadius("Spotlight Radius", Range(0,30)) = 10
		_spotlightOuter("Spotlight Outer Range", Range(0,30)) = 5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPosition : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			//int arrayLength;
			uniform int _arrayLength;
			uniform float3 _positionArray[255];
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPosition = mul(unity_ObjectToWorld,v.vertex).xyz;

				return o;
			}

			float _spotlightRadius;
			float _spotlightOuter;
			
			
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = fixed4(0.0, 0.0, 0.0, 1.0);
				//fixed black = fixed4(0.0, 0.0, 0.0, 1.0);
				

				for (int k = 0; k < _arrayLength; ++k) 
				{
				float dist = distance(i.worldPosition, _positionArray[k].xyz);
				float blendStrength = dist - _spotlightRadius;

				col = lerp(lerp(tex2D(_MainTex, i.uv), col,  (blendStrength/_spotlightOuter)),col, step(0, dist - _spotlightRadius-_spotlightOuter));
				//col = lerp(lerp(tex2D(_MainTex, i.uv), col, blendStrength / _spotlightOuter), col, step(0, dist - _spotlightRadius));

				}
				return col;
			}
			ENDCG
		}
	}
}
