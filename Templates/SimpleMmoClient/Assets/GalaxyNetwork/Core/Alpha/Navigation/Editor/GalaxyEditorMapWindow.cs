using GalaxyNetwork.Core.Alpha.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using Loc = GalaxyEditorLocalization;

namespace GalaxyCoreCommon.Navigation
{
    public class GalaxyEditorMapWindow : EditorWindow
    {
        private const string path = "Assets/GalaxyNetwork/Core/Alpha/Navigation/Editor/";
        private GalaxyEditorResources resources;
        private GalaxyEditopMapCommon common;
        private GalaxyMapBaker baker;
        private State state;
        private State oldState;
        private GameObject oldSelected;
        private VisualElement root;

        private VisualElement leftButtonScene;
        private VisualElement leftButtonLayers;
        private VisualElement leftButtonObjects;
        private VisualElement leftButtonSettings;
        private VisualElement bakeButton;
       
        private bool _loaded;

        [MenuItem("Galaxy Network/Navigation")]
        public static void ShowExample()
        {
            var wnd = GetWindow<GalaxyEditorMapWindow>();
            wnd.titleContent = new GUIContent("GalaxyEditorMapWindow");
        }

        public void OnEnable()
        {
            resources = AssetDatabase.LoadAssetAtPath<GalaxyEditorResources>(path + "GalaxyEditorResources.asset");
            root = rootVisualElement;
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "GalaxyEditorMapWindow.uss");
            root.styleSheets.Add(styleSheet);
            common = new GalaxyEditopMapCommon(rootVisualElement,this);
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "GalaxyEditorMapWindow.uxml");
            VisualElement labelFromUXML = visualTree.CloneTree();
            root.Add(labelFromUXML);
            Image logo = new Image();
            logo.image = resources.Logo;
            var headerLabel = new Label("Galaxy Navigation");
            headerLabel.name = "HeaderLabel";
            root.Q<VisualElement>("Header").Add(headerLabel);
            root.Q<VisualElement>("Logo").Add(logo);
            CreateLeftMenu();
            root.Q<VisualElement>("NotBacker").Add(common.CreateBackerContainer(OnClickCreateBacker));
            root.Q<VisualElement>("NotFile").Add(common.CreateNotFileContainer(OnClickOpenFile, OnClickCreateFile));

            state = State.none;
            Selection.selectionChanged += SelectionChanged;

            if (baker != null)
            {
                baker.Load();
                _loaded = true;
            }

            OnGUI();
        }

        public void ReDrawSettings()
        {
            root.Clear();
            OnEnable();
            state = State.settings;
        }

        private void CreateLeftMenu()
        {
            var leftPanel = root.Q<VisualElement>("LeftPanel");
            leftButtonScene = GalaxyEditopMapCommon.AddButton(resources.SceneImage, Loc.Get("scene"), OnClickScene);
            leftPanel.Add(leftButtonScene);
            
            leftButtonLayers = GalaxyEditopMapCommon.AddButton(resources.LayerImage, Loc.Get("layers"), OnClickLayers);
            leftPanel.Add(leftButtonLayers);

            leftButtonObjects =
                GalaxyEditopMapCommon.AddButton(resources.ObjectImage, Loc.Get("object"), OnClickObjects);
            leftPanel.Add(leftButtonObjects);

            leftButtonSettings =
                GalaxyEditopMapCommon.AddButton(resources.SettingsImage, Loc.Get("settings"), OnClickSettings);
            leftPanel.Add(leftButtonSettings);
        }

        private void SelectionChanged()
        {
            if (state == State.objectLayers) common.RenderObjects();
        }


        public void OnClickOpenFile()
        {
            var loadPath = EditorUtility.OpenFilePanel("Open map", "", "gnm");
            baker.Load(loadPath);
        }

        private void OnClickCreateFile()
        {
            baker.Path =
                EditorUtility.SaveFilePanel("Open map", "", SceneManager.GetActiveScene().name + ".gnm", "gnm");
            File.WriteAllBytes(baker.Path, new byte[0]);
        }

        private void OnClickScene()
        {
            if ((int) state < 3) return;
            state = State.sceneSetting;
        }

        public void OnClickLayers()
        {
            if ((int) state < 3) return;
            state = State.layers;
        }

        public void OnClickObjects()
        {
            if ((int) state < 3) return;
            state = State.objectLayers;
        }

        private void OnClickSettings()
        {
            if ((int) state < 3) return;
            state = State.settings;
        }

        public void StartBake()
        {
            EditorGUI.ProgressBar(new Rect(3, 45, position.width - 6, 20), 1, "Baked");
            baker.Baked(); 
            EditorGUI.ProgressBar(new Rect(3, 45, position.width - 6, 20), 0, "Baked");
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

            if (!_loaded && baker.Path != null)
            {
                baker.Load();
                    _loaded = true;
            }

            common.backer = baker;
            if (baker.Path == null)
            {
                state = State.notFile;
                UpdateState();
                return;
            }

            if (!File.Exists(baker.Path))
            {
                state = State.notFile;
                UpdateState();
                return;
            }

            if ((int) state < 3) state = State.sceneSetting;
            UpdateState();
        }

        private void SetVisible(string element, bool visible)
        {
            rootVisualElement.Q<VisualElement>(element).visible = visible;
            if (visible)
                rootVisualElement.Q<VisualElement>(element).style.height = new StyleLength(StyleKeyword.Auto);
            else
                rootVisualElement.Q<VisualElement>(element).style.height = 0;
        }

        private void UpdateBakeButton()
        {
            bakeButton?.Clear();
            bakeButton = common.CreateBackedButton();

        }

        private void UpdateState()
        {
            if (oldState == state) return;
            oldState = state;
            SetVisible("NotBacker", false);
            SetVisible("WorkSpace", false);
            SetVisible("BackerEdit", false);
            SetVisible("NotFile", false);
            SetVisible("Layers", false);
            SetVisible("Objects", false);  
            SetVisible("Settings", false);
            rootVisualElement.Q<VisualElement>("Layers").Clear();
            rootVisualElement.Q<VisualElement>("Settings").Clear();
            leftButtonScene.RemoveFromClassList("LeftButtonActive");
            leftButtonLayers.RemoveFromClassList("LeftButtonActive");
            leftButtonObjects.RemoveFromClassList("LeftButtonActive");
            leftButtonSettings.RemoveFromClassList("LeftButtonActive");
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
                    leftButtonScene.AddToClassList("LeftButtonActive");
                    break;
                case State.layers:
                    common.RenderLayersTab();
                    SetVisible("Layers", true);
                    leftButtonLayers.AddToClassList("LeftButtonActive");
                    break;
                case State.objectLayers:
                    common.RenderObjects();
                    SetVisible("Objects", true);
                    leftButtonObjects.AddToClassList("LeftButtonActive");
                    break;
                case State.settings:
                    common.RenderSettings();
                    SetVisible("Settings", true);
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
}