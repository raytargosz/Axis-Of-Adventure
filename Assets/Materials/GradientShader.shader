Shader "Custom/GradientShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gradient ("Gradient", 2D) = "white" {}
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _Gradient;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Gradient;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Use the _MainTex texture for the albedo
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;

            // Use the _Gradient texture for the alpha channel
            o.Alpha = tex2D(_Gradient, IN.uv_Gradient).r;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
