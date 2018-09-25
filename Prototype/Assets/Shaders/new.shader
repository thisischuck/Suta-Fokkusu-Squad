// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "New" {
	
	SubShader {
		
		Tags {
			"RenderType" = "Opaque"
			"PerformanceChecks" = "False"
		}
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
				#pragma fragment frag
				
			struct vertInput {
				float4 pos : POSITION;
			};
			
			struct vertOutput {
				float4 pos : SV_POSITION;
			};
			
			vertOutput vert(vertInput input) {
				vertOutput o;
				o.pos = UnityObjectToClipPos(input.pos);
				return o;
			}
			
			half4 frag(vertOutput output) : COLOR {
				return half4(0.5, 0.0, 0.0, 1.0);
			}
			ENDCG
		}
	}
}