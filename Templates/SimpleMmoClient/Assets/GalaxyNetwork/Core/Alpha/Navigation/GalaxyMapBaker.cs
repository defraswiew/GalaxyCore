using System;
using System.Collections.Generic;
using System.IO;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using UnityEngine;
using UnityEngine.Serialization;
using Tools = GalaxyCoreCommon.Navigation.Tools;

namespace GalaxyNetwork.Core.Alpha.Navigation
{
    public class GalaxyMapBaker : MonoBehaviour
    {
        public static GalaxyMap MainMap;
        [FormerlySerializedAs("mapSize")]
        public Vector3 MapSize;
        [FormerlySerializedAs("cellSize")]
        public float CellSize;
        [FormerlySerializedAs("minHeight")]
        public float MinHeight = 2;
        [FormerlySerializedAs("maxHeightLink")]
        public float MaxHeightLink = 0.1f;
        [FormerlySerializedAs("drawCell")]
        public bool DrawCell = true;
        [FormerlySerializedAs("drawLinks")]
        public bool DrawLinks;
        public GalaxyMap Map = new GalaxyMap();
        [FormerlySerializedAs("path")]
        public string Path;
        [Tooltip("Baked map embedded in the player; external Path is used only when this is unassigned.")]
        public TextAsset BakedMap;
        [Tooltip("Small layer definitions for the authoring scene; contains no geometry.")]
        public TextAsset LayerConfiguration;
        [Tooltip("Editor only: bake a missing authoring cache on first Play.")]
        public bool BakeInEditorOnPlay;

