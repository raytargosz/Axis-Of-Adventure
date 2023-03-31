Shader "Custom/TransparentShader" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Specular ("Specular", Range(0, 1)) = 0.0
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf BlinnPhong

        sampler2D _MainTex;
        float _Glossiness;
        float _Specular;
        half4 _Color;

        struct Input {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Specular = _Specular;
            o.Gloss = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
