Shader "Sample/BoardShader"
{
    Properties
    {
        _MainTex ("Background Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" 
        "Queue"= "Transparent"}

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

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

            uniform float _ShowTexture;

            uniform sampler2D _UVTex;
            sampler2D _MainTex;
            float4 _MainTex_ST;


            Fragment vert (Attribute input)
            {
                Fragment output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                // output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.uv = input.uv;
                return output;
            }

            float4 frag (Fragment fragment) : SV_Target
            {
                float4 colour;
                if (_ShowTexture >= 0.5){
                    float4 uv = tex2D(_UVTex,fragment.uv);
                    colour = tex2D(_MainTex, float2(uv.x, uv.y));
                }
                else
                    colour = float4(0,0,0,0);
                
                return colour;
            }
            ENDCG
        }
    }
}
