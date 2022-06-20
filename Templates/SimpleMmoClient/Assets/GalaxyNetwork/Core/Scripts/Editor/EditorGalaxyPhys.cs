#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using GalaxyCoreCommon;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GalaxyNetwork.Core.Scripts.Editor
{
    public class EditorGalaxyPhys : EditorWindow
    {
        /// <summary>
        /// Показывать ли террейн
        /// </summary>
        bool _showTerrains = true;
        [MenuItem("Galaxy Network / Physics")]
        public static void ShowWindow()
        {
            var wnd = (EditorGalaxyPhys)EditorWindow.GetWindow(typeof(EditorGalaxyPhys));
            wnd.titleContent.text = "Galaxy Physics";
            wnd.titleContent.tooltip = "Galaxy Physics";
        }

        void OnGUI()
        {
            GUILayout.Label("Управление сетевой физикой", EditorStyles.boldLabel);
            GUILayout.Label("Box colliders", EditorStyles.foldoutHeader);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Collisers");
            if (GUILayout.Button("Add", EditorStyles.miniButtonMid)) AddBoxColliders(false);
            if (GUILayout.Button("Remove", EditorStyles.miniButtonMid)) RemoveBoxColliders(false);
            GUILayout.EndHorizontal();

            GUILayout.Label("Sphere colliders", EditorStyles.foldoutHeader);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Collisers");
            if (GUILayout.Button("Add", EditorStyles.miniButtonMid)) AddSphereColliders(false);
            if (GUILayout.Button("Remove", EditorStyles.miniButtonMid)) RemoveSphereColliders();
            GUILayout.EndHorizontal();

            _showTerrains = EditorGUILayout.Foldout(_showTerrains, "Terrains", EditorStyles.foldoutHeader);
            if (_showTerrains)
            {
                foreach (var item in FindObjectsOfType<GalaxyColliderTerrain>())
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(item.name);
                    EditorGUILayout.LabelField("Quality:" + item.Quality.ToString());

                    GUILayout.EndHorizontal();
                }

            }
            GUILayout.Space(20);
            if (GUILayout.Button("Запечь физику сцены в файл")) Bake();
        }

        static void AddBoxColliders(bool triggers)
        {
            foreach (var item in FindObjectsOfType<BoxCollider>())
            {
                if (item.GetComponent<GalaxyColliderBox>()) continue;
                if (!item.gameObject.isStatic) continue;
                if (item.isTrigger != triggers) continue;
                GalaxyColliderBox box = item.gameObject.AddComponent<GalaxyColliderBox>();
                if (item.isTrigger) box.IsTrigger = true;
            }
        }


        static void AddSphereColliders(bool triggers)
        {
            foreach (var item in FindObjectsOfType<SphereCollider>())
            {
                if (item.GetComponent<GalaxyColliderSphere>()) continue;
                if (!item.gameObject.isStatic) continue;
                if (item.isTrigger != triggers) continue;
                item.gameObject.AddComponent<GalaxyColliderSphere>();
            }
        }


        static void RemoveBoxColliders(bool triggers)
        {
            foreach (var item in FindObjectsOfType<GalaxyColliderBox>())
            {
                if (item.IsTrigger != triggers) continue;
                DestroyImmediate(item);
            }
        }
        static void RemoveSphereColliders()
        {
            foreach (var item in FindObjectsOfType<GalaxyColliderSphere>())
            {
                DestroyImmediate(item);
            }

        }

        static void Bake()
        {
            string name = SceneManager.GetActiveScene().name;
            var path = EditorUtility.SaveFilePanel(
                "Bake colliders",
                "",
                name,
                "phys");
            if (path.Length == 0) return;


            PhysBake bakeData = new PhysBake();
            bakeData.BoxColliders = new List<PhysBakeBoxCollider>();
            foreach (var item in FindObjectsOfType<GalaxyColliderBox>())
            {
                bakeData.BoxColliders.Add(item.Bake());
            }
            bakeData.SphereColliders = new List<PhysSphereCollider>();
            foreach (var item in FindObjectsOfType<GalaxyColliderSphere>())
            {
                bakeData.SphereColliders.Add(item.Bake());
            }

            bakeData.TerrainColliders = new List<PhysTerrain>();
            foreach (var item in FindObjectsOfType<GalaxyColliderTerrain>())
            {
                Texture2D map = (Texture2D)item.GetImage();
                TextureScale.Bilinear(map, (int)item.Quality, (int)item.Quality);
                bakeData.TerrainColliders.Add(item.Bake(map));
            }


            File.WriteAllBytes(path, bakeData.Serialize());

        }

    }
}
#endif