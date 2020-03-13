#if UNITY_EDITOR
using GalaxyCoreCommon;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GalaxyColliderTerrain : MonoBehaviour
{

    Terrain terrain;
    public BakeTerrainQuality quality = BakeTerrainQuality.normal;
    public string physTag = "";
    private Texture2D img;

    public PhysTerrain Bake()
    {
        PhysTerrain bake = new PhysTerrain();
        terrain = GetComponent<Terrain>();
        var data = terrain.terrainData;
        int resolusion = data.heightmapResolution;
        var heights = data.GetHeights(0, 0, resolusion, resolusion);
        float verticalScale = data.size.y;
        for (int x = 0; x < resolusion; x++)
        {
            for (int z = 0; z < resolusion; z++)
                heights[x, z] *= verticalScale;
        }
        for (int x = 0; x < resolusion - 1; x++)
        {
            for (int z = x; z < resolusion; z++)
            {
                float h1 = heights[x, z];
                float h2 = heights[z, x];
                heights[x, z] = h2;
                heights[z, x] = h1;
            }
        }
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, heights);
        bake.heights = ms.ToArray();
        bake.z = data.size.z;
        bake.x = data.size.x;
        bake.resolusion = resolusion;
        return bake;
    }

    public PhysTerrain Bake(Texture2D map)
    {
        PhysTerrain bake = new PhysTerrain();
        terrain = GetComponent<Terrain>();
        var data = terrain.terrainData;
        int resolusion = map.width;
        //     var heights = data.GetHeights(0, 0, map.width, map.height);
        float[,] heights = new float[map.width, map.height];
        float verticalScale = data.size.y;
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                heights[x, y] = map.GetPixel(x, y).r * verticalScale;
                //     img.SetPixel(x, y, Color.Lerp(Color.black, Color.white, heights[x, y]));                
            }
            //     Debug.Log(heights[x, 5]* verticalScale);
        }
        for (int x = 0; x < resolusion - 1; x++)
        {
            for (int z = x; z < resolusion; z++)
            {
                float h1 = heights[x, z];
                float h2 = heights[z, x];
                heights[x, z] = h2;
                heights[z, x] = h1;
            }
        }
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, heights);
        bake.heights = ms.ToArray();
        bake.z = data.size.z;
        bake.x = data.size.x;
        bake.resolusion = map.width;
        bake.tag = physTag;
        if (physTag == "") bake.tag = transform.name;
        bake.position = transform.position.NetworkVector3();
        return bake;
    }

    public Texture GetImage()
    {
        //   if(img) return img;

        if (!terrain) terrain = GetComponent<Terrain>();
        var data = terrain.terrainData;
        int resolusion = data.heightmapResolution;
        var heights = data.GetHeights(0, 0, resolusion, resolusion);
        //   return TextureFromHeightMap(heights);
        // return terrain.terrainData.heightmapTexture;
        img = new Texture2D(terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution, TextureFormat.RGBA32, false);


        float verticalScale = data.size.y;
        //  terrain.terrainData.heightmapTexture
        /*
          for (int x = 0; x < resolusion; x++)
          {
              for (int z = 0; z < resolusion; z++)
                  img.SetPixel(0, 0, new Color(heights[x, z], heights[x, z], heights[x, z]));
            //  heights[x, z] *= verticalScale;
          }
         */

        for (int x = 0; x < img.width; x++)
        {
            for (int y = 0; y < img.height; y++)
            {
                img.SetPixel(x, y, new Color(heights[x, y], heights[x, y], heights[x, y], 1));
                //     img.SetPixel(x, y, Color.Lerp(Color.black, Color.white, heights[x, y]));                
            }
            //     Debug.Log(heights[x, 5]* verticalScale);
        }
        img.Apply();
        return img;
    }
    public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        return texture;
    }
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y] * 5);
            }
        }
        return TextureFromColourMap(colourMap, width, height);
    }
}
public enum BakeTerrainQuality
{

    shit = 28,
    veryLow = 32,
    low = 48,
    normal = 64,
    good = 80,
    great = 100,
    perfect = 128 
}
#endif