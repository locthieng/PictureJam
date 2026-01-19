Shader "Custom/UnlitTextureBlend" {
	Properties {
		[Header(Albedo Map)] _AlbedoMap ("Albedo Map", 2D) = "white" {}
		_Tint ("Tint", Vector) = (1,1,1,1)
		[Header(Gradient Map)] [Toggle] _UseGradientMap ("Use Gradient Map", Float) = 0
		_ColorShadow ("1. Shadow Color (0.0)", Vector) = (0,0,0,1)
		_ColorMidDark ("2. MidDark (≈0.33)", Vector) = (0.33,0.33,0.33,1)
		_ColorMidLight ("3. MidLight (≈0.66)", Vector) = (0.66,0.66,0.66,1)
		_ColorHighlight ("4. Highlight Color (1.0)", Vector) = (1,1,1,1)
		_MidPoint1 ("Midpoint 1 (Shadow → MidDark)", Range(0.01, 0.99)) = 0.33
		_MidPoint2 ("Midpoint 2 (MidLight → Highlight)", Range(0.01, 0.99)) = 0.66
		[Header(Texture Settings)] _MainTex ("Texture 2D", 2D) = "white" {}
		_TextureScale ("Texture Scale", Range(0.01, 10)) = 1
		_TextureOpacity ("Texture Opacity", Range(0, 1)) = 1
		_TextureRotation ("Texture Rotation (Degrees)", Range(-360, 360)) = 0
		[KeywordEnum(Multiply, Add, Overlay, Normal, Screen)] _BlendMode ("Blend Mode", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy);
			}

			ENDHLSL
		}
	}
	Fallback "Unlit/Color"
}