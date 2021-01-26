using UnityEngine;
using UnityEngine.UI;

public class RawImageVirtualWebCamTexture : MonoBehaviour
{
    private RawImage rawimage;

    void Start()
    {
        GameObject obj = GameObject.Find("RawImageTransform");
        rawimage = obj.GetComponent<RawImage>();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (source != null)
        {
            rawimage.texture = source;
        }
        Graphics.Blit(source, destination);
    }
}