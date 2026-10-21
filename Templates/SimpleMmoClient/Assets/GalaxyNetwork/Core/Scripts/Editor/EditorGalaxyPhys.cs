#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using GalaxyCoreCommon.Physics;
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
                foreach (var item in FindObjectsByType<GalaxyColliderTerrain>(FindObjectsSortMode.InstanceID))
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
            foreach (var item in FindObjectsByType<BoxCollider>(FindObjectsSortMode.InstanceID))
            {
                if (item.GetComponent<GalaxyColliderBox>()) continue;
                if (!item.gameObject.isStatic) continue;
                if (item.isTrigger != triggers) continue;
                GalaxyColliderBox box = Undo.AddComponent<GalaxyColliderBox>(item.gameObject);
                if (item.isTrigger) box.IsTrigger = true;
            }
        }


        static void AddSphereColliders(bool triggers)
        {
            foreach (var item in FindObjectsByType<SphereCollider>(FindObjectsSortMode.InstanceID))
            {
                if (item.GetComponent<GalaxyColliderSphere>()) continue;
                if (!item.gameObject.isStatic) continue;
                if (item.isTrigger != triggers) continue;
                Undo.AddComponent<GalaxyColliderSphere>(item.gameObject);
            }
        }


        static void RemoveBoxColliders(bool triggers)
        {
            foreach (var item in FindObjectsByType<GalaxyColliderBox>(FindObjectsSortMode.InstanceID))
            {
                if (item.IsTrigger != triggers) continue;
                Undo.DestroyObjectImmediate(item);
            }
        }
        static void RemoveSphereColliders()
        {
            foreach (var item in FindObjectsByType<GalaxyColliderSphere>(FindObjectsSortMode.InstanceID))
            {
                Undo.DestroyObjectImmediate(item);
            }

        }

        static void Bake()
        {
            string name = SceneManager.GetActiveScene().name;
            var path = EditorUtility.SaveFilePanel(
                "Bake colliders",
                "",
                name,
                "gphb");
            if (path.Length == 0) return;


            var bake = BakeLoadedScenes();
            File.WriteAllBytes(path, GalaxyPhysicsBakeFormat.Save(bake));
            Debug.Log("Galaxy GPHB bake saved: " + path + " (" + bake.Colliders.Count + " colliders)");
        }

        public static GalaxyPhysicsBake BakeLoadedScenes()
        {
            var bake = GalaxyPhysicsBakeUtility.CreateBake();
            foreach (var item in FindObjectsByType<GalaxyColliderBox>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
                item.Bake(bake);
            foreach (var item in FindObjectsByType<GalaxyColliderSphere>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
                item.Bake(bake);
            foreach (var item in FindObjectsByType<GalaxyColliderTerrain>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID))
                item.Bake(bake);
            return bake;
        }

    }
}
#endif
