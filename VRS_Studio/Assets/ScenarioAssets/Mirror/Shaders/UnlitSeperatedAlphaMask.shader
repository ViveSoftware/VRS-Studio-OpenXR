Shader "Unlit/SeparateAlphaMask" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_AlphaTex("Alpha mask (R)", 2D) = "white" {}
	}

	SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			float4 _MainTex_ST;
			float4 _AlphaTex_ST;

			v2f vert(appdata_t i) {
				v2f o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.texcoord = TRANSFORM_TEX(i.texcoord, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {
				fixed4 col1 = tex2D(_MainTex, i.texcoord);
				fixed4 col2 = tex2D(_AlphaTex, i.texcoord);
				return fixed4(col1.r, col1.g, col1.b, col1.a * col2.r);
			}
			ENDCG
		}
	}	
}