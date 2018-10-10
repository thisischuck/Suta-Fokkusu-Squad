/*Shader "Custom/Teste" {
    SubShader {
        Pass {
            Material {
                Diffuse (1,1,1,1)
            }
            Lighting On
            Cull Off
        }
    }
}*/

Shader "Custom/Teste" {
    Properties 
    {
    _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader 
    {
        Material {
            Diffuse (1,1,1,1)
        }
        //Lighting On
        Cull Off
        Tags { "RenderType" = "Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert
        struct Input 
        {
            float2 uv_MainTex;
        };
        sampler2D _MainTex;
        void surf (Input IN, inout SurfaceOutput o) 
        {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
        }
    ENDCG
    } 
    Fallback "Diffuse"
}
