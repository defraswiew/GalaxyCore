using System;
using System.Collections;
using System.Collections.Generic;
using GalaxyCoreCommon.Navigation;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Loc = GalaxyEditorLocalization;
using Random = System.Random;

public class GalaxyEditopMapCommon
{

    public GalaxyMapBaker backer;
    private VisualElement root;
    public GalaxyEditopMapCommon(VisualElement root)
    {
        this.root = root;
    }

    private Color32 GetRandomColor()
    {
        var color = new Color();
        color.r = UnityEngine.Random.Range(0f, 1f);
        color.g = UnityEngine.Random.Range(0f, 1f);
        color.b = UnityEngine.Random.Range(0f, 1f);
        return color;
    }
    
    public VisualElement AddButton(Texture2D img, string text, Action action)
    {
        var container = new VisualElement {name = "LeftButtonContainer"};
        var button = new Button();
        button.style.backgroundImage = img;
        button.name = "LeftButton";
        button.clickable.clicked += action;
        var label = new Label(text);
        label.name = "LeftButtonLabel";
        container.Add(button);
        container.Add(label);
        return container;
    }

    public VisualElement AddButton(string text, Action action,  string style = "ButtonBig")
    {
        var button = new Button();
        button.name = style;
        button.text = text;
        button.clickable.clicked += action;
        return button;
    }
    
    
    public VisualElement CreateBackerContainer(Action action)
    {
        var container = new VisualElement();
        container.name = "NotBackerContainer";
        var label = new Label(Loc.Get("notBacker"));
        label.name = "ErrorMessage";
        container.Add(label);
        var button = new Button();
        button.name = "ButtonBig";
        button.text = Loc.Get("addBacker");
        button.clickable.clicked += action;
        container.Add(button);
        return container;
    }


    public void RenderLayersTab()
    {
        root.Q<VisualElement>("Layers").Clear();
        root.Q<VisualElement>("Layers").Add(CreateLayersContainer());
        RenderAddLayerButton();
    }

    public VisualElement CreateLayersContainer()
    {
        var container = new VisualElement();
        container.name = "LayersContainer";
        container.Add(CreateLayersSelected(SelectLayer));
        container.Add(new VisualElement(){name = "LayerContainer"});
        return container;
    }
    public VisualElement CreateNotFileContainer(Action open ,Action create)
    {
        var container = new VisualElement();
        container.name = "NotFileContainer";
        var label = new Label(Loc.Get("notFile"));
        label.name = "ErrorMessage";
        container.Add(label);
        var button = new Button();
        button.name = "ButtonBig";
        button.text = Loc.Get("openFile");
        button.clickable.clicked += open;
        container.Add(button);
        var label2 = new Label(Loc.Get("or"));
        label2.name = "ErrorMessage";
        container.Add(label2);
        var button2 = new Button();
        button2.name = "ButtonBig";
        button2.text = Loc.Get("createFile");
        button2.clickable.clicked += create;
        container.Add(button2);
        return container;
    }
    public VisualElement CreateBackerEdit()
    {
        var container = new VisualElement();
        container.name = "BackerEdit";
        
        var size = new Vector3Field();
        size.value = backer.mapSize;
        size.label = Loc.Get("range");
        size.RegisterCallback<ChangeEvent<Vector3>>(change =>
        {
            backer.mapSize = change.newValue;
        });
        container.Add(size);
        
         
        var cellSizeValue = CreateSlider(0.5f, 15f, 1, Loc.Get("cell_size"));
    
        cellSizeValue.RegisterCallback<ChangeEvent<float>>(change =>
        {
            backer.cellSize = (float)Math.Round(change.newValue,1);
        });
        cellSizeValue.value = backer.cellSize;
        container.Add(cellSizeValue);
        
        var minHeight = CreateSlider(1f, 100f, 0, Loc.Get("minHeight"));
        
        minHeight.RegisterCallback<ChangeEvent<float>>(change =>
        {
            backer.minHeight = (float)Math.Round(change.newValue,0);
        });
        minHeight.value = backer.minHeight;
        container.Add(minHeight);
        return container;
    }

    public void AddLayer()
    {
        if (backer.map.layers == null)
        {
            backer.map.layers = new GalaxyMapLayerManager();
        }
        var layer = new GalaxyMapLayer();
      layer.name = "New Layer";
      layer.SetColor(GetRandomColor());
      RenderLayerEdit(layer);
    }

    public void RenderObjects()
    {
        var container = root.Q<VisualElement>("Objects");
        container.Clear();
        
        if (Selection.activeTransform == null)
        { 
            var notSelected = new Label(){text = "Not selected"};
            notSelected.name = "ErrorMessage";
            container.Add(notSelected);
            return;
        }
        var selected = Selection.activeTransform.gameObject;
        var colliders = selected.GetComponentsInChildren<Collider>();
        
        var objectName = new Label(){text =  selected.name};
        container.Add(objectName);
        if (colliders.Length == 0)
        {
            var notColliders = new Label(){text = Loc.Get("notColliders")};
            notColliders.name = "ErrorMessage";
            container.Add(notColliders);
            return;
        }
        container.Add(CreateLayersSelected(SelectLayerInObject));
        
    }

    private void SelectLayerInObject(object obj)
    {
        
    }
    
