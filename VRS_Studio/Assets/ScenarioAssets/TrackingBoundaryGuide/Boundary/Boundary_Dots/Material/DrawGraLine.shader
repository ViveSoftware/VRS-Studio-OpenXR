// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DrawGraLine"
{
	Properties
	{
		[HDR]_ColorA("ColorA", Color) = (0.1367925,0.7334858,1,0)
		[HDR]_ColorB("ColorB", Color) = (0,0.4748182,0.8490566,0)
		_speed("speed", Range( 0 , 1)) = 0.3
		_StartEnd("StartEnd", Range( 0 , 1)) = 1
		_Opacity("Opacity", Range( 0 , 1)) = 0.8
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _ColorB;
		uniform float4 _ColorA;
		uniform float _speed;
		uniform float _Opacity;
		uniform float _StartEnd;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float mulTime99 = _Time.y * _speed;
			float temp_output_94_0 = ( 0.0 - -0.43 );
			float clampResult92 = clamp( sin( ( ( 6.55 * i.uv_texcoord.y ) + mulTime99 ) ) , temp_output_94_0 , 1.0 );
			float4 lerpResult10 = lerp( _ColorB , _ColorA , (0.0 + (clampResult92 - temp_output_94_0) * (1.0 - 0.0) / (1.0 - temp_output_94_0)));
			o.Emission = lerpResult10.rgb;
			float4 color118 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 color119 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float clampResult127 = clamp( ( _StartEnd + ( ( _StartEnd - i.uv_texcoord.y ) / 0.1345823 ) ) , 0.0 , 1.0 );
			float4 lerpResult120 = lerp( color118 , color119 , clampResult127);
			o.Alpha = ( _Opacity * lerpResult120 ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
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
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
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
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18400
166;332;1189;687;1279.208;16.67784;1.597555;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;101;-1496.315,31.1576;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;102;-1433.426,216.3333;Inherit;False;Property;_speed;speed;3;0;Create;True;0;0;False;0;False;0.3;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-1429.452,-70.75766;Inherit;False;Constant;_Float2;Float 2;3;0;Create;True;0;0;False;0;False;6.55;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-762.1778,822.153;Inherit;False;Property;_StartEnd;StartEnd;4;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;121;-908.0748,664.9322;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;99;-1241.262,191.8761;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-1234.276,31.15762;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;123;-660.0748,687.4321;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-663.8665,988.0552;Inherit;False;Constant;_blur1;blur;8;0;Create;True;0;0;False;0;False;0.1345823;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-994.9438,211.0924;Inherit;False;Constant;_Float1;Float 1;3;0;Create;True;0;0;False;0;False;-0.43;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;97;-1063.076,24.16984;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;125;-489.4507,891.807;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;94;-746.8783,142.9616;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;95;-914.5862,17.18206;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;126;-341.3258,799.932;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;119;-371.6908,364.6742;Inherit;False;Constant;_ColorA1;ColorA;10;0;Create;True;0;0;False;0;False;1,1,1,0;0.7578583,0.7578583,0.7578583,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;118;-380.8778,560.3701;Inherit;False;Constant;_ColorB1;ColorB;11;0;Create;True;0;0;False;0;False;0,0,0,0;0.5575534,0.5823337,0.5916263,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;127;-189.9289,738.5221;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;92;-628.0875,-24.74449;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;129;-68.26149,205.3823;Inherit;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;False;0.8;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;8;-870.4684,-417.0685;Inherit;False;Property;_ColorA;ColorA;1;1;[HDR];Create;True;0;0;False;0;False;0.1367925,0.7334858,1,0;0.1058824,0.7333333,0.9019608,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;9;-879.6556,-221.3728;Inherit;False;Property;_ColorB;ColorB;2;1;[HDR];Create;True;0;0;False;0;False;0,0.4748182,0.8490566,0;0.2784314,0.2352941,0.7686275,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;120;-106.5787,497.9433;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;93;-425.4424,116.7576;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;-183.3093,-137.4993;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;214.5057,280.4675;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;476.5148,-179.5851;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;DrawGraLine;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;True;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;99;0;102;0
WireConnection;98;0;100;0
WireConnection;98;1;101;2
WireConnection;123;0;122;0
WireConnection;123;1;121;2
WireConnection;97;0;98;0
WireConnection;97;1;99;0
WireConnection;125;0;123;0
WireConnection;125;1;124;0
WireConnection;94;1;96;0
WireConnection;95;0;97;0
WireConnection;126;0;122;0
WireConnection;126;1;125;0
WireConnection;127;0;126;0
WireConnection;92;0;95;0
WireConnection;92;1;94;0
WireConnection;120;0;118;0
WireConnection;120;1;119;0
WireConnection;120;2;127;0
WireConnection;93;0;92;0
WireConnection;93;1;94;0
WireConnection;10;0;9;0
WireConnection;10;1;8;0
WireConnection;10;2;93;0
WireConnection;128;0;129;0
WireConnection;128;1;120;0
WireConnection;0;2;10;0
WireConnection;0;9;128;0
ASEEND*/
//CHKSM=839B6B2295685A9295C8688B8CB61722E4203004