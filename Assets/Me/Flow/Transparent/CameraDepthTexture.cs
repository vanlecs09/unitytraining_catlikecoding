using UnityEngine;

public class CameraDepthTexture : MonoBehaviour
{
    // private void Awake()
    // {
    //     Camera.main.depthTextureMode = DepthTextureMode.Depth;
    // }

    private void OnEnable()
    {
        Camera.main.depthTextureMode = Camera.main.depthTextureMode | DepthTextureMode.Depth;
        // Camera.main.SetReplacementShader(depth, "");
    }
}