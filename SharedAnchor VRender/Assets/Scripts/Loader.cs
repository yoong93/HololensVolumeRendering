// loads the raw binary data into a texture saved as a Unity asset 
// (so can be de-activated after a given data cube has been converted)
// adapted from a XNA project by Kyle Hayward 
// http://graphicsrunner.blogspot.ca/2009/01/volume-rendering-101.html
// Gilles Ferrand, University of Manitoba / RIKEN, 2016–2017

#if UNITY_EDITOR
using UnityEditor;
#endif
using HoloToolkit.Unity;
using Vuforia;
using UnityEngine;
using System;
using System.IO; // to get BinaryReader
using System.Linq; // to get array's Min/Max

public class Loader : Singleton<Loader>
{

    [Header("Drag all the textures in here")]
    [SerializeField]
    private Texture2D[] slices;
    [SerializeField]
    [Range(0, 2)]
    private float opacity = 1;
    [Header("Volume texture size. These must be a power of 2")]
    [SerializeField]
    private int volumeWidth  = 256;
    [SerializeField]
    private int volumeHeight = 256;
    [SerializeField]
    private int volumeDepth = 256;
    public bool mipmap;
    private Texture3D _texture;

    public void StartVRendering() {        
        _texture = new Texture3D(volumeWidth, volumeHeight, volumeDepth, TextureFormat.Alpha8, mipmap);
        GenerateVolumeTexture();
        Shader volumerendering = Shader.Find("Custom/Ray Casting");        
        GetComponent<Renderer>().material.shader = volumerendering;
        //Debug.Log("Shader set : " + volumerendering );
        GetComponent<Renderer>().material.SetTexture("_Data", _texture);
        Debug.Log("texture set");
}

    private void GenerateVolumeTexture()
    {
        Debug.Log("generate volume texture");
        // sort
        Array.Sort(slices, (x, y) => x.name.CompareTo(y.name));

        // skip some slices if we can't fit it all in
        float countOffset = (slices.Length - 1) / (float)volumeDepth;

        Color[] volumeColors = new Color[volumeWidth * volumeHeight * volumeDepth];
        int sliceCount = 0;
        float sliceCountFloat = 0f;
        Color color = Color.black;
        for (int z = 0; z < volumeDepth; z++)
        {
            sliceCountFloat += countOffset;
            sliceCount = Mathf.FloorToInt(sliceCountFloat);
            
            for (int x = 0; x < volumeWidth; x++)
            {
                for (int y = 0; y < volumeHeight; y++)
                {
                    int idx = x + (y * volumeWidth) + (z * (volumeWidth * volumeHeight));
                    volumeColors[idx] = slices[sliceCount].GetPixelBilinear(x / (float)volumeWidth, y / (float)volumeHeight);
                    volumeColors[idx].a *= volumeColors[idx].r;


                }
            }
        }
        _texture.SetPixels(volumeColors);
        _texture.Apply();
        Debug.Log("texture create finished");
    }
}
