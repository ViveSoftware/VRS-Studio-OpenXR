// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Bondury_v1"
{
	Properties
	{
		_hitDistance("hitDistance", Range( 0 , 5)) = 0.26
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		[Toggle(_AROUND_FORCEON_ON)] _Around_forceOn("Around_forceOn", Float) = 0
		[HDR]_Boundary_Color("Boundary_Color", Color) = (4.759382,0,0,0)
		_Hand_pos("Hand_pos", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma shader_feature_local _AROUND_FORCEON_ON
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _Boundary_Color;
		uniform float3 _Hand_pos;
		uniform float _Opacity;
		uniform float _hitDistance;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_0 = (1.0).xxxx;
			return temp_cast_0;
		}

		void vertexDataFunc( inout appdata_full v )
		{
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_439_0 = _Boundary_Color;
			o.Albedo = temp_output_439_0.rgb;
			o.Emission = temp_output_439_0.rgb;
			float3 ase_worldPos = i.worldPos;
			float temp_output_415_0 = ( distance( _Hand_pos , ase_worldPos ) * 1.0 );
			float mulTime81 = _Time.y * -0.2;
			float temp_output_1_0_g701 = ( abs( ( temp_output_415_0 + mulTime81 ) ) * 3.0 );
			float smoothstepResult259 = smoothstep( -1.2 , 4.6 , ( ( abs( ( ( temp_output_1_0_g701 - floor( ( temp_output_1_0_g701 + 0.5 ) ) ) * 2 ) ) * 2 ) - 1.0 ));
			float clampResult300 = clamp( smoothstepResult259 , 0.0 , 1.0 );
			float OverallSwipe334 = ( pow( clampResult300 , 3.95 ) * 33.0 );
			float clampResult35 = clamp( ( 1.0 - pow( distance( ase_worldPos , _Hand_pos ) , 40.0 ) ) , 0.0 , 1.0 );
			float clampResult58 = clamp( ( clampResult35 * _Opacity * ( distance( ase_worldPos , _Hand_pos ) <= _hitDistance ? 1.0 : 0.0 ) ) , 0.0 , 1.0 );
			float switchOpacitly417 = clampResult58;
			float3 objToWorld414 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float smoothstepResult447 = smoothstep( 0.0 , 0.5 , ( distance( objToWorld414 , ase_worldPos ) / 5.0 ));
			float temp_output_448_0 = (0.0 + (smoothstepResult447 - 0.0) * (1.0 - 0.0) / (1.0 - 0.0));
			float3 objToWorld474 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			#ifdef _AROUND_FORCEON_ON
				float staticSwitch482 = ( temp_output_448_0 * 20.0 );
			#else
				float staticSwitch482 = ( temp_output_448_0 * 20.0 * ( distance( _Hand_pos , objToWorld474 ) <= _hitDistance ? 1.0 : 0.0 ) );
			#endif
			o.Alpha = ( ( OverallSwipe334 * switchOpacitly417 ) + ( staticSwitch482 * _Opacity ) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"

}
