using GalaxyCoreCommon;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EditorGalaxyPhys : EditorWindow
{
    /// <summary>
    /// Показывать ли террейн
    /// </summary>
    bool showTerrains = true;
    [MenuItem("Galaxy Network / Physics")]
    public static void ShowWindow()
    {
        EditorGalaxyPhys wnd = (EditorGalaxyPhys)EditorWindow.GetWindow(typeof(EditorGalaxyPhys));
        wnd.titleContent.text = "Galaxy Physics";
        wnd.titleContent.tooltip = "Сетевая физика";
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
        if (GUILayout.Button("Remove", EditorStyles.miniButtonMid)) RemoveSphereColliders(false);
        GUILayout.EndHorizontal();

        showTerrains = EditorGUILayout.Foldout(showTerrains, "Terrains", EditorStyles.foldoutHeader);
        if (showTerrains)
        {
            foreach (var item in FindObjectsOfType<GalaxyColliderTerrain>())
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(item.name);
                EditorGUILayout.LabelField("Quality:" + item.quality.ToString());

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
            if (item.isTrigger) box.isTrigger = true;
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
            if (item.isTrigger != triggers) continue;
            DestroyImmediate(item);
        }
    }
    static void RemoveSphereColliders(bool triggers)
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
        bakeData.boxColliders = new List<PhysBakeBoxCollider>();
        foreach (var item in FindObjectsOfType<GalaxyColliderBox>())
        {
            bakeData.boxColliders.Add(item.Bake());
        }
        bakeData.sphereColliders = new List<PhysSphereCollider>();
        foreach (var item in FindObjectsOfType<GalaxyColliderSphere>())
        {
            bakeData.sphereColliders.Add(item.Bake());
        }

        bakeData.terrainColliders = new List<PhysTerrain>();
        foreach (var item in FindObjectsOfType<GalaxyColliderTerrain>())
        {
            Texture2D map = (Texture2D)item.GetImage();
            TextureScale.Bilinear(map, (int)item.quality, (int)item.quality);
            bakeData.terrainColliders.Add(item.Bake(map));
        }


        File.WriteAllBytes(path, bakeData.Serialize());

    }

}

