Shader "HTC/UnlitColorDetailLightmap"
{
	Properties
	{
		_DetailTex("DetailTex (R)", 2D) = "white" {}
		_DetailStrength("Detail Strength", Range( 0 , 1)) = 0
		[NoScaleOffset]_colorAtlasTex("Color Atlas Texture", 2D) = "white" {}
		_detailTiling("detail tiling", Float) = 1


	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
	LOD 0
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		
		// Non-lightmapped, for proper viewport display
		Pass
		{
			
			Tags { "LightMode"="Vertex" "RenderType"="Opaque" }
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
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 _texcoord0 : TEXCOORD0;
				float4 _texcoord1 : TEXCOORD1;
			};

			uniform sampler2D _DetailTex;
			uniform float4 _DetailTex_ST;
			uniform sampler2D _colorAtlasTex;
			uniform float _DetailStrength;
			uniform float _Float1,_detailTiling;			

			
			v2f vert ( appdata v )
			{
				v2f o;

				float2 uv_base = v._texcoord1.xy * float2( 1,1 ) + float2( 0,0 );

				o._texcoord0.zw = v._texcoord1.xy;
				o._texcoord1.xy = v._texcoord0.xy;
				//o._texcoord1.zw = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{

				float2 uv2_DetailTex = i._texcoord0.xy * _DetailTex_ST.xy + _DetailTex_ST.xy;
				float4 detailTex = lerp( 1, tex2D( _DetailTex, uv2_DetailTex ), _DetailStrength);
				float2 uv_colorTex = i._texcoord1.xy;
				float4 finalColor = ( float4( detailTex.r * tex2D( _colorAtlasTex, uv_colorTex ) ) );

				return finalColor;
			}
			ENDCG
		}		
		

		Pass
		{
			
			Tags { "LightMode"="VertexLM" "RenderType"="Opaque" }
			Name "Unlit LM Mobile"
			CGPROGRAM
			
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 _texcoord1 : TEXCOORD1;
				float4 _texcoord0 : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 _texcoord0 : TEXCOORD0;
				float4 _texcoord1 : TEXCOORD1;
			};

			uniform sampler2D _DetailTex;
			uniform float4 _DetailTex_ST;
			uniform sampler2D _colorAtlasTex;
			uniform float _DetailStrength;
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
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{

				float2 uv_lightmap = i._texcoord0.xy;
				float3 decodeLightMap = DecodeLightmap(UNITY_SAMPLE_TEX2D( unity_Lightmap, uv_lightmap ));
				//float2 uv2_DetailTex = i._texcoord0.xy * _DetailTex_ST.xy + _DetailTex_ST.zw;
				float2 uv2_DetailTex = i._texcoord0.xy * _detailTiling;				
				float4 detailTex = lerp( 1, tex2D( _DetailTex, uv2_DetailTex ), _DetailStrength);
				float2 uv_colorTex = i._texcoord1.xy;
				float4 finalColor = ( float4( decodeLightMap , 0.0 ) * ( detailTex.r * tex2D( _colorAtlasTex, uv_colorTex ) ) );

				return finalColor;
			}
			ENDCG
		}
	}
	
}
