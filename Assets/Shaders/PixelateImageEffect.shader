Shader "Hidden/PixelateImageEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Columns("Pixel Column", int) = 160
		_Rows("Pixel Row", int) = 160

		_FColor1("Force Color 1", Color) =  (1,1,1,1)
		_FColor2("Force Color 2", Color) = (0,0,0,1)
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#include "UnityCG.cginc"
		
			float4 _FColor1;
			float4 _FColor2;
			int _Columns;
			int _Rows;
			sampler2D _MainTex;

			float4 _MainTex_ST;

			struct Interpolators
			{
				float4 position : SV_POSITION;
				float2 uv: TEXCOORD0;
			};

			struct VertexData
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			Interpolators MyVertexProgram(VertexData v)
			{
				Interpolators i;

				i.position = UnityObjectToClipPos(v.position);
				i.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

				return i;
			}

			float4 MyFragmentProgram(Interpolators i) : SV_TARGET
			{
				i.uv.x = round(i.uv.x * _Columns) / _Columns;
				i.uv.y = round(i.uv.y * _Rows) / _Rows;

				fixed4 _OriC = tex2D(_MainTex, i.uv);
				float _C1SqrDiff = pow(_OriC.x - _FColor1.x,2) + pow(_OriC.y - _FColor1.y,2) + pow(_OriC.z - _FColor1.z,2);
				float _C2SqrDiff = pow(_OriC.x - _FColor2.x,2) + pow(_OriC.y - _FColor2.y,2) + pow(_OriC.z - _FColor2.z,2);
				if(_C1SqrDiff > _C2SqrDiff) return _FColor2;
				else return _FColor1;
			}

			ENDCG
		}
	}
}
