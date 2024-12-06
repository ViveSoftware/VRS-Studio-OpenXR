Shader "HTC/UnlitColorDetailLightmapSpec"
{
	Properties
	{
		_DetailTex("DetailTex (R)", 2D) = "white" {}
		_DetailStrength("Detail Strength", Range( 0 , 1)) = 0
		[NoScaleOffset]_colorAtlasTex("Color Atlas Texture", 2D) = "white" {}
		_Gloss ("Gloss", Range(2.0, 256.0)) = 32.0
		_SpecColor ("Specular Color", Color) = (1,1,1,1)
		_lightVector("Light Vector", Vector) = (0,0,0,0)
		_detailTiling("detail tiling", Float) = 1		


	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
	LOD 0
		CGINCLUDE
		#pragma target 2.0
		ENDCG
		
		
				// Non-lightmapped
		Pass 
		{
			Tags { "LightMode" = "Vertex" }
			Lighting Off
			SetTexture [_colorAtlasTex] {
			constantColor (1,1,1,1)
			combine texture, constant // UNITY_OPAQUE_ALPHA_FFP
		}
		}
		
		
		
		

		Pass
		{
			
			Tags { "LightMode"="VertexLM" "RenderType"="Opaque" }
			Name "Unlit LM Mobile"
			CGPROGRAM
			
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 _texcoord1 : TEXCOORD1;
				float4 _texcoord0 : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 _texcoord0 : TEXCOORD0;
				float4 _texcoord1 : TEXCOORD1;
				fixed3 normal : NORMAL;
				fixed3 worldPos : TEXCOORD2;
			};

			uniform sampler2D _DetailTex;
			uniform float4 _DetailTex_ST;
			uniform sampler2D _colorAtlasTex;
			uniform float _DetailStrength;
			float _Gloss;
			//fixed3 _LightPos = fixed3(14.10392, 14.82984, -1.66258);
			//fixed3 _LightPos = fixed3(0, 0, 0);
			fixed4 _SpecColor, finalSpec;
			uniform float3 _lightVector;
			uniform float _Float1,_detailTiling;			
			
			v2f vert ( appdata v )
			{
				v2f o;
 
				float2 uv_base = v._texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv_lightmap = ( ( uv_base * (unity_LightmapST).xy ) + (unity_LightmapST).zw );
				o._texcoord0.xy = uv_lightmap;
				
				o._texcoord0.zw = v._texcoord1.xy;
				o._texcoord1.xy = v._texcoord0.xy;
				//o._texcoord1.zw = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex.xyz);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				fixed3 worldNormalDir = normalize(i.normal);
				//fixed3 worldLightDir = _LightPos-i.worldPos;
				fixed3 worldLightDir = _lightVector;				
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));            
				fixed3 halfDir = normalize(worldLightDir + viewDir);
				float specular = pow(saturate(dot(worldNormalDir, halfDir)), _Gloss);
				finalSpec = specular * _SpecColor;
				
				float2 uv_lightmap = i._texcoord0.xy;
				float3 decodeLightMap = DecodeLightmap(UNITY_SAMPLE_TEX2D( unity_Lightmap, uv_lightmap ));
				float2 uv2_DetailTex = i._texcoord0.xy * _detailTiling;
				float4 detailTex = lerp( 1, tex2D( _DetailTex, uv2_DetailTex ), _DetailStrength);
				float2 uv_colorTex = i._texcoord1.xy;
				float4 finalColor = ( float4( decodeLightMap , 0.0 ) * ( detailTex.r * tex2D( _colorAtlasTex, uv_colorTex ) ) );

				return finalColor + finalSpec;
			}
			ENDCG
		}
	}
	
}
 
