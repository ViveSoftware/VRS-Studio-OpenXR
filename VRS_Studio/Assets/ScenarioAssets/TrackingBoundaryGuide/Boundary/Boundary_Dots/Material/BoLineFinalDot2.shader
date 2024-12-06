// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BoLineFinalDot2"
{
	Properties
	{
		_Wave("Wave", 2D) = "white" {}
		_DotsA("DotsA", 2D) = "white" {}
		[HDR]_wavecolor("wavecolor", Color) = (0.06666667,0.6,0.945098,0)
		[HDR]_wavecolor2("wavecolor2", Color) = (0.1764706,0.5568628,0.9882353,0)
		[HDR]_lineColor("lineColor", Color) = (0.7450981,0.2784314,0.08627451,0)
		[HDR]_gridcolor("gridcolor", Color) = (0.5843138,0.007843138,0.007843138,0)
		_speedA("speedA", Vector) = (0,-0.03,0,0)
		_speedB("speedB", Vector) = (0,-0.01,0,0)
		_speedC("speedC", Vector) = (0,-0.02,0,0)
		_waveOpacity("waveOpacity", Range( 0 , 1)) = 0.8
		_waverate("waverate", Range( 0 , 1)) = 0.1
		_StatusChange("StatusChange", Range( 0 , 1)) = 0.01
		_startend("startend", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		ZWrite Off
		Blend One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
		#endif//ASE Sampling Macros

		#pragma surface surf Unlit keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _wavecolor;
		uniform float4 _wavecolor2;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_DotsA);
		uniform float2 _speedA;
		SamplerState sampler_DotsA;
		uniform float2 _speedB;
		uniform float2 _speedC;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Wave);
		SamplerState sampler_Wave;
		uniform float4 _lineColor;
		uniform float4 _gridcolor;
		uniform float _StatusChange;
		uniform float _waveOpacity;
		uniform float _waverate;
		uniform float _startend;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 color343 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 lerpResult204 = lerp( _wavecolor , _wavecolor2 , ( sin( _Time.y ) * 2 ));
			float2 uv_TexCoord526 = i.uv_texcoord * float2( 1,1 );
			float2 panner536 = ( _Time.y * _speedA + uv_TexCoord526);
			float cos551 = cos( (float)radians( 90 ) );
			float sin551 = sin( (float)radians( 90 ) );
			float2 rotator551 = mul( panner536 - float2( 0.5,0.5 ) , float2x2( cos551 , -sin551 , sin551 , cos551 )) + float2( 0.5,0.5 );
			float2 uv_TexCoord534 = i.uv_texcoord * float2( 1,1 );
			float2 panner537 = ( _Time.y * _speedB + uv_TexCoord534);
			float cos549 = cos( (float)radians( 90 ) );
			float sin549 = sin( (float)radians( 90 ) );
			float2 rotator549 = mul( panner537 - float2( 0.5,0.5 ) , float2x2( cos549 , -sin549 , sin549 , cos549 )) + float2( 0.5,0.5 );
			float2 uv_TexCoord531 = i.uv_texcoord * float2( 1,1 );
			float2 panner541 = ( _Time.y * _speedC + uv_TexCoord531);
			float cos545 = cos( (float)radians( 90 ) );
			float sin545 = sin( (float)radians( 90 ) );
			float2 rotator545 = mul( panner541 - float2( 0.5,0.5 ) , float2x2( cos545 , -sin545 , sin545 , cos545 )) + float2( 0.5,0.5 );
			float2 panner589 = ( _Time.y * float2( 0.05,0 ) + i.uv_texcoord);
			float smoothstepResult576 = smoothstep( 0.98 , 0.83 , i.uv_texcoord.y);
			float dotline556 = ( ( ( SAMPLE_TEXTURE2D( _DotsA, sampler_DotsA, rotator551 ).b + SAMPLE_TEXTURE2D( _DotsA, sampler_DotsA, rotator549 ).g + SAMPLE_TEXTURE2D( _DotsA, sampler_DotsA, rotator545 ).r ) * SAMPLE_TEXTURE2D( _Wave, sampler_Wave, panner589 ).b ) * smoothstepResult576 );
			float4 temp_cast_3 = (dotline556).xxxx;
			float2 uv_TexCoord458 = i.uv_texcoord * float2( 4,1 );
			float2 panner455 = ( 1.0 * _Time.y * float2( 0,0 ) + uv_TexCoord458);
			float BlockLine400 = SAMPLE_TEXTURE2D( _Wave, sampler_Wave, panner455 ).r;
			float smoothstepResult516 = smoothstep( 0.99 , 0.87 , i.uv_texcoord.y);
			float2 uv_TexCoord425 = i.uv_texcoord * float2( 30,30 );
			float2 appendResult10_g21 = (float2(0.85 , 0.85));
			float2 temp_output_11_0_g21 = ( abs( (frac( uv_TexCoord425 )*2.0 + -1.0) ) - appendResult10_g21 );
			float2 break16_g21 = ( 1.0 - ( temp_output_11_0_g21 / fwidth( temp_output_11_0_g21 ) ) );
			float mulTime464 = _Time.y * -0.4376542;
			float temp_output_468_0 = ( 0.0 - -0.54 );
			float clampResult469 = clamp( sin( ( ( 6.55 * i.uv_texcoord.y ) + mulTime464 ) ) , temp_output_468_0 , 1.0 );
			float Grid449 = ( 0.4722091 * ( 1.0 - saturate( min( break16_g21.x , break16_g21.y ) ) ) * (0.0 + (clampResult469 - temp_output_468_0) * (1.0 - 0.0) / (1.0 - temp_output_468_0)) );
			float clampResult285 = clamp( ( _StatusChange + ( ( _StatusChange - i.uv_texcoord.y ) / 0.8932782 ) ) , 0.0 , 1.0 );
			float4 lerpResult288 = lerp( temp_cast_3 , ( ( _lineColor * ( 2.0 * ( BlockLine400 * ( 1.0 - ( _SinTime.w * 0.18 ) ) * smoothstepResult516 ) ) ) + ( _gridcolor * Grid449 ) ) , clampResult285);
			float2 uv_TexCoord174 = i.uv_texcoord * float2( 1.2,1 );
			float2 panner177 = ( 1.0 * _Time.y * float2( 0,0 ) + uv_TexCoord174);
			float2 temp_cast_4 = (( _waverate * _SinTime.w )).xx;
			float2 uv_TexCoord190 = i.uv_texcoord * float2( 21,0 ) + temp_cast_4;
			float simplePerlin2D183 = snoise( uv_TexCoord190 );
			simplePerlin2D183 = simplePerlin2D183*0.5 + 0.5;
			float4 temp_output_182_0 = ( lerpResult288 + ( _wavecolor * ( _waveOpacity * ( SAMPLE_TEXTURE2D( _Wave, sampler_Wave, panner177 ).g * simplePerlin2D183 ) ) ) );
			float clampResult345 = clamp( ( _startend + ( ( _startend - i.uv_texcoord.y ) / 1.0 ) ) , 0.0 , 1.0 );
			float4 lerpResult572 = lerp( color343 , ( lerpResult204 * temp_output_182_0 * ( ( 1.0 - lerpResult288 ) + lerpResult288 ) ) , clampResult345);
			o.Emission = lerpResult572.rgb;
			float4 color161 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 color160 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float clampResult162 = clamp( ( 0.4393776 + ( ( 0.4393776 - i.uv_texcoord.y ) / 1.0 ) ) , 0.0 , 1.0 );
			float4 lerpResult163 = lerp( color161 , color160 , clampResult162);
			float4 lerpResult346 = lerp( color343 , ( temp_output_182_0 * lerpResult163 ) , clampResult345);
			o.Alpha = lerpResult346.r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18400
46;223;1301;631;2625.405;248.1155;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;274;-5482.459,1910.727;Inherit;False;2708.691;1374.456;;24;449;450;451;470;429;469;423;468;427;467;465;466;425;463;464;426;471;461;462;400;398;455;458;459;Lines;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;461;-4854.537,2885.956;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;471;-4787.674,2784.041;Inherit;False;Constant;_AA;AA;3;0;Create;True;0;0;False;0;False;6.55;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;462;-4824.354,3120.19;Inherit;False;Constant;_speed;speed;3;0;Create;True;0;0;False;0;False;-0.4376542;0.3;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;520;-9189.259,-748.6146;Inherit;False;2604.377;1795.322;;38;555;551;549;548;547;546;545;544;543;542;541;540;539;538;537;536;535;534;533;532;531;530;529;528;527;526;525;524;523;522;521;569;575;588;589;590;591;587;Dots;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;522;-8715.718,93.34698;Inherit;False;Constant;_Vector2;Vector 2;3;0;Create;True;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;459;-5280.64,1959.957;Inherit;False;Constant;_Vector6;Vector 6;3;0;Create;True;0;0;False;0;False;4,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;464;-4565.142,3028.687;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;463;-4592.498,2885.956;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;523;-8743.22,-333.0388;Inherit;False;Constant;_tiling;tiling;3;0;Create;True;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;521;-8715.35,521.0235;Inherit;False;Constant;_Vector1;Vector 1;3;0;Create;True;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;426;-5336.669,2477.894;Inherit;False;Constant;_Ve;Ve;13;0;Create;True;0;0;False;0;False;30,30;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;533;-8490.151,404.8762;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;524;-8517.653,-21.50983;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;530;-8506.888,849.6563;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;526;-8537.153,-297.11;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;525;-8512.453,-157.7097;Inherit;False;Property;_speedA;speedA;6;0;Create;True;0;0;False;0;False;0,-0.03;0,-0.03;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;534;-8509.651,129.276;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;527;-8484.585,697.353;Inherit;False;Property;_speedC;speedC;8;0;Create;True;0;0;False;0;False;0,-0.02;0,-0.02;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;531;-8509.284,556.9525;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;529;-8484.951,269.6762;Inherit;False;Property;_speedB;speedB;7;0;Create;True;0;0;False;0;False;0,-0.01;0,-0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.IntNode;532;-8847.161,878.1523;Inherit;False;Constant;_Int0;Int 0;6;0;Create;True;0;0;False;0;False;90;82;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;535;-8618.77,-454.2418;Inherit;False;Constant;_Int2;Int 2;4;0;Create;True;0;0;False;0;False;90;82;0;1;INT;0
Node;AmplifyShaderEditor.RangedFloatNode;465;-4353.167,3065.891;Inherit;False;Constant;_V;V;3;0;Create;True;0;0;False;0;False;-0.54;0;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;466;-4421.298,2878.969;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;425;-5131.085,2492.38;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;458;-5074.574,1995.886;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;528;-9139.26,463.0098;Inherit;False;Constant;_Int1;Int 1;5;0;Create;True;0;0;False;0;False;90;82;0;1;INT;0
Node;AmplifyShaderEditor.RadiansOpNode;544;-8458.769,-470.2418;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.Vector2Node;540;-8490.769,-646.2418;Inherit;False;Constant;_Vector3;Vector 3;3;0;Create;True;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;537;-8273.051,241.0762;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RadiansOpNode;543;-8687.158,862.1523;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.PannerNode;536;-8300.554,-185.3098;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;542;-8820.156,714.3891;Inherit;False;Constant;_Vector5;Vector 5;3;0;Create;True;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleSubtractOpNode;468;-4105.1,2997.76;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;539;-9011.258,271.0097;Inherit;False;Constant;_Vector4;Vector 4;3;0;Create;True;0;0;False;0;False;0.5,0.5;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;455;-4837.974,2107.686;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;541;-8272.684,668.7529;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;467;-4272.809,2871.981;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;427;-4830.263,2539.894;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RadiansOpNode;538;-8979.257,447.0098;Inherit;False;1;0;INT;0;False;1;INT;0
Node;AmplifyShaderEditor.RotatorNode;551;-8295.726,-698.6146;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinTimeNode;316;-2609.482,654.3885;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;469;-3986.309,2830.054;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;549;-8104.692,348.1917;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;590;-7210.278,945.0296;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;591;-7253.479,812.2298;Inherit;False;Constant;_Vector0;Vector 0;14;0;Create;True;0;0;False;0;False;0.05,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;547;-7886.071,618.9259;Inherit;False;294.4658;242.2329;;1;554;M_Dots;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;548;-7980.409,-263.0912;Inherit;False;315.6152;240.7224;;1;553;Basedots;1,1,1,1;0;0
Node;AmplifyShaderEditor.RotatorNode;545;-8237.083,818.2383;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;588;-7200.679,618.6295;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;315;-2668.163,827.696;Inherit;False;Constant;_Float9;Float 9;2;0;Create;True;0;0;False;0;False;0.18;0.7295765;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;546;-7728.089,206.7395;Inherit;False;303.5298;248.2758;;1;552;L_dots;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;423;-4653.001,2564.499;Inherit;True;Rectangle;-1;;21;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;0.85;False;3;FLOAT;0.85;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;398;-4591.653,2074.814;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;False;173;None;None;True;0;False;white;Auto;False;Instance;173;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;569;-8293.116,30.43382;Inherit;True;Property;_DotsA;DotsA;1;0;Create;True;0;0;False;0;False;7a875ecbc59da9e4599d2560bb22fa0e;7a875ecbc59da9e4599d2560bb22fa0e;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexCoordVertexDataNode;517;-2665.707,953.1854;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;589;-6999.077,735.4296;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;553;-7954.791,-213.0912;Inherit;True;Property;_A;A;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;554;-7872.328,668.926;Inherit;True;Property;_C;C;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;552;-7707.241,257.9237;Inherit;True;Property;_B;B;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;317;-2407.519,719.8904;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;519;-2625.928,1198.336;Inherit;False;Constant;_Float14;Float 14;12;0;Create;True;0;0;False;0;False;0.87;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;400;-4206.907,2066.917;Inherit;True;BlockLine;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;470;-3783.664,2971.556;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;451;-4056.378,2625.795;Inherit;False;Constant;_I2;I2;14;0;Create;True;0;0;False;0;False;0.4722091;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;429;-4314.115,2487.173;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;518;-2624.861,1095.131;Inherit;False;Constant;_Float13;Float 13;12;0;Create;True;0;0;False;0;False;0.99;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-2503.471,-283.6924;Inherit;False;Property;_waverate;waverate;11;0;Create;True;0;0;False;0;False;0.1;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;573;-6702.263,1157.266;Inherit;False;Constant;_Float10;Float 10;12;0;Create;True;0;0;False;0;False;0.83;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;574;-6701.197,1054.061;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;False;0;False;0.98;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;450;-3711.984,2515.789;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;555;-7121.876,296.9603;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;575;-6742.042,912.1158;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;175;-2350.758,-990.2471;Inherit;False;Constant;_Vector7;Vector 7;10;0;Create;True;0;0;False;0;False;1.2,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;279;-1841.456,-3.735834;Inherit;False;Property;_StatusChange;StatusChange;12;0;Create;True;0;0;False;0;False;0.01;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;188;-2422.91,-147.359;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;587;-6817.832,672.8297;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;173;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;516;-2371.996,1045.688;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;276;-2422.376,365.4026;Inherit;True;400;BlockLine;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;280;-1767.949,-156.938;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;318;-2272.357,675.3824;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;576;-6448.332,1004.618;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;-2154.374,-188.6722;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;282;-1546.429,-36.08218;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;449;-3091.156,2505.998;Inherit;True;Grid;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;402;-1942.036,439.7371;Inherit;False;Constant;_Float8;Float 8;13;0;Create;True;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;191;-2249.951,-404.5253;Inherit;False;Constant;_Vector9;Vector 9;10;0;Create;True;0;0;False;0;False;21,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;595;-6625.335,248.2143;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;281;-1882.322,101.696;Inherit;False;Constant;_Float7;Float 7;8;0;Create;True;0;0;False;0;False;0.8932782;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;304;-2043.798,643.6967;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;174;-2163.6,-983.1846;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;577;-6290.596,286.8452;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;155;-982.8649,1058.136;Inherit;False;Constant;_length;length;8;0;Create;True;0;0;False;0;False;0.4393776;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;190;-2013.353,-307.4142;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;154;-1128.762,900.9152;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;177;-1926.358,-872.284;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;336;-1913.036,228.3497;Inherit;False;Property;_lineColor;lineColor;4;1;[HDR];Create;True;0;0;False;0;False;0.7450981,0.2784314,0.08627451,0;0.7450981,0.2784314,0.08627451,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;374;-1712.671,813.3271;Inherit;False;Property;_gridcolor;gridcolor;5;1;[HDR];Create;True;0;0;False;0;False;0.5843138,0.007843138,0.007843138,0;0.5843138,0.007843138,0.007843138,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;283;-1543.573,134.8274;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;362;-1674.437,1066.35;Inherit;True;449;Grid;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;401;-1775.886,441.6503;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;173;-1827.785,-730.3036;Inherit;True;Property;_Wave;Wave;0;0;Create;True;0;0;False;0;False;-1;16928f2a27bd2b94494b119ef363ff9a;16928f2a27bd2b94494b119ef363ff9a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;373;-1426.068,666.1766;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;556;-6062.061,277.1778;Inherit;True;dotline;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;284;-1401.682,140.8735;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;156;-880.7621,923.4152;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;157;-854.2,1214.453;Inherit;False;Constant;_blur;blur;8;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;334;-1559.799,292.5374;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;183;-1748.639,-454.6879;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;211;-2216.989,-1207.759;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;515;-1287.699,-107.8762;Inherit;True;556;dotline;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;195;-1783.654,-982.1548;Inherit;False;Property;_waveOpacity;waveOpacity;10;0;Create;True;0;0;False;0;False;0.8;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;363;-1269.466,288.7416;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;192;-1498.065,-707.8339;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;158;-710.1373,1127.79;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;337;57.5629,889.1207;Inherit;False;Property;_startend;startend;13;0;Create;True;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;338;32.90927,705.905;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;285;-1229.858,109.317;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;-1491.178,-967.2057;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;201;-2144.236,-1414.882;Inherit;False;Property;_wavecolor;wavecolor;2;1;[HDR];Create;True;0;0;False;0;False;0.06666667,0.6,0.945098,0;0.06666667,0.6,0.945098,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;340;117.1045,1008.39;Inherit;False;Constant;_BF;BF;8;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;159;-562.0123,1035.915;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;288;-918.1913,59.03682;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SinOpNode;210;-1956.117,-1212.257;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;212;-1912.075,-1100.676;Inherit;False;Constant;_Int3;Int 3;12;0;Create;True;0;0;False;0;False;2;0;0;1;INT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;339;280.9092,728.405;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;160;-592.3774,600.6573;Inherit;False;Constant;_ColorA;ColorA;10;0;Create;True;0;0;False;0;False;1,1,1,0;0.7578583,0.7578583,0.7578583,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;161;-601.5644,796.3531;Inherit;False;Constant;_ColorB;ColorB;11;0;Create;True;0;0;False;0;False;0,0,0,0;0.5575534,0.5823337,0.5916263,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;162;-410.6161,974.5052;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;-1036.007,-269.5849;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;11;-598.2682,160.0552;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;341;451.5341,932.7802;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;207;-2002.079,-1591.351;Inherit;False;Property;_wavecolor2;wavecolor2;3;1;[HDR];Create;True;0;0;False;0;False;0.1764706,0.5568628,0.9882353,0;0.1764706,0.5568628,0.9882353,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;206;-1758.99,-1201.724;Inherit;False;2;2;0;FLOAT;0;False;1;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;182;-661.6266,-238.9537;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;12;-423.7793,-11.20953;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;204;-1466.177,-1409.77;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;342;599.6591,840.9051;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;163;-327.2654,733.9267;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;-158.0365,-238.9582;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;343;436.5276,135.4126;Inherit;False;Constant;_black;black;11;0;Create;True;0;0;False;0;False;0,0,0,0;0.5575534,0.5823337,0.5916263,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;166;20.2746,341.3177;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;345;751.0551,779.495;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;346;646.1218,298.9766;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;572;903.8926,-96.40147;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1330.592,-111.6594;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;BoLineFinalDot2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;4;1;False;-1;1;False;-1;0;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;9;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;True;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;464;0;462;0
WireConnection;463;0;471;0
WireConnection;463;1;461;2
WireConnection;526;0;523;0
WireConnection;534;0;522;0
WireConnection;531;0;521;0
WireConnection;466;0;463;0
WireConnection;466;1;464;0
WireConnection;425;0;426;0
WireConnection;458;0;459;0
WireConnection;544;0;535;0
WireConnection;537;0;534;0
WireConnection;537;2;529;0
WireConnection;537;1;533;0
WireConnection;543;0;532;0
WireConnection;536;0;526;0
WireConnection;536;2;525;0
WireConnection;536;1;524;0
WireConnection;468;1;465;0
WireConnection;455;0;458;0
WireConnection;541;0;531;0
WireConnection;541;2;527;0
WireConnection;541;1;530;0
WireConnection;467;0;466;0
WireConnection;427;0;425;0
WireConnection;538;0;528;0
WireConnection;551;0;536;0
WireConnection;551;1;540;0
WireConnection;551;2;544;0
WireConnection;469;0;467;0
WireConnection;469;1;468;0
WireConnection;549;0;537;0
WireConnection;549;1;539;0
WireConnection;549;2;538;0
WireConnection;545;0;541;0
WireConnection;545;1;542;0
WireConnection;545;2;543;0
WireConnection;423;1;427;0
WireConnection;398;1;455;0
WireConnection;589;0;588;0
WireConnection;589;2;591;0
WireConnection;589;1;590;0
WireConnection;553;0;569;0
WireConnection;553;1;551;0
WireConnection;554;0;569;0
WireConnection;554;1;545;0
WireConnection;552;0;569;0
WireConnection;552;1;549;0
WireConnection;317;0;316;4
WireConnection;317;1;315;0
WireConnection;400;0;398;1
WireConnection;470;0;469;0
WireConnection;470;1;468;0
WireConnection;429;0;423;0
WireConnection;450;0;451;0
WireConnection;450;1;429;0
WireConnection;450;2;470;0
WireConnection;555;0;553;3
WireConnection;555;1;552;2
WireConnection;555;2;554;1
WireConnection;587;1;589;0
WireConnection;516;0;517;2
WireConnection;516;1;518;0
WireConnection;516;2;519;0
WireConnection;318;0;317;0
WireConnection;576;0;575;2
WireConnection;576;1;574;0
WireConnection;576;2;573;0
WireConnection;185;0;187;0
WireConnection;185;1;188;4
WireConnection;282;0;279;0
WireConnection;282;1;280;2
WireConnection;449;0;450;0
WireConnection;595;0;555;0
WireConnection;595;1;587;3
WireConnection;304;0;276;0
WireConnection;304;1;318;0
WireConnection;304;2;516;0
WireConnection;174;0;175;0
WireConnection;577;0;595;0
WireConnection;577;1;576;0
WireConnection;190;0;191;0
WireConnection;190;1;185;0
WireConnection;177;0;174;0
WireConnection;283;0;282;0
WireConnection;283;1;281;0
WireConnection;401;0;402;0
WireConnection;401;1;304;0
WireConnection;173;1;177;0
WireConnection;373;0;374;0
WireConnection;373;1;362;0
WireConnection;556;0;577;0
WireConnection;284;0;279;0
WireConnection;284;1;283;0
WireConnection;156;0;155;0
WireConnection;156;1;154;2
WireConnection;334;0;336;0
WireConnection;334;1;401;0
WireConnection;183;0;190;0
WireConnection;363;0;334;0
WireConnection;363;1;373;0
WireConnection;192;0;173;2
WireConnection;192;1;183;0
WireConnection;158;0;156;0
WireConnection;158;1;157;0
WireConnection;285;0;284;0
WireConnection;194;0;195;0
WireConnection;194;1;192;0
WireConnection;159;0;155;0
WireConnection;159;1;158;0
WireConnection;288;0;515;0
WireConnection;288;1;363;0
WireConnection;288;2;285;0
WireConnection;210;0;211;2
WireConnection;339;0;337;0
WireConnection;339;1;338;2
WireConnection;162;0;159;0
WireConnection;200;0;201;0
WireConnection;200;1;194;0
WireConnection;11;0;288;0
WireConnection;341;0;339;0
WireConnection;341;1;340;0
WireConnection;206;0;210;0
WireConnection;206;1;212;0
WireConnection;182;0;288;0
WireConnection;182;1;200;0
WireConnection;12;0;11;0
WireConnection;12;1;288;0
WireConnection;204;0;201;0
WireConnection;204;1;207;0
WireConnection;204;2;206;0
WireConnection;342;0;337;0
WireConnection;342;1;341;0
WireConnection;163;0;161;0
WireConnection;163;1;160;0
WireConnection;163;2;162;0
WireConnection;198;0;204;0
WireConnection;198;1;182;0
WireConnection;198;2;12;0
WireConnection;166;0;182;0
WireConnection;166;1;163;0
WireConnection;345;0;342;0
WireConnection;346;0;343;0
WireConnection;346;1;166;0
WireConnection;346;2;345;0
WireConnection;572;0;343;0
WireConnection;572;1;198;0
WireConnection;572;2;345;0
WireConnection;0;2;572;0
WireConnection;0;9;346;0
ASEEND*/
//CHKSM=7BFADA4CA5C8AEFA263108B0FEF053CE5678F260