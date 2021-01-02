using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Localize = GalaxyEditorLocalization;

public class GalaxyMapEditor : EditorWindow
{
    public GalaxyMapBaker baker;
    GalaxyMapEditorCommon common;
    private bool tabObject;
    private bool tabLayers;
    private bool tabScene;
    private bool tabOptions;
    private GameObject selected;
    internal int currentLayer;
    public float secs = 20f;
    public float startVal = 0f;
   
    [MenuItem("Galaxy Network / Navigation")]
    public static void ShowWindow()
    {
        GalaxyMapEditor wnd = (GalaxyMapEditor)EditorWindow.GetWindow(typeof(GalaxyMapEditor));
        wnd.titleContent.text = "Galaxy map baker";
        wnd.titleContent.tooltip = "Galaxy map baker";
        
    }
    void OnEnable()
    {
        if(common==null) common = new GalaxyMapEditorCommon();
    }
/*
    void OnEnable()
    {
     if(common==null) common = new GalaxyMapEditorCommon();
    
       common.Load(baker);
    }

    void Update()
    {
     CreateBaker();
    }

    private void CreateBaker()
    {
        if (baker == null)
        {
            baker = FindObjectOfType<GalaxyMapBaker>();
            if (baker == null)
                baker = new GameObject("GalaxyMapBaker").AddComponent<GalaxyMapBaker>();
        }
    }
*/
    void OnGUI()
    {
        if (baker == null)
        {
            baker = FindObjectOfType<GalaxyMapBaker>();
            common.RenderNotController(this);
           // return;
        }
        
        
        if (EditorApplication.isPlaying)
        {
            GUILayout.Label("Runtime changes are not supported");
            return;
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(Localize.Get("scene"), EditorStyles.miniButtonMid)) SetTab(TabType.scene);
        if (GUILayout.Button(Localize.Get("layers"), EditorStyles.miniButtonMid)) SetTab(TabType.layers);
        if (GUILayout.Button(Localize.Get("object"), EditorStyles.miniButtonMid)) SetTab(TabType.objects);
        if (GUILayout.Button(Localize.Get("settings"), EditorStyles.miniButtonMid)) SetTab(TabType.options);
        GUILayout.EndHorizontal();

        if (tabScene)common.RenderSceneSettings(baker);
        if (tabObject) common.RenderTabObject(baker);
        if (tabLayers) common.RenderLayers(this);
        if (tabOptions) common.RenderTabOptions();

        //common.RenderPath(baker);

        if (GUILayout.Button("Bake"))
        {
            if (baker.path == null) common.RenderPath(baker);
            if (baker.path == "") common.RenderPath(baker);  
           baker.Baked();


        }
        if (GUILayout.Button("Test"))
        {
            foreach (var item in baker.map.Nodes)
            {
                Debug.Log(new Vector3(item.x, item.y, item.z));
            }


        }

        if (baker.progress > 0 && baker.progress < 1) 
            EditorUtility.DisplayProgressBar("Simple Progress Bar", "Shows a progress bar for the given seconds", baker.progress);
        else
            EditorUtility.ClearProgressBar();

        GUILayout.Label("Nodes: "+ baker.map.Nodes.Count);
    }

    private void SetTab(TabType type)
    {
        tabObject = false;
        tabLayers = false;
        tabScene = false;
        tabOptions = false;
        switch (type)
        {
            case TabType.scene:
                tabScene = true;
                break;
            case TabType.layers:
                tabLayers = true;
                break;
            case TabType.objects:
                tabObject = true;
                break;
            case TabType.options:
                tabOptions = true;
                break;
        }
    }
    

private enum TabType
    {
        scene,
        layers,
        objects,
        options
    }
}

