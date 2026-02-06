Shader "Sample/SampleShader"
{
    Properties
    {
        _MainTex ("Background Texture", 2D) = "white" {}
        _Color ("Background Color", Color) = (1,1,1,1)
        [Toggle(_False)]
        _ShowTexture("Use Texture", float) = 0
        _ShowGrid("Show grid layout", float) = 1
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

            uniform float _ShowGrid;
            uniform float _ShowTexture;
            uniform float4 _Color;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform int width;
            uniform int height;
            uniform float scale;

            uniform int XSelected;
            uniform int YSelected;

            uniform float4 restColor;

            Fragment vert (Attribute input)
            {
                Fragment output;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            float4 GridHide(float2 UV, float4 fragColor)
            {
                float gridX = UV.x*(width-1);
                float gridY = UV.y*(height-1);

                float offsetX = abs(frac(gridX)-0.5f);
                float offsetY = abs(frac(gridY)-0.5f);

                if ((offsetX >= scale || offsetY >= scale))
                {
                    if (floor(gridX+0.5f) == XSelected && !(offsetY >= scale))
                    {
                        fragColor = _ShowTexture >= 0.5 ? fragColor + float4(1,0,0,1) : fragColor * float4(1,0,0,1);
                    }
                    if (floor(gridY+0.5f) == YSelected && !(offsetX >= scale))
                    {
                        fragColor = _ShowTexture >= 0.5 ? fragColor + float4(0,0,1,1) : fragColor * float4(0,0,1,1);                   
                    }

                    if (floor(gridX+0.5f) == XSelected && !(offsetX <= scale))
                    {
                        fragColor = _ShowTexture >= 0.5 ? fragColor + float4(1,0,0,1) : fragColor * float4(1,0,0,1);    
                    }
                    if (floor(gridY+0.5f) == YSelected && !(offsetY <= scale))
                    {
                        fragColor = _ShowTexture >= 0.5 ? fragColor + float4(0,0,1,1) : fragColor * float4(0,0,1,1);                    }
                    }
                return fragColor;
            }

            float4 Grid(float2 UV, float4 fragColor)
            {
                float gridX = UV.x*(width-1);
                float gridY = UV.y*(height-1);

                float offsetX = abs(frac(gridX)-0.5f);
                float offsetY = abs(frac(gridY)-0.5f);

                if ((offsetX >= scale || offsetY >= scale))
                {
                    if (floor(gridX+0.5f) == XSelected && !(offsetY >= scale))
                    {
                        return float4(1,0,0,1);
                    }
                    if (floor(gridY+0.5f) == YSelected && !(offsetX >= scale))
                    {
                        return float4(0,0,1,1);
                    }

                    return restColor; 
                }
                return fragColor;
            }

            float4 frag (Fragment fragment) : SV_Target
            {
                float4 frag_color = _Color;

                if (_ShowTexture >= 0.5)
                    frag_color = tex2D(_MainTex, fragment.uv);
                
                if (_ShowGrid)
                    frag_color = Grid(fragment.uv, frag_color);
                else
                    frag_color = GridHide(fragment.uv, frag_color);
                
                return frag_color;
            }
            ENDCG
        }
    }
}
