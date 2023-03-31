Shader "Custom/Mask" {
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct appdata {
            float4 vertex : POSITION;
        };

        struct v2f {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        v2f vert (appdata v) {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target {
            return fixed4(0, 0, 0, 0);
        }
        ENDCG
    }
    Stencil {
        Ref 1
        Comp Always
        Pass Replace
    }
}