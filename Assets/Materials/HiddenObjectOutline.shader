Shader "Custom/HiddenObjectOutline" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.005
        _OcclusionThreshold ("Occlusion Threshold", Range(0, 100)) = 50
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float4 _Color;
        float _OutlineWidth;
        float _OcclusionThreshold;

        struct Input {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
            o.Alpha = 1.0;

            // Calculate occlusion
            float occlusion = dot(IN.viewDir, o.Normal);

            // Apply outline based on occlusion threshold
            if (occlusion * 100.0 >= _OcclusionThreshold) {
                float2 uvOffset = _OutlineWidth * (1.0 - occlusion) * float2(sign(-IN.viewDir.x), sign(-IN.viewDir.y));
                float outline = tex2D(_MainTex, IN.uv_MainTex + uvOffset).a;
                o.Emission = _OutlineColor.rgb * outline;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}