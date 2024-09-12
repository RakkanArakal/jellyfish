using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraBuffer : MonoBehaviour
{
    public Camera[] renderCameras;
    public Camera camera;
    public Mesh sphereMesh;
    public Material renderMat;
    public RenderTexture depthTexture;
    private Mesh screenQuadMesh;
    private CommandBuffer commandBuffer;
    private ComputeBuffer sphereInstancedArgsBuffer;


    // Start is called before the first frame update
    void Start()
    {
        // camera.depthTextureMode |= DepthTextureMode.Depth;

        // camera.depthTextureMode = DepthTextureMode.DepthNormals;
        // screenQuadMesh = new Mesh();
        // screenQuadMesh.vertices = new Vector3[4] {
        //     new Vector3( 1.0f , 1.0f,  0.0f),
        //     new Vector3(-1.0f , 1.0f,  0.0f),
        //     new Vector3(-1.0f ,-1.0f,  0.0f),
        //     new Vector3( 1.0f ,-1.0f,  0.0f),
        // };
        // screenQuadMesh.uv = new Vector2[4] {
        //     new Vector2(1, 0),
        //     new Vector2(0, 0),
        //     new Vector2(0, 1),
        //     new Vector2(1, 1)
        // };
        // screenQuadMesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
        
        if (GetComponent<Camera>() == null)
        {
            camera = GetComponent<Camera>();
        }

        // 初始化深度纹理
        depthTexture = new RenderTexture( Screen.width, Screen.height,  16, RenderTextureFormat.Depth);
        depthTexture.Create();

        CommandBuffer cmd = new CommandBuffer();
        cmd.name = "Capture Depth";

        // 获取临时渲染纹理
        int tempID = Shader.PropertyToID("depthBuffer");
        cmd.GetTemporaryRT(tempID,  Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.RFloat);
        cmd.SetRenderTarget(tempID);
        cmd.ClearRenderTarget(true, true, Color.clear, 1f);

        // 绘制场景深度
        // cmd.DrawRenderer(GetComponent<Camera>().GetComponent<Renderer>(), Shader.Find("Hidden/Internal-DepthNormalMap"));

        // 将深度信息复制到深度纹理
        cmd.Blit(tempID, depthTexture);
        cmd.ReleaseTemporaryRT(tempID);
        
        
        commandBuffer = new CommandBuffer();
        commandBuffer.name = "Render";
        
        
        foreach (var camera in renderCameras) {
            // UpdateCommandBuffer(camera);
            camera.depthTextureMode |= DepthTextureMode.Depth;

            camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, commandBuffer);
            // camera.depthTextureMode |= DepthTextureMode.Depth;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void UpdateCommandBuffer(Camera camera) {
        
        commandBuffer.Clear();

        int[] worldPosBufferIds = new int[] {
            Shader.PropertyToID("worldPosBuffer1"),
            Shader.PropertyToID("worldPosBuffer2")
        };

        commandBuffer.GetTemporaryRT(worldPosBufferIds[0], Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.ARGBFloat);
        commandBuffer.GetTemporaryRT(worldPosBufferIds[1], Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.ARGBFloat);

        int depthId = Shader.PropertyToID("depthBuffer");
        commandBuffer.GetTemporaryRT(depthId, Screen.width, Screen.height, 32, FilterMode.Point, RenderTextureFormat.Depth);

        commandBuffer.SetRenderTarget((RenderTargetIdentifier)worldPosBufferIds[0], (RenderTargetIdentifier)depthId);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);

        commandBuffer.DrawMeshInstancedIndirect(
            sphereMesh,
            0,  // submeshIndex
            renderMat,
            0,  // shaderPass
            sphereInstancedArgsBuffer
        );

        int depth2Id = Shader.PropertyToID("depth2Buffer");
        commandBuffer.GetTemporaryRT(depth2Id, Screen.width, Screen.height, 32, FilterMode.Point, RenderTextureFormat.Depth);

        commandBuffer.SetRenderTarget((RenderTargetIdentifier)worldPosBufferIds[0], (RenderTargetIdentifier)depth2Id);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);

        commandBuffer.SetGlobalTexture("depthBuffer", depthId);

        commandBuffer.DrawMesh(
            screenQuadMesh,
            Matrix4x4.identity,
            renderMat,
            0, // submeshIndex
            1  // shaderPass
        );

        int normalBufferId = Shader.PropertyToID("normalBuffer");
        commandBuffer.GetTemporaryRT(normalBufferId, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.ARGBHalf);
        
        int colorBufferId = Shader.PropertyToID("colorBuffer");
        commandBuffer.GetTemporaryRT(colorBufferId, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.RGHalf);

        commandBuffer.SetRenderTarget(new RenderTargetIdentifier[] { normalBufferId, colorBufferId }, (RenderTargetIdentifier)depth2Id);
        commandBuffer.ClearRenderTarget(false, true, Color.clear);

        commandBuffer.SetGlobalTexture("worldPosBuffer", worldPosBufferIds[0]);

        commandBuffer.DrawMeshInstancedIndirect(
            sphereMesh,
            0,  // submeshIndex
            renderMat,
            2,  // shaderPass
            sphereInstancedArgsBuffer
        );
        
        // draw thickness
        int thicknessBufferId = Shader.PropertyToID("thicknessBuffer");
        commandBuffer.GetTemporaryRT(thicknessBufferId, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.RHalf);
        commandBuffer.SetRenderTarget((RenderTargetIdentifier)thicknessBufferId, (RenderTargetIdentifier)depth2Id);
        commandBuffer.ClearRenderTarget(true, true, Color.clear);
        commandBuffer.DrawMeshInstancedIndirect(
            sphereMesh,
            0,  // submeshIndex
            renderMat,
            4,  // shaderPass
            sphereInstancedArgsBuffer
        );
        commandBuffer.SetGlobalTexture("thicknessBuffer", thicknessBufferId);
        commandBuffer.SetGlobalTexture("normalBuffer", normalBufferId);
        commandBuffer.SetGlobalTexture("colorBuffer", colorBufferId);

        commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);

        commandBuffer.DrawMesh(
            screenQuadMesh,
            Matrix4x4.identity,
            renderMat,
            0, // submeshIndex
            3  // shaderPass
        );
    }
}
