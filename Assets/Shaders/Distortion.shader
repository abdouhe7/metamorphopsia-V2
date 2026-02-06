Shader "Custom/RippleDistortionShader" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Distortion("Distortion Amount", Float) = 0.05
        _Frequency("Frequency", Float) = 10.0
        _Speed("Speed", Float) = 1.0
    }
        SubShader{
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _Distortion;
                float _Frequency;
                float _Speed;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float2 center = float2(0.5, 0.5);
                    float2 uv = i.uv;
                    float2 offset = uv - center;
                    float dist = length(offset);
                    float ripple = sin(dist * _Frequency - _Time * _Speed) * _Distortion;
                    uv += offset / dist * ripple;
                    return tex2D(_MainTex, uv);
                }
                ENDCG
            }
        }
            FallBack "Diffuse"
}
