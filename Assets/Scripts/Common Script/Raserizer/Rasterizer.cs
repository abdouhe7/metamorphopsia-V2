using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomGrid;

namespace RasterizerCS
{
    public class Rasterizer
    {
        /*singleton mode*/
        private static readonly Rasterizer instance = new Rasterizer();
        static Rasterizer() { }
        private Rasterizer() { }
        public static Rasterizer Instance() { return instance; }
        //

        Mesh mesh;
        Transform transform;
        Camera camera;

        ComputeBuffer VertexBuffer;
        ComputeBuffer UVBuffer;
        ComputeBuffer QuadBuffer;

        ComputeBuffer VertexOutBuffer;

        ComputeShader computeShader;
        int kernelVertexProcess;
        int kernelQuadProcess;

        private RenderTexture UVCoordinateTexture;

        int[] QuadIndex()
        {
            int width = GridGeneration.Instance().GetWidthVerticesNumber();
            int height = GridGeneration.Instance().GetHeightVerticesNumber();

            int index = 0;
            int[] quads = new int[(height - 1) * (width - 1) * 4];

            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    if (index > quads.Length)
                    {
                        Debug.Log("The generation of quad's index is out of range.");
                    }
                    quads[index++] = (y + 0) * width + (x + 0);
                    quads[index++] = (y + 1) * width + (x + 0);
                    quads[index++] = (y + 1) * width + (x + 1);
                    quads[index++] = (y + 0) * width + (x + 1);
                }
            }
            return quads;
        }

        void TransportData()
        {
            int vertexCount = mesh.vertexCount;

            VertexBuffer = new ComputeBuffer(vertexCount, 3 * sizeof(float));
            VertexBuffer.SetData(mesh.vertices);

            UVBuffer = new ComputeBuffer(vertexCount, 2 * sizeof(float));
            UVBuffer.SetData(mesh.uv);

            int[] quad = QuadIndex();
            QuadBuffer = new ComputeBuffer(quad.Length / 4, 4 * sizeof(uint));
            QuadBuffer.SetData(quad);

            VertexOutBuffer = new ComputeBuffer(vertexCount, 9 * sizeof(float));
        }
        void DataBufferRelease()
        {
            VertexBuffer.Release();
            VertexBuffer = null;
            UVBuffer.Release();
            UVBuffer = null;
            QuadBuffer.Release();
            QuadBuffer = null;
            VertexOutBuffer.Release();
            VertexOutBuffer = null;
        }

        void VertexProcess()
        {
            Matrix4x4 matModel = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Matrix4x4 matView = camera.worldToCameraMatrix;
            Matrix4x4 matProjection = camera.projectionMatrix;
            Matrix4x4 mvp = matProjection * matView * matModel;

            computeShader.SetMatrix("matMVP", mvp);
            computeShader.SetMatrix("matModel", matModel);

            computeShader.SetBuffer(kernelVertexProcess, "vertexBuffer", VertexBuffer);
            computeShader.SetBuffer(kernelVertexProcess, "uvBuffer", UVBuffer);

            computeShader.SetBuffer(kernelVertexProcess, "vertexOutBuffer", VertexOutBuffer);

            int groupCount = Mathf.CeilToInt(mesh.vertexCount / 512f);
            computeShader.Dispatch(kernelVertexProcess, groupCount, 1, 1);
        }

        void TriangleProcess()
        {
            computeShader.SetTexture(kernelQuadProcess, "frameUVTexture", UVCoordinateTexture);
            computeShader.SetBuffer(kernelQuadProcess, "quadBuffer", QuadBuffer);
            computeShader.SetBuffer(kernelQuadProcess, "vertexOutBuffer", VertexOutBuffer);
            computeShader.SetInts("frameBufferSize", SimpleGameManager.Instance.width, SimpleGameManager.Instance.height);

            int groupCount = Mathf.CeilToInt(mesh.vertexCount / 512f);
            computeShader.Dispatch(kernelQuadProcess, groupCount, 1, 1);
        }

        public void Initilize(ComputeShader computeShader_)
        {
            computeShader = computeShader_;

            UVCoordinateTexture = new RenderTexture(SimpleGameManager.Instance.width, SimpleGameManager.Instance.height, 0, RenderTextureFormat.ARGBFloat);
            UVCoordinateTexture.filterMode = FilterMode.Point;
            UVCoordinateTexture.enableRandomWrite = true;
            UVCoordinateTexture.Create();

            kernelVertexProcess = computeShader.FindKernel("VertexProcess");
            kernelQuadProcess = computeShader.FindKernel("QuadProcess");
        }

        public RenderTexture Refresh(Mesh mesh_, Transform transform_, Camera camera_)
        {
            camera = camera_;
            mesh = mesh_;
            transform = transform_;

            TransportData();
            VertexProcess();
            TriangleProcess();

            DataBufferRelease();
            return UVCoordinateTexture;
        }
    }
}