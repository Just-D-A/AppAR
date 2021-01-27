using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButterflyRendere : MonoBehaviour
{
    public RawImage image;
    private RenderTexture _texture;


    private void Start()
    {
        _texture = new RenderTexture(500, 500, 24) { format = RenderTextureFormat.ARGB32 };
        Camera.main.targetTexture = _texture;
    }

    private void Update()
    {
        image.texture = _texture;
    }

    private void OnDestroy()
    {
        _texture.Release();
    }
}
