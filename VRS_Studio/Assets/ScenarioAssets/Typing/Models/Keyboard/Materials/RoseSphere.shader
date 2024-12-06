// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Rose_Sphere"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (0.06207724,0.5146144,0.8773585,0)
		_Radius("Radius", Range( 0 , 1)) = 0.9096602
		_Opacity("Opacity", Range( 0 , 1)) = 1
		[HDR]_Outline_Color("Outline_Color", Color) = (0.1058824,0.6901961,0.9019608,0)
		_line_opacity("line_opacity", Range( 0 , 1)) = 0.5
		_OutlineThickness("OutlineThickness", Range( 0 , 1)) = 0.001
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0"}
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog alpha:fade  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		
		struct Input
		{
			half filler;
		};
		uniform float _OutlineThickness;
		uniform float4 _Outline_Color;
		uniform float _line_opacity;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = _OutlineThickness;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _Outline_Color.rgb;
			o.Alpha = _line_opacity;
		}
		ENDCG
		

		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float _Radius;
		uniform float4 _Color;
		uniform float _Opacity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV116 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode116 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV116, _Radius ) );
			o.Emission = ( fresnelNode116 * _Color ).rgb;
			o.Alpha = ( fresnelNode116 * _Opacity );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18400
137;298;1548;588;1600.208;124.1547;1.493676;True;True
Node;AmplifyShaderEditor.RangedFloatNode;119;-1133.39,-58.04735;Inherit;False;Property;_Radius;Radius;1;0;Create;True;0;0;False;0;False;0.9096602;0.9096602;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;116;-830.1518,-146.8491;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-827.734,114.8409;Inherit;False;Property;_Opacity;Opacity;2;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-795.5959,460.015;Inherit;False;Property;_line_opacity;line_opacity;4;0;Create;True;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;97;-631.1859,265.743;Inherit;False;Property;_Outline_Color;Outline_Color;3;1;[HDR];Create;True;0;0;False;0;False;0.1058824,0.6901961,0.9019608,0;0.1134819,0.7397338,0.9666976,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;118;-509.7217,-366.2675;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;False;0.06207724,0.5146144,0.8773585,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;64;-733.8884,557.9638;Inherit;False;Property;_OutlineThickness;OutlineThickness;5;0;Create;True;0;0;False;0;False;0.001;0.001;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-319.8751,59.89663;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;117;-296.4584,-153.2618;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;70;-262.3509,294.5433;Inherit;False;0;True;Transparent;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Rose_Sphere;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;True;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;116;3;119;0
WireConnection;120;0;116;0
WireConnection;120;1;115;0
WireConnection;117;0;116;0
WireConnection;117;1;118;0
WireConnection;70;0;97;0
WireConnection;70;2;65;0
WireConnection;70;1;64;0
WireConnection;0;2;117;0
WireConnection;0;9;120;0
WireConnection;0;11;70;0
ASEEND*/
//CHKSM=7C425942CCF9012C0DF7D5868BF42ABBBEC3A082