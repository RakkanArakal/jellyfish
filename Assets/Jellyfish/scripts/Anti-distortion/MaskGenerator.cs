using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskGenerator : MonoBehaviour
{
    public Material maskPass;
    // public Camera mainCamera; 
    public Vector2 eyeOffset;
    public float thick = 1.0f;
    public float bright = 0.4f;

    // public RenderTexture[] delayrenderTextures;
    // public int frameCount = 10;
    private int currentFrame = 0;
    
    void Start()
    {
        // mainCamera.depthTextureMode |= DepthTextureMode.Depth;
        //
        // delayrenderTextures = new RenderTexture[frameCount];
        // for (int i = 0; i < frameCount; i++)
        // {
        //     delayrenderTextures[i] = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 24);
        // }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        maskPass.SetFloat("_Thick", thick);
        maskPass.SetFloat("_Bright", bright);
        maskPass.SetFloat("_EyeOffsetX", eyeOffset.x);
        maskPass.SetFloat("_EyeOffsetY", eyeOffset.y);
        
        Graphics.Blit(src, dest, maskPass, 0);

    }
}
