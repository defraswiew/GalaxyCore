using System;
using System.Collections.Generic;
using System.IO;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Localize = GalaxyEditorLocalization;
public class GalaxyMapEditorCommon  
{
    internal void RenderSceneSettings(GalaxyMapBaker baker)
    {
        Selection.activeObject = baker.gameObject;
        Vector3 range = baker.mapSize;
        GUILayout.Label(Localize.Get("scene_settings"));
        GUILayout.BeginHorizontal();
        GUILayout.Label(Localize.Get("range"));
        GUILayout.Label("X:");
        range.x = EditorGUILayout.FloatField(range.x);
        GUILayout.Label("Y:");
        range.y = EditorGUILayout.FloatField(range.y);
        GUILayout.Label("Z:");
        range.z = EditorGUILayout.FloatField(range.z);
        GUILayout.EndHorizontal();
        baker.mapSize = range;
        GUILayout.BeginHorizontal();
        GUILayout.Label(Localize.Get("cell_size"));        
        baker.cellSize = EditorGUILayout.Slider(baker.cellSize, 0.5f, 10);
        baker.cellSize = (float)Math.Round(baker.cellSize, 1);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Minimal Height:");
        baker.minHeight = EditorGUILayout.Slider(baker.minHeight, 1, 100);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Draw cells");
        baker.drawCell = EditorGUILayout.Toggle(baker.drawCell);
        GUILayout.Label("Draw Links");
        baker.drawLinks = EditorGUILayout.Toggle(baker.drawLinks);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("File");
        GUILayout.Label(baker.path);
        if(baker.path!=null && baker.path != "")
        {
            if (GUILayout.Button("Load")) Load(baker);
        }

        if(GUILayout.Button("Open"))
        baker.path = EditorUtility.OpenFilePanel("Open map", "", "");
        GUILayout.EndHorizontal();
    }

    internal void Load(GalaxyMapBaker baker)
    {
        try
        {
            baker.map = BaseMessage.Deserialize<GalaxyMap>(File.ReadAllBytes(baker.path));
            baker.map.Init();
        }
        catch (Exception)
        {

            Debug.LogError("Ошибка чтения файла");
        }
    }

    internal void RenderPath(GalaxyMapBaker baker)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Path:");
        GUILayout.Label(baker.path);
        baker.path = EditorUtility.SaveFilePanel(
          "Save map",
          "",
          "map" + ".gnm",
          "gnm");
        GUILayout.EndHorizontal();
    }


    internal void RenderTabObject(GalaxyMapBaker baker)
    {
        GUILayout.Label("Assigning navigation layers");
        try
        {
            if (Selection.activeTransform.gameObject == null) return;
        }
        catch (System.Exception)
        {
            return;
        }
        GameObject selected = Selection.activeTransform.gameObject;

        var colliders = selected.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            GUILayout.Label("Object does not have a colliders");
            return;
        }

        GUILayout.Label("Layer: " + baker.GetLayer(selected));
        int n = baker.GetLayer(selected);
        string[] options = GetDropList(baker);
        n = EditorGUILayout.Popup("Layer", n, options);
        baker.SetLayer(selected, (byte)n);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(selected);
            EditorSceneManager.MarkSceneDirty(selected.gameObject.scene);
        }
    }
    internal void RenderTabOptions()
    {
        GUILayout.Label(Localize.Get("settings"));
        Localize.DrawSettings();
    }

    private string[] GetDropList(GalaxyMapBaker baker)
    {
        if (baker.map.layers == null) baker.map.layers = new GalaxyMapLayerManager();
        if (baker.map.layers.list == null) baker.map.layers.list = new List<GalaxyMapLayer>();
        GalaxyMapLayer layer = baker.map.layers.GetById(0);
        layer = baker.map.layers.GetById(255);
        string[] options = new string[baker.map.layers.list.Count+1];
        options[0] = "Not Walkable";
     //   options[1] = "Cropping";
        int i = 1;
        foreach (var item in baker.map.layers.list)
        {
            options[i] = item.name;
            i++;
        }      
        return options;
    }



    public void RenderLayers(GalaxyMapEditor window)
    {
   //     mainBox.position = new Vector2(0, 50);
    //    GUILayout.BeginArea(mainBox, skin.window);

        GUILayout.Label("Edit Layers");
        var list = GetDropList(window.baker);
      
        window.currentLayer = EditorGUILayout.Popup("Select layer", window.currentLayer, list);



        if (window.currentLayer != 0) DrawLayer(window.baker.map.layers.GetById((byte)window.currentLayer));

        if (GUILayout.Button("Add new Layer", EditorStyles.miniButtonMid))
        {
            window.currentLayer = window.baker.map.layers.CreateLayer();
        }
        if (GUI.changed)
        {
           // BaPathLayers.Save();
        }
 
    }


    private void DrawLayer(GalaxyMapLayer layer)
    {
        if (layer == null) return;
        var layerBox = new Rect(0, 0, 0, 0);
        layerBox.width = (Screen.width - 5);
        layerBox.height = 50;
        layerBox.position = new Vector2(0, 50);
        //      GUILayout.BeginArea(layerBox, skin.window);
        GUILayout.BeginHorizontal();
        GUILayout.Label("[" + layer.id + "] Name:");
        layer.name = GUILayout.TextField(layer.name);
        GUILayout.EndHorizontal();
        GUILayout.Label("Multiplers:");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Max height linked:");
        layer.heightWalkable = EditorGUILayout.FloatField(layer.heightWalkable);
        GUILayout.Label("Color:");
        Color32 color = new Color32(layer.r, layer.g, layer.b, 200);
        color = EditorGUILayout.ColorField(color);
        layer.r = color.r;
        layer.g = color.g;
        layer.b = color.b;
        //layer.speedMultiplier = EditorGUILayout.FloatField(layer.speedMultiplier);
        //  GUILayout.Label("Transparent:");

        layer.transperent = GUILayout.Toggle(layer.transperent, "Transparent:");        
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Cost multipler:");
        layer.cost = EditorGUILayout.FloatField(layer.cost);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        layer.excludeGraph = GUILayout.Toggle(layer.excludeGraph, "Do not include in graph:");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        layer.exscind = GUILayout.Toggle(layer.exscind, "Cut all nodes in collision:");
        GUILayout.EndHorizontal();

        //      GUILayout.EndArea();
        GUILayout.Space(30);
    }


    internal void RenderNotController(GalaxyMapEditor window)
    {
        
    }
}
