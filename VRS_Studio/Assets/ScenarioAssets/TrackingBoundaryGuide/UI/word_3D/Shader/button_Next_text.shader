// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "word_3D_Next"
{
	Properties
	{
		_GraColorA("GraColorA", Color) = (0.338713,0.240032,0.5849056,0)
		_GraColorB("GraColorB", Color) = (0.1802243,0.409049,0.8490566,0)
		_Blur("Blur", Range( 0 , 1)) = 0.5
		_Level("Level", Range( 0 , 1)) = 0.3608115
		[HDR]_WipeColor("WipeColor", Color) = (0.6226415,0.06104949,0.04405482,0)
		_Opacity("Opacity", Range( 0 , 1)) = 1
		_WipeSpeed("WipeSpeed", Range( -2 , 0)) = -1.5
		_InnerGlow_Color("InnerGlow_Color", Color) = (0.05807226,0.6672797,0.8207547,0)
		_InnerGlow_RangeA("InnerGlow_RangeA", Range( 0 , 1)) = 0.58
		_InnerGlow_RangeB("InnerGlow_RangeB", Range( 0 , 5)) = 3.1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			float2 uv2_texcoord2;
		};

		uniform float4 _GraColorB;
		uniform float4 _GraColorA;
		uniform float _Level;
		uniform float _Blur;
		uniform float4 _InnerGlow_Color;
		uniform float _InnerGlow_RangeB;
		uniform float _InnerGlow_RangeA;
		uniform float4 _WipeColor;
		uniform float _WipeSpeed;
		uniform float _Opacity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float clampResult23 = clamp( ( _Level + ( ( _Level - i.uv_texcoord.y ) / _Blur ) ) , 0.0 , 1.0 );
			float4 lerpResult29 = lerp( _GraColorB , _GraColorA , clampResult23);
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV91 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode91 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV91, _InnerGlow_RangeB ) );
			float mulTime253 = _Time.y * _WipeSpeed;
			float clampResult257 = clamp( sin( ( i.uv2_texcoord2.x + mulTime253 ) ) , 0.0 , 1.0 );
			float temp_output_258_0 = (0.0 + (clampResult257 - 0.0) * (1.0 - 0.0) / (1.0 - 0.0));
			o.Emission = ( lerpResult29 + ( _InnerGlow_Color * ( fresnelNode91 * _InnerGlow_RangeA ) ) + ( _WipeColor * temp_output_258_0 ) ).rgb;
			float smoothstepResult289 = smoothstep( 0.2 , 1.0 , i.uv2_texcoord2.x);
			float temp_output_290_0 = step( smoothstepResult289 , 0.0 );
			float lerpResult291 = lerp( temp_output_258_0 , temp_output_290_0 , temp_output_290_0);
			o.Alpha = ( lerpResult291 * _Opacity );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18400
44.8;296.8;1470;664;4225.35;1178.997;5.115571;True;True
Node;AmplifyShaderEditor.RangedFloatNode;251;-1185.751,765.0105;Inherit;False;Property;_WipeSpeed;WipeSpeed;6;0;Create;True;0;0;False;0;False;-1.5;-0.4376542;-2;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;252;-1306.065,525.9411;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-1030.575,-174.6939;Inherit;False;Property;_Level;Level;3;0;Create;True;0;0;False;0;False;0.3608115;0.3608115;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-968.4012,-342.5471;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;253;-1010.315,668.6722;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;254;-866.4712,518.954;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;13;-722.1993,-327.1713;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-861.7682,-74.06496;Inherit;False;Property;_Blur;Blur;2;0;Create;True;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;14;-543.0552,-255.8529;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-1393.816,154.6339;Inherit;False;Property;_InnerGlow_RangeB;InnerGlow_RangeB;9;0;Create;True;0;0;False;0;False;3.1;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;255;-717.9822,511.9659;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;288;-966.0112,892.836;Inherit;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;False;0.2;0.284617;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;257;-518.4092,518.7228;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-400.3243,-229.2288;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;91;-889.6112,230.0054;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;229;-827.0313,81.07486;Inherit;False;Property;_InnerGlow_RangeA;InnerGlow_RangeA;8;0;Create;True;0;0;False;0;False;0.58;0.6653172;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;289;-683.2023,842.2081;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;258;-284.9766,518.251;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;250;-12.27112,90.08896;Inherit;False;Property;_WipeColor;WipeColor;4;1;[HDR];Create;True;0;0;False;0;False;0.6226415,0.06104949,0.04405482,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;93;-481.6382,-82.37646;Inherit;False;Property;_InnerGlow_Color;InnerGlow_Color;7;0;Create;True;0;0;False;0;False;0.05807226,0.6672797,0.8207547,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;22;-478.6372,-670.1132;Inherit;False;Property;_GraColorB;GraColorB;1;0;Create;True;0;0;False;0;False;0.1802243,0.409049,0.8490566,0;0.9224793,0.2801247,0.2656355,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;23;-242.8694,-245.2188;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;290;-496.5572,805.738;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;21;-735.1313,-547.1767;Inherit;False;Property;_GraColorA;GraColorA;0;0;Create;True;0;0;False;0;False;0.338713,0.240032,0.5849056,0;0.9027165,0.4035986,0.1490334,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;228;-517.2982,183.2465;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;225;126.2249,874.4321;Inherit;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;291;68.84242,710.1724;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-158.1193,19.53865;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;29;-62.28259,-518.5487;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;302.3563,116.0849;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;90;556.6742,-35.82587;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;449.3087,684.6274;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;796.7999,14.81753;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;word_3D_Next;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;True;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;253;0;251;0
WireConnection;254;0;252;1
WireConnection;254;1;253;0
WireConnection;13;0;11;0
WireConnection;13;1;10;2
WireConnection;14;0;13;0
WireConnection;14;1;12;0
WireConnection;255;0;254;0
WireConnection;257;0;255;0
WireConnection;18;0;11;0
WireConnection;18;1;14;0
WireConnection;91;3;92;0
WireConnection;289;0;252;1
WireConnection;289;1;288;0
WireConnection;258;0;257;0
WireConnection;23;0;18;0
WireConnection;290;0;289;0
WireConnection;228;0;91;0
WireConnection;228;1;229;0
WireConnection;291;0;258;0
WireConnection;291;1;290;0
WireConnection;291;2;290;0
WireConnection;94;0;93;0
WireConnection;94;1;228;0
WireConnection;29;0;22;0
WireConnection;29;1;21;0
WireConnection;29;2;23;0
WireConnection;259;0;250;0
WireConnection;259;1;258;0
WireConnection;90;0;29;0
WireConnection;90;1;94;0
WireConnection;90;2;259;0
WireConnection;88;0;291;0
WireConnection;88;1;225;0
WireConnection;0;2;90;0
WireConnection;0;9;88;0
ASEEND*/
//CHKSM=861C4BB4F541B994DB2D4CFB59C3DE3B9E157A11