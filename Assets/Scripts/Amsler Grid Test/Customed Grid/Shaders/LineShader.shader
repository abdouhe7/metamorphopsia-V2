Shader "Sample/LineShader"
{
    Properties
    {
        _Color ("Line Color", Color) = (0,0,0,1)
    }
    SubShader
    {

        ZTest Always
        ZWrite Off 
        Blend SrcAlpha OneMinusSrcAlpha

        Tags { "RenderType"="Transparent"
        "Queue" = "Transparent"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Attribute
            {
                float4 vertex : POSITION;
            };

            struct Fragment
            {
                float4 vertex : SV_POSITION;
            };

            uniform float4 _Color;

            Fragment vert (Attribute input)
            {
                Fragment output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                return output;
            }

            float4 frag (Fragment fragment) : SV_Target
            {
                float4 frag_color;
                frag_color = _Color;
                frag_color.w = _Color.w;

                return frag_color;
            }
            ENDCG
        }
    }
}
