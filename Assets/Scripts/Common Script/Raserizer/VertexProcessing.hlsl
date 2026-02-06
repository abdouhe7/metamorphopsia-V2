void VertexTransform (uint vertexIdx)
{
    float4 pos = float4(vertexBuffer[vertexIdx].x, vertexBuffer[vertexIdx].y, vertexBuffer[vertexIdx].z, 1.0f);
    
    vertexOutBuffer[vertexIdx].clipPos = mul(matMVP, pos);      
    vertexOutBuffer[vertexIdx].worldPos = mul(matModel, pos).xyz;
    vertexOutBuffer[vertexIdx].uv = uvBuffer[vertexIdx];
}