        public string ResolveBakePath()
        {
            if (string.IsNullOrEmpty(Path)) return string.Empty;
            return System.IO.Path.IsPathRooted(Path) ? Path :
                System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, "..", Path));
        }

        private void Awake() => LoadLayerConfiguration();

        private void LoadLayerConfiguration()
        {
            if (LayerConfiguration == null) return;
            var configuration = BaseMessage.Deserialize<GalaxyMap>(LayerConfiguration.bytes);
            Map.Layers = configuration.Layers;
        }

        [SerializeField]
        [FormerlySerializedAs("objects")]
        public GameObject[] Objects;
        [SerializeField]
        [FormerlySerializedAs("layers")]
        public byte[] Layers;
        public bool IsNotLayerAssigned => CheckLayers();
        private float _sectorSize;
        private GalaxyMapBakerCore _bakerCore;

        private bool CheckLayers()
        {
            if (Objects == null) return false;
            if (Objects.Length == 0) return false;
            foreach (var layer in Layers)
            {
                if (layer > 0) return true;
            }

            return false;
        }
        void Update()
        {
            Map.Update();
        }
        private void OnValidate()
        {
            if (MapSize.x < 10) MapSize.x = 10;
            if (MapSize.y < 10) MapSize.y = 10;
            if (MapSize.z < 10) MapSize.z = 10;
            if (CellSize < 0.5f) CellSize = 1;
            if (MinHeight < 1) MinHeight = 1;
            if (MaxHeightLink < 0) MaxHeightLink = 0;
        }

        public void SetLayer(GameObject go, byte layer)
        {
            // если массивы пусты пересоздаем
            if (Objects == null)
            {
                Objects = new GameObject[1];
                Layers = new byte[1];
                Objects[0] = go;
                Layers[0] = layer;
                return;
            }
            // получаем индекс
            int index = GetSaveIndex(go);
            // если индекс существует тогда просто обновляем запись
            if (index != -1)
            {
                Layers[index] = layer;
                return;
            }
            // запрашиваем свободный индекс
            index = GetFreeIndex();
            // если свободный нашелся то пишем все туда
            if (index != -1)
            {
                Objects[index] = go;
                Layers[index] = layer;
                return;
            }
            //запрашиваем новый индекс
            index = ResizeArray();
            Objects[index] = go;
            Layers[index] = layer;
        }
        /// <summary>
        /// Ищем есть ли такой объект
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private int GetSaveIndex(GameObject go)
        {
            if (Objects == null) Objects = Array.Empty<GameObject>();
            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] == go)
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
            return Layers[index];
        }

        /// <summary>
        /// ищем свободный индекс
        /// </summary>
        /// <returns></returns>
        private int GetFreeIndex()
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                if (Objects[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
        private int ResizeArray()
        {
            int result = Objects.Length + 1;
            GameObject[] _objects = new GameObject[Objects.Length + 10];
            byte[] _layers = new byte[Objects.Length + 10];
            for (int i = 0; i < Objects.Length; i++)
            {
                _objects[i] = Objects[i];
                _layers[i] = Layers[i];
            }
            Objects = _objects;
            Layers = _layers;
            return result;
        }

        public void Baked()
        {
            uint nodeID = 0;
            Debug.Log("Baked");
            _bakerCore = new GalaxyMapBakerCore();
            var tmpMap = new List<List<GalaxyNode>>();
            _sectorSize = CellSize * 100;
            float mapX = (MapSize.x / _sectorSize);
            float mapZ = (MapSize.z / _sectorSize);
            float step = (mapX * mapZ) * 0.01f;
            int count = 0;

            for (int x = 0; x < mapX; x++)
            {
                for (int z = 0; z < mapZ; z++)
                {
                    var mapSegment = new List<GalaxyNode>();

                    Vector3 pos = transform.position;
                    pos.x = pos.x + ((MapSize.x / mapX) * x);
                    pos.z = pos.z + ((MapSize.z / mapZ) * z);
                    mapSegment = _bakerCore.RayCastZone(pos, new Vector3(_sectorSize, MapSize.y, _sectorSize), CellSize, MinHeight,this,x,z);
                    count += mapSegment.Count;

                    foreach (var item in mapSegment)
                    {
                        nodeID++;
                        item.Id = nodeID;
                    }
                    tmpMap.Add(Tools.MergeNodes(mapSegment,MaxHeightLink));
                }
            }

            tmpMap = Tools.Linked(tmpMap, CellSize, Map.Layers.List);
            Map.SetBakeNodes(tmpMap);
            tmpMap.Clear();
            string output = ResolveBakePath();
            if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("Choose a navigation bake output file first.");
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(output));
            Map.Save(output);
            Debug.Log(count);
        }

        private void OnDrawGizmosSelected()
        {
            GalaxyMapBakerCore.DrawZones(transform.position, MapSize, CellSize);
            if (DrawCell && Map.Nodes != null && Map.Nodes.Count > 0)
            {
                GalaxyMapBakerCore.DrawCells(Map.Nodes, CellSize, Map.Layers, transform.position);
            }

            if (DrawLinks && Map.Nodes != null && Map.Nodes.Count > 0)
            {
                GalaxyMapBakerCore.DrawLinks(Map.Nodes, CellSize, Map.Layers);
            }

            if (DrawCell && Map.NodesArray != null && (Map.Nodes == null || Map.Nodes.Count == 0))
            {
                GalaxyMapBakerCore.DrawCells(Map.NodesArray, CellSize, Map.Layers, transform.position);
            }

            if (DrawLinks && Map.NodesArray != null && (Map.Nodes == null || Map.Nodes.Count == 0))
            {
                GalaxyMapBakerCore.DrawLinks(Map.NodesArray, CellSize, Map.Layers);
            }
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(MainMap, Map)) MainMap = null;
        }

        private void Start()
        {
            Load();
        }

        public void Save()
        {
            string output = ResolveBakePath();
            if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("Choose a navigation bake output file first.");
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(output));
            Map.Save(output);
        }

        public void Load()
        {
            LoadLayerConfiguration();
#if UNITY_EDITOR
            if (LayerConfiguration != null && !Application.isPlaying) return;
            if (BakeInEditorOnPlay && Application.isPlaying && !File.Exists(ResolveBakePath())) Baked();
#endif
            if (BakedMap != null) LoadBytes(BakedMap.bytes);
            else if (!string.IsNullOrEmpty(Path)) Load(Path);
        }

        public void Load(string loadPath)
        {
            Path = loadPath;
            if (string.IsNullOrEmpty(Path))
            {
                return;
            }
            string resolved = ResolveBakePath();
            if (!File.Exists(resolved))
            {
                Debug.LogWarning("Galaxy navigation bake not found: " + Path + ". Assign BakedMap or choose a valid file in the navigation editor.", this);
                return;
            }
            LoadBytes(File.ReadAllBytes(resolved));
        }

        private void LoadBytes(byte[] bytes)
        {
            Map = BaseMessage.Deserialize<GalaxyMap>(bytes);
            Map?.Init(new NavigationConfig
            {
                ParallelWorkerCount = 2,
                QueueLimit = 500,
                UseJumping = false
            });
            MainMap = Map;
        }
    }
}
