Shader "Sample/RightPlayerShader"
{
    Properties
    {
        _MainTex ("Background Texture", 2D) = "Black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Geometry" 
        "Queue"= "Geometry"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Attribute
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Fragment
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            uniform sampler2D _UVTex;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float exist = 0.0;

            Fragment vert (Attribute input)
            {
                Fragment output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            float4 frag (Fragment fragment) : SV_Target
            {
                float4 colour;

                if (exist > 0.5){
                    float4 uv = tex2D(_UVTex,fragment.uv);
                    colour = tex2D(_MainTex, float2(uv.x, uv.y));
                }

                else
                    colour = tex2D(_MainTex, fragment.uv);
                
                return colour;
            }
            ENDCG
        }
    }
}