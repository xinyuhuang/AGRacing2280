Shader "Hidden/BoostRadialBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, Xbox360, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 xbox360 gles
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform float _BlurStrength;
			uniform float _SampleStrength;
			
			float4 frag(v2f_img i) : COLOR
			{
				half4 screen = tex2D(_MainTex, i.uv);
				return screen;
			}
			
			ENDCG
		}
	} 
}
