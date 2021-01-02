using System.Collections.Generic;
using System.Linq;
using GalaxyCoreCommon.Navigation;
using UnityEngine;
public class GalaxyMapBakerCore
{
    public List<GalaxyNode> RayCastZone(Vector3 position, Vector3 range, float cellSize, float heightMin, GalaxyMapBaker baker,int segX,int segZ)
    {
        List<GalaxyNode> points = new List<GalaxyNode>();
        Dictionary<byte, bool> transperents = new Dictionary<byte, bool>();
        foreach (var item in baker.map.layers.list)
        {
            transperents.Add(item.id, item.transperent);
        }
        transperents.Add(0, true);
        int xMax = (int)(range.x / cellSize);
        int zMax = (int)(range.z / cellSize);
        Vector3 rayPos;
        RaycastHit[] hits;
        GalaxyMapLayer mapLayer;
        rayPos.y = range.y + position.y;
        for (ushort x = 0; x < xMax; x++)
        {
            rayPos.x = position.x + x * cellSize;
            for (ushort z = 0; z < zMax; z++)
            {
                rayPos.z = position.z + z * cellSize;
                hits = Physics.RaycastAll(rayPos, Vector3.down, range.y);
                float lastY = float.MaxValue;
                foreach (var item in hits.OrderByDescending(s => s.point.y))
                {
                    if (lastY - item.point.y < heightMin) continue;
                    lastY = item.point.y;
                    GalaxyNode node = new GalaxyNode();
                    node.indexX = (ushort)(x + (xMax*segX));
                    node.indexZ = (ushort)(z + (zMax * segZ));
                    node.x = item.point.x;
                    node.y = item.point.y;
                    node.z = item.point.z;
                    node.layer = baker.GetLayer(item.transform.gameObject);
                     
                    if (node.layer != 0)
                    {
                        mapLayer = baker.map.layers.GetById(node.layer);
                        if (mapLayer.exscind) break;
                        if (mapLayer.excludeGraph) continue;
                    }
                  
                    points.Add(node);
                    if (!transperents[node.layer]) break;
                }
            }
        }
        return points;
    }
    public static void DrawZones(Vector3 position, Vector3 mapSize, float cellSize)
    {
        if (cellSize < 0.5f) return;
        Vector3 center = position;
        float sectorSize;
        center.x += mapSize.x / 2;
        center.y += mapSize.y / 2;
        center.z += mapSize.z / 2;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, mapSize + Vector3.up * 2f);
        sectorSize = cellSize * 100;
        float mapX = (mapSize.x / sectorSize);
        float mapZ = (mapSize.z / sectorSize);
        Gizmos.color = Color.red;
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                Vector3 pos = position;
                pos.x = pos.x + ((mapSize.x / mapX) * x);
                pos.z = pos.z + ((mapSize.z / mapZ) * z);
                Vector3 dSize = new Vector3(sectorSize, mapSize.y, sectorSize);
                Gizmos.DrawWireCube(pos + (dSize / 2), dSize);
            }
        }
    }

    public static void DrawCells(List<GalaxyNode> nodes, float cellSize, GalaxyMapLayerManager layers, Vector3 pos)
    {
        float sizeIndex = cellSize;
        cellSize = cellSize * 0.9f;
        if (nodes == null) return;       

            foreach (var node in nodes)
            {
            if (node.parent != null) continue;
            GalaxyMapLayer layer = layers.GetById(node.layer);
            Gizmos.color = Color.red;
            if (layer != null)
            {
                Gizmos.color = new Color32(layer.r, layer.g, layer.b, 250);
            }
         
            Vector3 size = new Vector3(node.size * cellSize, 0.2f, node.size * cellSize);
            Gizmos.DrawCube(new Vector3(pos.x+(node.indexX * sizeIndex), node.y + 0.2f, pos.z+ (node.indexZ* sizeIndex)), size);
       
        }        
    }
    public static void DrawLinks(List<GalaxyNode> nodes, float cellSize, GalaxyMapLayerManager layers)
    {         
        if (nodes == null) return;

        foreach (var node in nodes)
        {
            if (node.parent != null) continue;
            GalaxyMapLayer layer = layers.GetById(node.layer);
            Gizmos.color = Color.red;
            if (layer != null)
            {
                Gizmos.color = new Color32(layer.r, layer.g, layer.b, 250);
            }
          
            foreach (var item in node.links)
            {
                Gizmos.DrawLine(new Vector3(node.x, node.y + 0.2f, node.z), new Vector3(item.x, item.y + 0.2f, item.z));

            }

        }
    }
}