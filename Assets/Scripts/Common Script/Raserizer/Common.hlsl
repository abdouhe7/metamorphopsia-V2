struct VertexOutBuf
{
    float4 clipPos;
    float3 worldPos;
    float2 uv;
};

float4x4 matMVP;
float4x4 matModel;

float3 worldSpaceCameraPos;

int2 frameBufferSize;

StructuredBuffer<float3> vertexBuffer;
StructuredBuffer<float2> uvBuffer;
StructuredBuffer<uint4> quadBuffer;

RWStructuredBuffer<VertexOutBuf> vertexOutBuffer;
RWTexture2D<float2> frameUVTexture;
