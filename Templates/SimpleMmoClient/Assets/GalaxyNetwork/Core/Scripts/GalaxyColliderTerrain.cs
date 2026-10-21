using GalaxyCoreCommon.Physics;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    [RequireComponent(typeof(Terrain), typeof(TerrainCollider))]
    public class GalaxyColliderTerrain : MonoBehaviour
    {
        public BakeTerrainQuality Quality = BakeTerrainQuality.normal;
        public string PhysTag = "";

        public GalaxyPhysicsBake Bake()
        {
            var bake = GalaxyPhysicsBakeUtility.CreateBake();
            Bake(bake);
            return bake;
        }

        public void Bake(GalaxyPhysicsBake bake)
        {
            var terrain = GetComponent<Terrain>();
            var data = terrain.terrainData;
            var source = GetComponent<TerrainCollider>();
            var scale = GalaxyPhysicsBakeUtility.AbsoluteScale(transform);
            int resolution = Mathf.Max(2, (int)Quality);
            var heights = new float[resolution * resolution];
            for (int z = 0; z < resolution; z++)
                for (int x = 0; x < resolution; x++)
                    heights[z * resolution + x] = data.GetInterpolatedHeight(
                        x / (float)(resolution - 1), z / (float)(resolution - 1)) * scale.y;
            uint id = (uint)bake.Shapes.Count + 1;
            GalaxyPhysicsGeometryCodec.AddHeightField(bake, id, resolution, resolution, heights,
                Vector3.Scale(data.size, scale).NetworkVector3());
            GalaxyPhysicsBakeUtility.AddCollider(bake, source, id, transform.position,
                transform.rotation, PhysTag, source.isTrigger);
        }

        // Retained for callers that preview the terrain height image in editor tools.
        public Texture GetImage()
        {
            var data = GetComponent<Terrain>().terrainData;
            int resolution = Mathf.Max(2, (int)Quality);
            var image = new Texture2D(resolution, resolution, TextureFormat.RFloat, false);
            for (int z = 0; z < resolution; z++)
                for (int x = 0; x < resolution; x++)
                {
                    float h = data.GetInterpolatedHeight(x / (float)(resolution - 1),
                        z / (float)(resolution - 1)) / Mathf.Max(data.size.y, Mathf.Epsilon);
                    image.SetPixel(x, z, new Color(h, h, h, 1));
                }
            image.Apply();
            return image;
        }
    }

    public enum BakeTerrainQuality
    {
        shit = 28, veryLow = 32, low = 48, normal = 64, good = 80, great = 100, perfect = 128
    }
}
