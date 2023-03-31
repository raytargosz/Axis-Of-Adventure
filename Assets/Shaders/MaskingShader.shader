Shader "Custom/MaskingShader" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG

        Stencil {
            Ref 1
            Comp Equal
            Pass Keep
        }
    }
    FallBack "Diffuse"
}