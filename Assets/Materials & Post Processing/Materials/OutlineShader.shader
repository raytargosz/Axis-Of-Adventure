Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineThickness("Outline Thickness", Range(0, 0.05)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        
        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
        };
        
        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };
        
        float _OutlineThickness;
        float4 _OutlineColor;
        
        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _OutlineThickness);
            return o;
        }
        
        fixed4 frag (v2f i) : SV_Target
        {
            return _OutlineColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
