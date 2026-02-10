// ═══════════════════════════════════════════════════════════════
// L2 Topic: Grid shader for drawing lines  (Q3)
//
//   This shader draws a grid purely in the GPU fragment stage,
//   using the mesh's UV coordinates — no extra geometry needed.
//
//   How it works:
//     1. The vertex shader passes UVs to the fragment shader unchanged.
//     2. The fragment shader scales UVs by GridResolution so that
//        each grid cell spans exactly [0, 1] in the new space.
//     3. frac() gives the position WITHIN the current cell (0..1).
//     4. min(frac, 1-frac) gives the distance from the nearest line.
//     5. smoothstep draws an anti-aliased line when that distance
//        is smaller than half the desired line thickness.
//
//   Attach to a Material, set the Material on the MeshRenderer.
// ═══════════════════════════════════════════════════════════════
Shader "Lessons/L2_GridLines"
{
    Properties
    {
        _LineColor      ("Line Color",      Color)  = (0, 0, 0, 1)
        _FillColor      ("Fill Color",      Color)  = (1, 1, 1, 1)
        // Number of grid squares along each axis (use 20 for Amsler Grid)
        _GridResolution ("Grid Resolution", Float)  = 20
        // Line thickness as a fraction of one cell width (0..1)
        // e.g. 0.04 = 4% of cell width → visible but not too thick
        _LineThickness  ("Line Thickness",  Float)  = 0.04
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // ── Input structs ────────────────────────────────────────
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            // ── Properties ──────────────────────────────────────────
            fixed4 _LineColor;
            fixed4 _FillColor;
            float  _GridResolution;
            float  _LineThickness;

            // ── Vertex shader ────────────────────────────────────────
            //   Simply transforms position to clip space and forwards UVs.
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            // ── Fragment shader ──────────────────────────────────────
            fixed4 frag(v2f i) : SV_Target
            {
                // Scale UV so each cell occupies [0, 1]
                float2 cellUV = i.uv * _GridResolution;

                // Position within the cell: 0 = line, 0.5 = cell centre
                float2 cellPos = frac(cellUV);

                // Distance from the nearest line edge (0 = on line, 0.5 = centre)
                float2 distFromLine = min(cellPos, 1.0 - cellPos);

                // Half of the desired line thickness in cell-fraction units
                float halfThick = _LineThickness * 0.5;

                // Anti-aliased blend: 0 = on the line, 1 = away from line
                // fwidth gives the screen-space derivative so the edge stays
                // 1 pixel wide regardless of zoom level.
                float2 fw       = fwidth(distFromLine);
                float  blendX   = smoothstep(halfThick - fw.x, halfThick + fw.x, distFromLine.x);
                float  blendY   = smoothstep(halfThick - fw.y, halfThick + fw.y, distFromLine.y);
                float  onLine   = 1.0 - min(blendX, blendY);   // 1 = on line

                return lerp(_FillColor, _LineColor, onLine);
            }
            ENDCG
        }
    }

    Fallback "Unlit/Color"
}