    public void RenderAddLayerButton()
    {
        root.Q<VisualElement>("LayerContainer").Clear();
        root.Q<VisualElement>("LayerContainer").Add(AddButton(Loc.Get("createLayer"),AddLayer));
    }
    public void RenderLayerEdit(GalaxyMapLayer layer)
    {
        var container = new VisualElement();
        container.name = "LayerEditor";
        root.Q<VisualElement>("LayerContainer").Clear();
        var name = new TextField();
        name.value = layer.name;
        name.label = Loc.Get("name");
        name.RegisterCallback<ChangeEvent<string>>(change =>
        {
            layer.name = change.newValue;
        });
        container.Add(name);
        
        var color = new ColorField(){label = Loc.Get("color")};
        color.value = layer.Color();
        container.Add(color);
        color.RegisterCallback<ChangeEvent<Color>>(change =>
        {
            layer.SetColor(change.newValue);
        });
        
        var maxHeight = new FloatField(){label = Loc.Get("maxHeightLink")};
        maxHeight.value = layer.heightWalkable;
        maxHeight.RegisterCallback<ChangeEvent<float>>(change =>
        {
            layer.heightWalkable = change.newValue;
        });
        container.Add(maxHeight);
        
        var cost = new FloatField(){label = Loc.Get("cost")};
        cost.value = layer.cost;
        cost.RegisterCallback<ChangeEvent<float>>(change =>
        {
            layer.heightWalkable = change.newValue;
        });
        container.Add(cost);
        var transparent = new Toggle() {label = Loc.Get("layerTransparent")};
        transparent.value = layer.transperent;
        transparent.RegisterCallback<ChangeEvent<bool>>(change =>
        {
            layer.transperent = change.newValue;
        });
        container.Add(transparent);
        var graphInclude = new Toggle() {label = Loc.Get("graphInclude")};
        graphInclude.value = layer.excludeGraph;
        graphInclude.RegisterCallback<ChangeEvent<bool>>(change =>
        {
            layer.excludeGraph = change.newValue;
        });
        container.Add(graphInclude);
        var trimNodes = new Toggle() {label = Loc.Get("trimNodes")};
        trimNodes.value = layer.exscind;
        trimNodes.RegisterCallback<ChangeEvent<bool>>(change =>
        {
            layer.exscind = change.newValue;
        });
        container.Add(trimNodes);
         
        
        container.Add(AddButton("Save", () => { SaveLayers(layer);}, "GreenButton"));
        root.Q<VisualElement>("LayerContainer").Add(container);
    }

    public void SaveLayers(GalaxyMapLayer layer)
    {
        if (layer.id == 0)
        {
          var id =  backer.map.layers.CreateLayer();
          var newLayer = backer.map.layers.GetById(id);
          layer.id = id;
          newLayer.Apply(layer);
        }
        else
        {
            var newLayer = backer.map.layers.GetById(layer.id);
            newLayer.Apply(layer);
        }
        RenderLayersTab();
    }
    public VisualElement CreateLayersSelected(Action<object> action)
    {
        var container = new VisualElement();
        container.name = "LayersSelector";
        var label = new Label("Selected Layer");
        label.name = "LayersSelectorLabel";
        container.Add(label);
        var layers = new Dictionary<int, string>();
        layers.Add(0,"Not walkable");
        if (backer.map.layers != null)
        {
            foreach (var layer in backer.map.layers.list)
            {
                layers.Add(layer.id, layer.name);
            }
        }
        container.Add(CreateListView(layers,action));
        return container;
    }

    private void SelectLayer(object obj)
    {
        if (Int32.TryParse(((string) obj).Split('%')[0], out var id))
        {
            var copy = backer.map.layers.GetById((byte) id).Copy();
            RenderLayerEdit(copy);
        }
    }

    public ListView CreateListView(Dictionary<int, string> values, Action<object> action)
    {
        List<string> elements = new List<string>();
        foreach (var item in values)
        {
            elements.Add(item.Key+"%"+item.Value);
        }
        Func<VisualElement> makeItem = () => new Label();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            var elem = elements[i].Split('%');
            (e as Label).text = elem[1];
            (e as Label).name = "GalaxySelectedLabel";

        };
        var list = new ListView(elements,20,makeItem,bindItem);
        list.name = "GalaxySelected";
        list.selectionType = SelectionType.Multiple;
        list.onItemChosen += action;
      //  list.onSelectionChanged += objects => Debug.Log(objects[0]);
        list.style.flexGrow = 1.0f;
        return list;
    }

 
    public Slider CreateSlider(float min, float max, int dec, string label, string tooltip = "")
    {
        var slider = new Slider();
        slider.tooltip = tooltip;
        var floatField = new FloatField();
        floatField.value  = (float)Math.Round(max/2,dec);
        slider.label = label;
        slider.value = (float)Math.Round(max/2,dec);
        slider.lowValue = min;
        slider.highValue = max;
        slider.Add(floatField);
        slider.RegisterCallback<ChangeEvent<float>>(change =>
        { 
            var value = (float)Math.Round(change.newValue,dec);
            floatField.value = value;
            if (value > max) value = max;
            if (value <min) value = min;
        });
        floatField.RegisterCallback<ChangeEvent<float>>(change =>
        {
            var value = (float)Math.Round(change.newValue,1);
            slider.value = value;
        });
        return slider;
    }
    
}
