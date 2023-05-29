using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeChangeColors : MonoBehaviour
{
    private CubeController[] _cubes;

    private void Awake()
    {
        _cubes = GetComponentsInChildren<CubeController>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            foreach (var cube in _cubes)
            {
                cube.SetMaterialColor(RandomColor());
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            foreach (var cube in _cubes)
            {
                cube.SetSharedMaterialColor(RandomColor());
            }
        }
    }

    private Color RandomColor()
    {
        return Random.ColorHSV();
    }
}
