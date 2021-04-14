using System.Collections.Generic;
using System.Linq;
using GalaxyCoreCommon.Navigation;
using UnityEngine;

namespace GalaxyNetwork.Core.Alpha.Navigation
{
    public class GalaxyMapBakerCore
    {
        public List<GalaxyNode> RayCastZone(Vector3 position, Vector3 range, float cellSize, float heightMin, GalaxyMapBaker baker,int segX,int segZ)
        {
            var points = new List<GalaxyNode>();
            var transparent = new Dictionary<byte, bool>();
            foreach (var item in baker.Map.Layers.List)
            {
                transparent.Add(item.Id, item.Transparent);
            }
            transparent.Add(0, true);
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
                    var lastY = float.MaxValue;
                    foreach (var item in hits.OrderByDescending(s => s.point.y))
                    {
                        if (lastY - item.point.y < heightMin) continue;
                        lastY = item.point.y;
                        var node = new GalaxyNode
                        {
                            IndexX = (ushort) (x + (xMax * segX)),
                            IndexZ = (ushort) (z + (zMax * segZ)),
                            X = item.point.x,
                            Y = item.point.y,
                            Z = item.point.z,
                            Layer = baker.GetLayer(item.transform.gameObject)
                        };

                        if (node.Layer != 0)
                        {
                            mapLayer = baker.Map.Layers.GetById(node.Layer);
                            if (mapLayer.Cut) break;
                            if (mapLayer.ExcludeGraph) continue;
                        }
                  
                        points.Add(node);
                        if (!transparent[node.Layer]) break;
                    }
                }
            }
            return points;
        }
        public static void DrawZones(Vector3 position, Vector3 mapSize, float cellSize)
        {
            if (cellSize < 0.5f) return;
            var center = position;
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
                if (node.Parent != null) continue;
                var layer = layers.GetById(node.Layer);
                Gizmos.color = Color.red;
                if (layer != null)
                {
                    Gizmos.color = new Color32(layer.R, layer.G, layer.B, 250);
                }

                var size = new Vector3(node.size * cellSize, 0.2f, node.size * cellSize);
                Gizmos.DrawCube(
                    new Vector3(pos.x + (node.IndexX * sizeIndex), node.Y + 0.2f, pos.z + (node.IndexZ * sizeIndex)),
                    size);
            }
        }
        
        public static void DrawCells(WorkNode[] nodes, float cellSize, GalaxyMapLayerManager layers, Vector3 pos)
        {
            float sizeIndex = cellSize;
            cellSize = cellSize * 0.9f;
            if (nodes == null) return;

            foreach (var node in nodes)
            {
                if(node == null) continue;
                if (node.Parent > 0) continue;
                var layer = layers.GetById(node.Layer);
                Gizmos.color = Color.red;
                if (layer != null)
                {
                    Gizmos.color = new Color32(layer.R, layer.G, layer.B, 250);
                }

                var size = new Vector3(node.Size * cellSize, 0.2f, node.Size * cellSize);
                Gizmos.DrawCube(
                    new Vector3(pos.x + (node.IndexX * sizeIndex), node.Y + 0.2f, pos.z + (node.IndexZ * sizeIndex)),
                    size);
            }
        }
        public static void DrawLinks(List<GalaxyNode> nodes, float cellSize, GalaxyMapLayerManager layers)
        {         
            if (nodes == null) return;

            foreach (var node in nodes)
            {
                if (node.Parent != null) continue;
                var layer = layers.GetById(node.Layer);
                Gizmos.color = Color.red;
                if (layer != null)
                {
                    Gizmos.color = new Color32(layer.R, layer.G, layer.B, 250);
                }
          
                foreach (var item in node.Links)
                {
                    Gizmos.DrawLine(new Vector3(node.X, node.Y + 0.2f, node.Z), new Vector3(item.X, item.Y + 0.2f, item.Z));
                }

            }
        }
        
        public static void DrawLinks(WorkNode[] nodes, float cellSize, GalaxyMapLayerManager layers)
        {         
            if (nodes == null) return;

            foreach (var node in nodes)
            {
                if(node == null) continue;
                if (node.Parent > 0) continue;
                var layer = layers.GetById(node.Layer);
                Gizmos.color = Color.red;
                if (layer != null)
                {
                    Gizmos.color = new Color32(layer.R, layer.G, layer.B, 250);
                }
                if(node.Links == null) continue;
                foreach (var item in node.Links)
                {
                    Gizmos.DrawLine(new Vector3(node.X, node.Y + 0.2f, node.Z), new Vector3(item.X, item.Y + 0.2f, item.Z));
                }

            }
        }
    }
}