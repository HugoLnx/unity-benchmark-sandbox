using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWorld : MonoBehaviour
{
    GameObject world;
    Vector2 lastPoint = Vector2.zero;
    Texture2D texture; //Texture should be set to Read/Write and RGBA32bit in Inspector with no mipmaps
    private Renderer _worldRenderer;
    private Texture2D _mainTexture;
    private Texture2D _sharedTexture;
    private Camera _camera;
    private HashSet<Vector2Int> _editedPixels = new();

    private Texture2D Texture
    {
        get => _mainTexture ??= _worldRenderer.material.mainTexture as Texture2D;
        set => _worldRenderer.material.mainTexture = value;
    }
    private Texture2D SharedTexture
    {
        get => _sharedTexture ??= _worldRenderer.material.mainTexture as Texture2D;
        set => _worldRenderer.material.mainTexture = value;
    }
    private Texture2D WorldTexture
    {
        get => Texture;
        set => Texture = value;
    }

    private Camera Cam => _camera ??= Camera.main;

    private void Awake()
    {
        world = GameObject.Find("World");
        _worldRenderer = world.GetComponent<Renderer>();
        texture = Instantiate(WorldTexture) as Texture2D;
        WorldTexture = texture;
    }

    private void SetBlankPixel(Texture2D texture, int x, int y, bool isEditing = true)
    {
        texture.SetPixel(x, y, Color.white);
        if (isEditing) AddAffectedPixelsAround(x, y);
    }

    private void SetRoadPixel(Texture2D texture, int x, int y)
    {
        texture.SetPixel(x, y, Color.black);
        AddAffectedPixelsAround(x, y);
    }

    private void AddAffectedPixelsAround(int x, int y)
    {
        for (int ny = -1; ny < 2; ny++)
        {
            for (int nx = -1; nx < 2; nx++)
            {
                _editedPixels.Add(new Vector2Int(x + nx, y + ny));
            }
        }
    }

    private void SetMagentaPixel(Texture2D texture, int x, int y)
    {
        texture.SetPixel(x, y, Color.magenta);
    }

    private void SetGreenPixel(Texture2D texture, int x, int y)
    {
        texture.SetPixel(x, y, Color.green);
    }

    private void SetBluePixel(Texture2D texture, int x, int y)
    {
        texture.SetPixel(x, y, Color.blue);
    }
    private void SetYellowPixel(Texture2D texture, int x, int y)
    {
        texture.SetPixel(x, y, Color.yellow);
    }

    void Start()
    {
        Application.targetFrameRate = 150;

        //add yellow cells using perlin noise to create some patches of "minerals"
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                if (Mathf.PerlinNoise(x/20.0f,y/20.0f) * 100 < 40)
                    SetYellowPixel(texture, x, y);
            }
        }

        ApplyTexture();
    }

    int CountNeighbourColor(int x, int y, Color col, Texture2D texture)
    {
        int count = 0;
        //loop through all 8 neighbouring cells and if their colour
        //is the one passed through then count it
        for (int ny = -1; ny < 2; ny++)
        {
            for (int nx = -1; nx < 2; nx++)
            {
                if (ny == 0 && nx == 0) continue; //ignore cell you are looking at neighbours
                if (texture.GetPixel(x + nx, y + ny) == col)
                    count++;
            }
        }
        return count;
    }

    void SimulateWorld(Texture2D texture)
    {
        //for (int y = 0; y < texture.height; y++)
        //{
        //    for (int x = 0; x < texture.width; x++)
        //    {
        foreach (var pixel in _editedPixels)
        {
                var x = pixel.x;
                var y = pixel.y;
                //if a cell has more than 4 black neighbours make it blue
                //Commercial Property
                int blackNeighbourCount = CountNeighbourColor(x, y, Color.black, texture);
                Color pixelColor = texture.GetPixel(x, y);
                if (blackNeighbourCount > 4 )
                {
                    SetBluePixel(texture, x, y);
                }
                //if a cell has a black neighbour and is not black itself
                //set to green
                //Residential Property
                else if (blackNeighbourCount > 0 && pixelColor == Color.white)
                {
                    SetGreenPixel(texture, x, y);
                }
                //if near a black cell but the cell is already yellow
                //Mining Property
                if (blackNeighbourCount > 0 && pixelColor == Color.yellow)
                {
                    SetMagentaPixel(texture, x, y);
                }

                //if a cell is blue, green or magenta and has no black next to it then it should die = turn white)
                //if road is taken away the cell should die/deallocate property
                
                if (
                    blackNeighbourCount == 0
                    && (
                        pixelColor == Color.green
                        || pixelColor == Color.blue
                        || pixelColor == Color.magenta
                    )
                )
                {
                    SetBlankPixel(texture, x, y, isEditing: false);
                }
        //    }
        //}
        }
        ApplyTexture();
    }

    void Update()
    {
        _editedPixels.Clear();
        //record start of mouse drawing (or erasing) to get the first position the mouse touches down
        RaycastHit ray;
        bool mouseIsOver = Physics.Raycast(Cam.ScreenPointToRay(Input.mousePosition), out ray);
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) )
        {
            if (mouseIsOver)
            {
                lastPoint = new Vector2((int)(ray.textureCoord.x * texture.width),
                                        (int)(ray.textureCoord.y * texture.height));
            }
        }
        //draw a line between the last known location of the mouse and the current location
        if (Input.GetMouseButton(0))
        {
            if (mouseIsOver)
            {
                // _worldRenderer.material.mainTexture = texture;

                DrawPixelLine((int)(ray.textureCoord.x * texture.width),
                                   (int)(ray.textureCoord.y * texture.height),
                                   (int)lastPoint.x, (int)lastPoint.y, texture);
                ApplyTexture();
                lastPoint = new Vector2((int)(ray.textureCoord.x * texture.width),
                                   (int)(ray.textureCoord.y * texture.height));
            }
        }

        if (Input.GetMouseButton(1))
        {
            if (mouseIsOver)
            {
                // _worldRenderer.material.mainTexture = texture;
                SetBlankPixel(texture, (int)(ray.textureCoord.x * texture.width),
                                   (int)(ray.textureCoord.y * texture.height));
                ApplyTexture();
            }
        }

        SimulateWorld(texture);
    }

    //Draw a pixel by pixel line between two points
    //For more information on the algorithm see: https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
    //DO NOT MODIFY OR OPTMISE
    void DrawPixelLine(int x, int y, int x2, int y2, Texture2D texture)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            SetRoadPixel(texture, x, y);
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
        ApplyTexture();
    }

    private void ApplyTexture()
    {
        texture.Apply();
    }
}
