using System.Collections.Generic;
using System.IO;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using UnityEngine;
using Tools = GalaxyCoreCommon.Navigation.Tools;

public class GalaxyMapBaker : MonoBehaviour
{
    public Vector3 mapSize;
    public float cellSize;
    public float minHeight = 2;
    public float maxHeightLink = 0.1f;
    private float sectorSize;
    private GalaxyMapBakerCore bakerCore;
    public bool drawCell = false;
    public bool drawLinks = false;
    public GalaxyMap map = new GalaxyMap();
    public string path;

    [SerializeField]
    public GameObject[] objects;
    [SerializeField]
    public byte[] layers;
    public float progress;

    void Update()
    {
        map.Update();
    }

    public void SetLayer(GameObject go, byte layer)
    {
        // если массивы пусты пересоздаем
        if (objects == null)
        {
            objects = new GameObject[1];
            layers = new byte[1];
            objects[0] = go;
            layers[0] = layer;
            return;
        }
        // получаем индекс
        int index = GetSaveIndex(go);
        // если индекс существует тогда просто обновляем запись
        if (index != -1)
        {
            layers[index] = layer;
            return;
        }
        // запрашиваем свободный индекс
        index = GetFreeIndex();
        // если свободный нашелся то пишем все туда
        if (index != -1)
        {
            objects[index] = go;
            layers[index] = layer;
            return;
        }
        //запрашиваем новый индекс
        index = ResizeArray();
        objects[index] = go;
        layers[index] = layer;
    }
    /// <summary>
    /// Ищем есть ли такой объект
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    private int GetSaveIndex(GameObject go)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == go)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Получить слой заданого go
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public byte GetLayer(GameObject go)
    {
        int index = GetSaveIndex(go);
        if (index == -1) return 0;
        return layers[index];
    }

    /// <summary>
    /// ищем свободный индекс
    /// </summary>
    /// <returns></returns>
    private int GetFreeIndex()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
    private int ResizeArray()
    {
        int result = objects.Length + 1;
        GameObject[] _objects = new GameObject[objects.Length + 10];
        byte[] _layers = new byte[objects.Length + 10];
        for (int i = 0; i < objects.Length; i++)
        {
            _objects[i] = objects[i];
            _layers[i] = layers[i];
        }
        objects = _objects;
        layers = _layers;
        return result;
    }



    public void Baked()
    {
        uint nodeID = 0;
        Debug.Log("Baked");
        bakerCore = new GalaxyMapBakerCore();
        var tmpMap = new List<List<GalaxyNode>>();
        sectorSize = cellSize * 100;
        float mapX = (mapSize.x / sectorSize);
        float mapZ = (mapSize.z / sectorSize);
        float step = (mapX * mapZ) * 0.01f;
        progress = 0;
        int count = 0;
        
        for (int x = 0; x < mapX; x++)
        {
            for (int z = 0; z < mapZ; z++)
            {
                List<GalaxyNode> mapSegment = new List<GalaxyNode>();

                Vector3 pos = transform.position;
                pos.x = pos.x + ((mapSize.x / mapX) * x);
                pos.z = pos.z + ((mapSize.z / mapZ) * z);
                mapSegment = bakerCore.RayCastZone(pos, new Vector3(sectorSize, mapSize.y, sectorSize), cellSize, minHeight,this,x,z);
                count += mapSegment.Count;

                foreach (var item in mapSegment)
                {
                    nodeID++;
                    item.id = nodeID;
                }

                tmpMap.Add(Tools.MergeNodes(mapSegment,maxHeightLink));
                progress += step;
            }
        }
       
        /*
        List<GalaxyNodeBaker> mapSegment = new List<GalaxyNodeBaker>();
        mapSegment = bakerCore.RayCastZone(transform.position, mapSize, cellSize, minHeight, this);
        count += mapSegment.Count;

        foreach (var item in mapSegment)
        {
            nodeID++;
            item.id = nodeID;
        }

        tmpMap.Add(Tools.MergeNodes(mapSegment, heightTrashold));
        */

        progress = 0.2f;
        tmpMap = Tools.Linked(tmpMap, cellSize, map.layers.list);
        progress = 0.8f;
        map.SetBakeNodes(tmpMap);
        tmpMap.Clear();


        map.Save(path);
        progress = 0;
        Debug.Log(count);
    }


    void OnDrawGizmosSelected()
    {
        GalaxyMapBakerCore.DrawZones(transform.position, mapSize, cellSize);
        if (drawCell && map.Nodes != null)
        {
            GalaxyMapBakerCore.DrawCells(map.Nodes, cellSize, map.layers,transform.position);
        }
        if (drawLinks && map.Nodes != null)
        {
            GalaxyMapBakerCore.DrawLinks(map.Nodes, cellSize, map.layers);
        }
    }

    void Start()
    {
        map = BaseMessage.Deserialize<GalaxyMap>(File.ReadAllBytes(path));
        map.Init();
    }
}