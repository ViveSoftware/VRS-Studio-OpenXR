// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Unlit/Bondury_v2"
{
	Properties
	{
		_hitDistance("hitDistance", Range( 0 , 5)) = 0.26
		_Opacity("Opacity", Range( 0 , 1)) = 0.5
		[Toggle(_AROUND_FORCEON_ON)] _Around_forceOn("Around_forceOn", Float) = 0
		[HDR]_Boundary_Color("Boundary_Color", Color) = (0.9950371,0.09950373,0,0)
		_Hand_pos("Hand_pos", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _AROUND_FORCEON_ON
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
		};

		uniform float4 _Boundary_Color;
		uniform float3 _Hand_pos;
		uniform float _Opacity;
		uniform float _hitDistance;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_output_58_0 = _Boundary_Color;
			o.Albedo = temp_output_58_0.rgb;
			o.Emission = temp_output_58_0.rgb;
			float3 ase_worldPos = i.worldPos;
			float temp_output_6_0 = ( distance( _Hand_pos , ase_worldPos ) * 1.0 );
			float temp_output_1_0_g701 = ( abs( ( temp_output_6_0 + 0.0 ) ) * 3.0 );
			float smoothstepResult23 = smoothstep( -1.2 , 4.6 , ( ( abs( ( ( temp_output_1_0_g701 - floor( ( temp_output_1_0_g701 + 0.5 ) ) ) * 2 ) ) * 2 ) - 1.0 ));
			float clampResult28 = clamp( smoothstepResult23 , 0.0 , 1.0 );
			float OverallSwipe45 = ( pow( clampResult28 , 3.95 ) * 33.0 );
			float clampResult30 = clamp( ( 1.0 - pow( distance( ase_worldPos , _Hand_pos ) , 40.0 ) ) , 0.0 , 1.0 );
			float temp_output_81_0 = distance( _WorldSpaceCameraPos , _Hand_pos );
			float clampResult38 = clamp( ( clampResult30 * _Opacity * ( temp_output_81_0 <= _hitDistance ? 1.0 : 0.0 ) * 20.0 ) , 0.0 , 1.0 );
			float switchOpacitly44 = clampResult38;
			float3 objToWorld17 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			float smoothstepResult35 = smoothstep( 0.0 , 0.5 , ( distance( objToWorld17 , ase_worldPos ) / 5.0 ));
			float temp_output_39_0 = (0.0 + (smoothstepResult35 - 0.0) * (1.0 - 0.0) / (1.0 - 0.0));
			#ifdef _AROUND_FORCEON_ON
				float staticSwitch47 = ( temp_output_39_0 * 20.0 );
			#else
				float staticSwitch47 = 0.0;
			#endif
			o.Alpha = ( ( OverallSwipe45 * switchOpacitly44 ) + ( staticSwitch47 * _Opacity ) );
		}

		ENDCG
	}
	
}
