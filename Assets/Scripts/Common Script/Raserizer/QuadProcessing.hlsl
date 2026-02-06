float4 ComputeBarycentric2D(float x, float y, float4 v[4])
{
    float2 s0 = v[0].xy - float2(x, y);
    float2 s1 = v[1].xy - float2(x, y);
    float2 s2 = v[2].xy - float2(x, y);
    float2 s3 = v[3].xy - float2(x, y);

    float D0 = dot(s0,s1);
    float D1 = dot(s1,s2);
    float D2 = dot(s2,s3);
    float D3 = dot(s3,s0);

    float r0 = length(s0);
    float r1 = length(s1);
    float r2 = length(s2);
    float r3 = length(s3);

    float A0 = (v[0].x - x)*(v[1].y - y) - (v[1].x - x)*(v[0].y - y);
    float A1 = (v[1].x - x)*(v[2].y - y) - (v[2].x - x)*(v[1].y - y);
    float A2 = (v[2].x - x)*(v[3].y - y) - (v[3].x - x)*(v[2].y - y);
    float A3 = (v[3].x - x)*(v[0].y - y) - (v[0].x - x)*(v[3].y - y);

    float tan0 = (r0*r1 - D0)/A0;
    float tan1 = (r1*r2 - D1)/A1;
    float tan2 = (r2*r3 - D2)/A2;
    float tan3 = (r3*r0 - D3)/A3;

    float u0 = (tan3 + tan0) / r0;
    float u1 = (tan0 + tan1) / r1;
    float u2 = (tan1 + tan2) / r2;
    float u3 = (tan2 + tan3) / r3;

    float sum = u0+u1+u2+u3;

    return float4(u0/sum, u1/sum, u2/sum, u3/sum);
}

void RasterizeTriangle(int idx0, int idx1, int idx2, int idx3, float4 v[4])
{        
    //Find out the bounding box of current triangle.
    float minX = v[0].x;
    float maxX = minX;
    float minY = v[0].y;
    float maxY = minY;

    for(int i=1; i<4; ++i)
    {
        float x = v[i].x;
        float y = v[i].y;

        if(x < minX)
        {
            minX = x;
        } 
        else if(x > maxX)
        {
            maxX = x;
        }

        if(y < minY)
        {
            minY = y;
        }
        else if(y > maxY)
        {
            maxY = y;
        }
    }

    int minPX = floor(minX);
    minPX = minPX < 0 ? 0 : minPX;
    int maxPX = ceil(maxX);
    maxPX = maxPX > frameBufferSize.x ? frameBufferSize.x : maxPX;
    int minPY = floor(minY);
    minPY = minPY < 0 ? 0 : minPY;
    int maxPY = ceil(maxY);
    maxPY = maxPY > frameBufferSize.y ? frameBufferSize.y : maxPY;

    //
    VertexOutBuf vertex0 = vertexOutBuffer[idx0];
    VertexOutBuf vertex1 = vertexOutBuffer[idx1];
    VertexOutBuf vertex2 = vertexOutBuffer[idx2];
    VertexOutBuf vertex3 = vertexOutBuffer[idx3];

    const float tolerant = -0.0005;

    for(int y = minPY; y < maxPY; ++y)
    {
        for(int x = minPX; x < maxPX; ++x)
        {
            float4 c = ComputeBarycentric2D(x, y, v);

            float lambda0 = c.x;
            float lambda1 = c.y;
            float lambda2 = c.z;
            float lambda3 = c.w;
            
            if(lambda0 < tolerant || lambda1 < tolerant || lambda2 < tolerant || lambda3 < tolerant){
                continue;
            }            

            float z = 1.0f / (lambda0 / v[0].w + lambda1 / v[1].w + lambda2 / v[2].w + lambda3/v[3].w);
            float2 uv_p = (lambda0 * vertex0.uv / v[0].w + lambda1 * vertex1.uv / v[1].w + lambda2 * vertex2.uv / v[2].w + lambda3 * vertex3.uv / v[3].w) * z;

            frameUVTexture[uint2(x,y)] = uv_p;
        }
    }
}