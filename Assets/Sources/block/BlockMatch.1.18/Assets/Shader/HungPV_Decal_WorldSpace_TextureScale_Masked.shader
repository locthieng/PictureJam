Shader "HungPV/Decal_WorldSpace_TextureScale_Masked" {
	Properties {
		_MainTex ("Decal Texture (RGBA)", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		_AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.001
		_DecalScale ("Decal Scale (texture-based)", Float) = 1
		_DecalRotation ("Decal Rotation (deg)", Range(-180, 180)) = 0
		_DecalOffset ("Decal Offset (Local X,Y World Units)", Vector) = (0,0,0,0)
		[Toggle] _DecalFlipX ("Decal Flip X", Float) = 0
		[Toggle] _DecalFlipY ("Decal Flip Y", Float) = 0
		_PixelsPerUnit ("Pixels Per World Unit", Float) = 256
		_MaskTex ("Mask Texture", 2D) = "white" {}
		[Header(Mask Transform)] [Toggle] _MaskFlipX ("Mask Flip X", Float) = 0
		_MaskRotation ("Mask Rotation (Degrees)", Range(-360, 360)) = 0
		[Header(Outline Layer 1)] _Outline1Color ("Outline 1 Color", Vector) = (0,0,0,1)
		_Outline1WidthPixels ("Outline 1 Width (pixels)", Float) = 2
		[Header(Outline Layer 2)] _Outline2Color ("Outline 2 Color", Vector) = (1,1,1,1)
		_Outline2WidthPixels ("Outline 2 Width (pixels)", Float) = 4
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
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
}