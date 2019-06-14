Shader "Custom/EnemyPulse"
{
	Properties{
		_Color("Main Color", Color) = (0,0,0,1)
		_PulseColor("Pulse Color", Color) = (1,0,0,1)
		_PulseAmount("Pulse Amount", float) = 0.0
	}
		SubShader{
			Pass {
				Blend SrcAlpha OneMinusSrcAlpha
				ZTest Greater
				Tags { "RenderType" = "Transparent" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				float4 _Color;
				float4 _PulseColor;
				float _PulseAmount;

					struct v2f {
						float4 pos : SV_POSITION;
						float3 worldPos : TEXCOORD1;
					};

					v2f vert(appdata_base v) {
						v2f o;
						o.pos = UnityObjectToClipPos(v.vertex);
						o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
						return o;
					}

					fixed4 frag(v2f i) : COLOR {
						return lerp(_Color, _PulseColor, _PulseAmount);
					}

					ENDCG
				}
			Pass {
				ZTest LEqual
				Tags { "RenderType" = "Opaque" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				float4 _Color;
				float4 _PulseColor;
				float _PulseAmount;

					struct v2f {
						float4 pos : SV_POSITION;
						float3 worldPos : TEXCOORD1;
					};

					v2f vert(appdata_base v) {
						v2f o;
						o.pos = UnityObjectToClipPos(v.vertex);
						o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
						return o;
					}

					fixed4 frag(v2f i) : COLOR {
						return lerp(_Color, _PulseColor, _PulseAmount);
					}

					ENDCG
				}
	}
		FallBack "Diffuse"
}