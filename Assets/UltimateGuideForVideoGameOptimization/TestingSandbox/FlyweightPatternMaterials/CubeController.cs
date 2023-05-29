using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material = new Material(_renderer.material.shader);
    }

    public void SetMaterialColor(Color color)
    {
        _renderer.material.color = color;
    }
    public void SetSharedMaterialColor(Color color)
    {
        _renderer.sharedMaterial.color = color;
    }
}