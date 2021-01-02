using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using Loc = GalaxyEditorLocalization;

public class GalaxyEditorMapWindow : EditorWindow
{
    private string path = "Assets/GalaxyNetwork/Core/Alpha/Navigation/Editor/test/";
    private GalaxyEditorResources resources;
    private GalaxyEditopMapCommon common;
    public GalaxyMapBaker baker;
    private State state;
    private State oldState;
    private GameObject oldSelected;
    [MenuItem("Window/UIElements/GalaxyEditorMapWindow")]
    public static void ShowExample()
    {
        GalaxyEditorMapWindow wnd = GetWindow<GalaxyEditorMapWindow>();
        wnd.titleContent = new GUIContent("GalaxyEditorMapWindow");
    }

    public void OnEnable()
    {
        resources = AssetDatabase.LoadAssetAtPath<GalaxyEditorResources>(path + "GalaxyEditorResources.asset");
        var root = rootVisualElement;
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "GalaxyEditorMapWindow.uss");
        root.styleSheets.Add(styleSheet);
        common = new GalaxyEditopMapCommon(rootVisualElement);
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "GalaxyEditorMapWindow.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);
        Image logo = new Image();
        logo.image = resources.Logo;
        var headerLabel = new Label("Galaxy Navigation");
        headerLabel.name = "HeaderLabel";
        root.Q<VisualElement>("Header").Add(headerLabel);
        root.Q<VisualElement>("Logo").Add(logo);
        root.Q<VisualElement>("LeftPanel").Add(common.AddButton(resources.SceneImage, Loc.Get("scene"), OnClickScene));
        root.Q<VisualElement>("LeftPanel").Add(common.AddButton(resources.LayerImage, Loc.Get("layers"), OnClickLayers));
        root.Q<VisualElement>("LeftPanel").Add(common.AddButton(resources.ObjectImage, Loc.Get("object"), OnClickObjects));
        root.Q<VisualElement>("LeftPanel").Add(common.AddButton(resources.SettingsImage, Loc.Get("settings"), OnClickSettings));
        root.Q<VisualElement>("NotBacker").Add(common.CreateBackerContainer(OnClickCreateBacker));
        root.Q<VisualElement>("NotFile").Add(common.CreateNotFileContainer(OnClickOpenFile,OnClickCreateFile));
        
        state = State.none;
        Selection.selectionChanged += SelectionChanged;
    }

    private void SelectionChanged()
    {
       if(state == State.objectLayers)  common.RenderObjects();
    }


    private void OnClickOpenFile()
    {
        baker.path = EditorUtility.OpenFilePanel("Open map", "", "gnm");
    }
    private void OnClickCreateFile()
    {
        baker.path = EditorUtility.SaveFilePanel("Open map", "", SceneManager.GetActiveScene().name + ".gnm", "gnm");
        File.WriteAllBytes(baker.path,new byte[0]);
    }
    private void OnClickScene()
    {
       if((int)state<3)return;
       state = State.sceneSetting;
    }
    private void OnClickLayers()
    {
        if((int)state<3)return;
        state = State.layers;
    }
    private void OnClickObjects()
    {
        if((int)state<3)return;
        state = State.objectLayers;
    }
    private void OnClickSettings()
    {
        if((int)state<3)return;
        state = State.layers;
    }
    private void OnGUI()
    {
        rootVisualElement.Q<VisualElement>("Container").style.height = new 
            StyleLength(position.height);
        if (baker == null)
        {
            baker = FindObjectOfType<GalaxyMapBaker>();
            if (baker == null)
            {
                state = State.notBacker;
                UpdateState();
                return;
            }
        }

        common.backer = baker;
        if (baker.path == null)
        {
            state = State.notFile;
            UpdateState();
            return;
        }
        if (!File.Exists(baker.path))
        {
            state = State.notFile;
            UpdateState(); 
            return;
        }

        if ((int)state < 3) state = State.sceneSetting;
        UpdateState();
    }

    private void SetVisible(string element,bool visible)
    {
        rootVisualElement.Q<VisualElement>(element).visible = visible;
        if (visible)
            rootVisualElement.Q<VisualElement>(element).style.height = new StyleLength(StyleKeyword.Auto);
        else
            rootVisualElement.Q<VisualElement>(element).style.height = 0;
    }
    private void UpdateState()
    {
        if(oldState == state)return;
        oldState = state;
        SetVisible("NotBacker", false);
        SetVisible("WorkSpace", false);
        SetVisible("BackerEdit", false);
        SetVisible("NotFile", false);
        SetVisible("Layers", false);
        SetVisible("Objects", false);
        switch (state)
        {
            case State.notBacker:
                SetVisible("NotBacker", true);
                break;
            case State.notFile:
                SetVisible("NotFile", true);
                break;
            case State.sceneSetting:
                rootVisualElement.Q<VisualElement>("BackerEdit").Clear();
                rootVisualElement.Q<VisualElement>("BackerEdit").Add(common.CreateBackerEdit());
                SetVisible("BackerEdit", true);
                break;
            case State.layers:
                common.RenderLayersTab();
                SetVisible("Layers", true);
                break;
            case State.objectLayers:
                common.RenderObjects();
                SetVisible("Objects", true);
                break;
            case State.settings:
                break;
        }
    }

    private void OnClickCreateBacker()
    {
        baker = new GameObject("GalaxyMapBaker").AddComponent<GalaxyMapBaker>();
    }

    enum State
    {
        none,
        notBacker,
        notFile,
        sceneSetting,
        layers,
        objectLayers,
        settings
    }
}