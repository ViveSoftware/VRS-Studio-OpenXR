// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "hand_blueGrafillOL2022"
{
	Properties
	{
		[HDR]_GraColorA("GraColorA", Color) = (0.1058824,0.6901961,0.9019608,0)
		[HDR]_GraColorB("GraColorB", Color) = (1,1,1,0)
		_Gradient_Blur("Gradient_Blur", Range( 0 , 1)) = 0.2854134
		_Gradient_level("Gradient_level", Range( 0 , 1)) = 0.5225084
		_Opacity("Opacity", Range( 0 , 1)) = 0.45
		_line_opacity("line_opacity", Range( 0 , 1)) = 0.5
		_OutlineThickness("OutlineThickness", Range( 0 , 0.002)) = 0.001
		_AlphaText("AlphaText", 2D) = "white" {}
		[HideInInspector] _texcoord3( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
			float2 uv3_texcoord3;
			float2 uv_texcoord;
		};
		uniform float _OutlineThickness;
		uniform float4 _GraColorA;
		uniform float4 _GraColorB;
		uniform float _Gradient_level;
		uniform float _Gradient_Blur;
		uniform float _line_opacity;
		uniform sampler2D _AlphaText;
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = _OutlineThickness;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float clampResult114 = clamp( ( _Gradient_level + ( ( _Gradient_level - i.uv3_texcoord3.y ) / _Gradient_Blur ) ) , 0.0 , 1.0 );
			float4 lerpResult100 = lerp( _GraColorA , _GraColorB , clampResult114);
			float4 tex2DNode92 = tex2D( _AlphaText, i.uv_texcoord );
			o.Emission = lerpResult100.rgb;
			o.Alpha = ( _line_opacity * tex2DNode92 ).r;
		}
		ENDCG
		

		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv3_texcoord3;
			float2 uv_texcoord;
		};

		uniform float4 _GraColorA;
		uniform float4 _GraColorB;
		uniform float _Gradient_level;
		uniform float _Gradient_Blur;
		uniform float _Opacity;
		uniform sampler2D _AlphaText;

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
			float clampResult114 = clamp( ( _Gradient_level + ( ( _Gradient_level - i.uv3_texcoord3.y ) / _Gradient_Blur ) ) , 0.0 , 1.0 );
			float4 lerpResult100 = lerp( _GraColorA , _GraColorB , clampResult114);
			o.Emission = lerpResult100.rgb;
			float4 color104 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 color102 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float smoothstepResult103 = smoothstep( -0.05 , 1.0 , i.uv3_texcoord3.y);
			float4 lerpResult105 = lerp( color104 , color102 , smoothstepResult103);
			float4 tex2DNode92 = tex2D( _AlphaText, i.uv_texcoord );
			o.Alpha = ( lerpResult105 * _Opacity * tex2DNode92 ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18400
-44;191;1685;789;3167.946;78.14783;1.866454;True;True
Node;AmplifyShaderEditor.TextureCoordinatesNode;101;-2095.378,78.48389;Inherit;False;2;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;109;-2081.555,596.8061;Inherit;False;Property;_Gradient_level;Gradient_level;4;0;Create;True;0;0;False;0;False;0.5225084;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;110;-1696.416,569.4415;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-2019.447,735.7135;Inherit;False;Property;_Gradient_Blur;Gradient_Blur;3;0;Create;True;0;0;False;0;False;0.2854134;0.2854134;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;112;-1512.071,553.6597;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;113;-1579.992,722.2491;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;-1753.314,-34.73285;Inherit;False;Constant;_Float1;Float 1;7;0;Create;True;0;0;False;0;False;-0.05;0.6287292;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;93;-1155.808,252.2;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;-1712.319,-143.0189;Inherit;False;Constant;_Float0;Float 0;10;0;Create;True;0;0;False;0;False;1;0.374157;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;97;-1560.862,170.8781;Inherit;False;Property;_GraColorA;GraColorA;1;1;[HDR];Create;True;0;0;False;0;False;0.1058824,0.6901961,0.9019608,0;0.1134819,0.7397338,0.9666976,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;114;-1370.761,698.1024;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;102;-1500.754,-363.8611;Inherit;False;Constant;_Color1;Color 1;2;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;92;-930.0894,220.2226;Inherit;True;Property;_AlphaText;AlphaText;8;0;Create;True;0;0;False;0;False;-1;e7af263ffe482d54d81bc0a535a37294;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;98;-1555.662,358.5779;Inherit;False;Property;_GraColorB;GraColorB;2;1;[HDR];Create;True;0;0;False;0;False;1,1,1,0;1.184883,1.123403,1.123403,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;65;-957.0479,639.6686;Inherit;False;Property;_line_opacity;line_opacity;6;0;Create;True;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;104;-1505.953,-551.561;Inherit;False;Constant;_Color0;Color 0;1;0;Create;True;0;0;False;0;False;0,0,0,0;0.1058824,0.6901961,0.9019608,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;103;-1397.039,-174.0364;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;100;-1200.868,428.7743;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-498.8477,428.1474;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;105;-1071.885,-309.6809;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-720.4553,553.3095;Inherit;False;Property;_OutlineThickness;OutlineThickness;7;0;Create;True;0;0;False;0;False;0.001;0.001;0;0.002;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-1146.433,103.9943;Inherit;False;Property;_Opacity;Opacity;5;0;Create;True;0;0;False;0;False;0.45;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;70;-262.3509,294.5433;Inherit;False;0;True;Transparent;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-647.8427,-101.2824;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;hand_blueGrafillOL2022;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;True;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;110;0;109;0
WireConnection;110;1;101;2
WireConnection;112;0;110;0
WireConnection;112;1;111;0
WireConnection;113;0;109;0
WireConnection;113;1;112;0
WireConnection;114;0;113;0
WireConnection;92;1;93;0
WireConnection;103;0;101;2
WireConnection;103;1;108;0
WireConnection;103;2;107;0
WireConnection;100;0;97;0
WireConnection;100;1;98;0
WireConnection;100;2;114;0
WireConnection;84;0;65;0
WireConnection;84;1;92;0
WireConnection;105;0;104;0
WireConnection;105;1;102;0
WireConnection;105;2;103;0
WireConnection;70;0;100;0
WireConnection;70;2;84;0
WireConnection;70;1;64;0
WireConnection;71;0;105;0
WireConnection;71;1;63;0
WireConnection;71;2;92;0
WireConnection;0;2;100;0
WireConnection;0;9;71;0
WireConnection;0;11;70;0
ASEEND*/
//CHKSM=571EBA847279C0A33BE4D42A88F8315F6F4E6B55