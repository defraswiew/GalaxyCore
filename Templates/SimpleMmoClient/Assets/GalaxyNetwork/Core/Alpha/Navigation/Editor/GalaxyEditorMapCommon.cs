using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Loc = GalaxyEditorLocalization;

namespace GalaxyCoreCommon.Navigation
{
    public class GalaxyEditopMapCommon
    {
        public GalaxyMapBaker backer;
        private readonly VisualElement root;
        private readonly GalaxyEditorMapWindow window;

        public GalaxyEditopMapCommon(VisualElement root, GalaxyEditorMapWindow window)
        {
            this.root = root;
            this.window = window;
        }

        private static Color32 GetRandomColor()
        {
            var color = new Color
            {
                r = UnityEngine.Random.Range(0f, 1f),
                g = UnityEngine.Random.Range(0f, 1f),
                b = UnityEngine.Random.Range(0f, 1f)
            };
            return color;
        }

        public static VisualElement AddButton(Texture2D img, string text, Action action)
        {
            var container = new VisualElement
            {
                name = "LeftButtonContainer"
            };
            var button = new Button();
            button.style.backgroundImage = img;
            button.name = "LeftButton";
            button.clickable.clicked += action;
            var label = new Label(text)
            {
                name = "LeftButtonLabel"
            };
            container.Add(button);
            container.Add(label);
            return container;
        }

        private VisualElement AddButton(string text, Action action, string style = "ButtonBig")
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

        private VisualElement CreateLayersContainer()
        {
            var container = new VisualElement
            {
                name = "LayersContainer"
            };
            container.Add(CreateLayersSelected(SelectLayer));
            container.Add(new VisualElement() {name = "LayerContainer"});
            return container;
        }

        public VisualElement CreateNotFileContainer(Action open, Action create)
        {
            var container = new VisualElement
            {
                name = "NotFileContainer"
            };
            var label = new Label(Loc.Get("notFile"))
            {
                name = "ErrorMessage"
            };
            container.Add(label);
            var button = new Button
            {
                name = "ButtonBig",
                text = Loc.Get("openFile")
            };
            button.clickable.clicked += open;
            container.Add(button);
            var label2 = new Label(Loc.Get("or"))
            {
                name = "ErrorMessage"
            };
            container.Add(label2);
            var button2 = new Button
            {
                name = "ButtonBig",
                text = Loc.Get("createFile")
            };
            button2.clickable.clicked += create;
            container.Add(button2);
            return container;
        }

        public VisualElement CreateBackerEdit()
        {
            Selection.SetActiveObjectWithContext(backer, backer);
            var container = new VisualElement
            {
                name = "BackerEdit"
            };
            var size = new Vector3Field
            {
                value = backer.mapSize,
                label = Loc.Get("range")
            };
            size.RegisterCallback<ChangeEvent<Vector3>>(change =>
            {
                backer.mapSize = change.newValue;
                MarkDirty();
            });
            container.Add(size);

            var cellSizeValue = CreateSlider(0.5f, 15f, 1, Loc.Get("cell_size"));

            cellSizeValue.RegisterCallback<ChangeEvent<float>>(change =>
            {
                backer.cellSize = (float) Math.Round(change.newValue, 1);
                MarkDirty();
            });
            cellSizeValue.value = backer.cellSize;
            container.Add(cellSizeValue);

            var minHeight = CreateSlider(1f, 100f, 0, Loc.Get("minHeight"));

            minHeight.RegisterCallback<ChangeEvent<float>>(change =>
            {
                backer.minHeight = (float) Math.Round(change.newValue, 0);
                MarkDirty();
            });
            minHeight.value = backer.minHeight;
            container.Add(minHeight);

           
            
            var visibleCells = new Toggle
            {
                label = Loc.Get("drawCells"),
                value = backer.drawCell
            };
            visibleCells.RegisterCallback<ChangeEvent<bool>>(change => { backer.drawCell = change.newValue; SceneView.RepaintAll(); });
            container.Add(visibleCells);
            
            var visibleLinks = new Toggle
            {
                label = Loc.Get("drawLinks"),
                value = backer.drawLinks
            };
            visibleLinks.RegisterCallback<ChangeEvent<bool>>(change => { backer.drawLinks = change.newValue; SceneView.RepaintAll(); });
            container.Add(visibleLinks);
            
            var bakeButton = CreateBackedButton();
            container.Add(bakeButton);
            
            return container;
        }

        private void MarkDirty()
        {
            EditorUtility.SetDirty(backer);
            EditorSceneManager.MarkSceneDirty(backer.gameObject.scene);
            SceneView.RepaintAll();
        }

        private void AddLayer()
        {
            if (backer.map.layers == null)
            {
                backer.map.layers = new GalaxyMapLayerManager();
            }

            var layer = new GalaxyMapLayer
            {
                name = "New Layer"
            };
            layer.SetColor(GetRandomColor());
            RenderLayerEdit(layer);
        }

        public void RenderObjects()
        {
            var container = root.Q<VisualElement>("Objects");
            container.Clear();

            if (Selection.activeTransform == null)
            {
                var notSelected = new Label
                {
                    text = "Not selected",
                    name = "ErrorMessage"
                };
                container.Add(notSelected);
                return;
            }

            var selected = Selection.activeTransform.gameObject;
            var colliders = selected.GetComponentsInChildren<Collider>();

            var objectName = new Label()
            {
                text = selected.name,
                name = "ObjectSelectedLabel"
            };
            container.Add(objectName);
            if (colliders.Length == 0)
            {
                var notColliders = new Label
                {
                    text = Loc.Get("notColliders"),
                    name = "ErrorMessage"
                };
                container.Add(notColliders);
                return;
            }

            var layerId = backer.GetLayer(selected);
            container.Add(CreateLayersSelected(SelectLayerInObject, layerId, 700));

        }

        private void SelectLayerInObject(object obj)
        {
            if (int.TryParse(((string) obj).Split('%')[0], out var id))
            {
                backer.SetLayer(Selection.activeGameObject.gameObject, (byte) id);
                MarkDirty();
            }
        }

        private void RenderAddLayerButton()
        {
            root.Q<VisualElement>("LayerContainer").Clear();
            root.Q<VisualElement>("LayerContainer").Add(AddButton(Loc.Get("createLayer"), AddLayer));
        }

        private void RenderLayerEdit(GalaxyMapLayer layer)
        {
            var container = new VisualElement
            {
                name = "LayerEditor"
            };
            root.Q<VisualElement>("LayerContainer").Clear();
            var name = new TextField
            {
                value = layer.name,
                label = Loc.Get("name")
            };
            name.RegisterCallback<ChangeEvent<string>>(change => { layer.name = change.newValue; });
            container.Add(name);

            var color = new ColorField
            {
                label = Loc.Get("color"),
                value = layer.Color()
            };
            container.Add(color);
            color.RegisterCallback<ChangeEvent<Color>>(change => { layer.SetColor(change.newValue); });
            var maxHeight = new FloatField
            {
                label = Loc.Get("maxHeightLink"),
                value = layer.heightWalkable
            };
            maxHeight.RegisterCallback<ChangeEvent<float>>(change => { layer.heightWalkable = change.newValue; });
            container.Add(maxHeight);

            var cost = new FloatField
            {
                label = Loc.Get("cost"),
                value = layer.cost
            };
            cost.RegisterCallback<ChangeEvent<float>>(change => { layer.heightWalkable = change.newValue; });
            container.Add(cost);
            var transparent = new Toggle
            {
                label = Loc.Get("layerTransparent"),
                value = layer.transperent
            };
            transparent.RegisterCallback<ChangeEvent<bool>>(change => { layer.transperent = change.newValue; });
            container.Add(transparent);
            var graphInclude = new Toggle
            {
                label = Loc.Get("graphInclude"),
                value = layer.excludeGraph
            };
            graphInclude.RegisterCallback<ChangeEvent<bool>>(change => { layer.excludeGraph = change.newValue; });
            container.Add(graphInclude);
            var trimNodes = new Toggle
            {
                label = Loc.Get("trimNodes"),
                value = layer.exscind
            };
            trimNodes.RegisterCallback<ChangeEvent<bool>>(change => { layer.exscind = change.newValue; });
            container.Add(trimNodes);
            container.Add(AddButton("Delete", () => { RemoveLayer(layer); }, "RedButton"));
            container.Add(AddButton("Save", () => { SaveLayers(layer); }, "GreenButton"));
           
            root.Q<VisualElement>("LayerContainer").Add(container);
        }

        private void RemoveLayer(GalaxyMapLayer layer)
        {
            int i = 0;
            foreach (var layerCurrent in backer.layers)
            {
                if (layerCurrent == layer.id)
                {
                    backer.layers[i] = 0;
                    backer.objects[i] = null;
                }
                i++;
            }
            backer.map.layers.RemoveLayer(layer.id);
            RenderLayersTab();
            backer.Save();
        }

        private void SaveLayers(GalaxyMapLayer layer)
        {
            if (layer.id == 0)
            {
                var id = backer.map.layers.CreateLayer();
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
            backer.Save();
        }

        private VisualElement CreateLayersSelected(Action<object> action, byte selectedId = 0, int height = 0)
        {
            var container = new VisualElement
            {
                name = "LayersSelector"
            };
            var label = new Label(Loc.Get("select_layer"))
            {
                name = "LayersSelectorLabel"
            };
            container.Add(label);
            var layers = new Dictionary<int, string>
            {
                {0, "Not walkable"}
            };
            if (backer.map.layers != null)
            {
                foreach (var layer in backer.map.layers.list)
                {
                    layers.Add(layer.id, layer.name);
                }
            }

            container.Add(CreateListView(layers, action, selectedId, height));
            return container;
        }

        private void SelectLayer(object obj)
        {
            if (int.TryParse(((string) obj).Split('%')[0], out var id))
            {
                var copy = backer.map.layers.GetById((byte) id).Copy();
                RenderLayerEdit(copy);
            }
        }

        private ListView CreateListView(Dictionary<int, string> values, Action<object> action, byte selected = 0,
            int height = 0)
        {
            List<string> elements = new List<string>();
            var selectionItem = 0;
            var index = 0;
            foreach (var item in values)
            {
                elements.Add(item.Key + "%" + item.Value);
                if (item.Key == selected)
                {
                    selectionItem = index;
                }

                index++;
            }

            VisualElement MakeItem() => new Label();

            void BindItem(VisualElement e, int i)
            {
                var elem = elements[i].Split('%');
                ((Label) e).text = elem[0]+") "+elem[1];
                ((Label) e).name = "GalaxySelectedLabel";
            }

            var list = new ListView(elements, 20, MakeItem, BindItem)
            {
                name = "GalaxySelected",
                selectionType = SelectionType.Multiple,
                selectedIndex = selectionItem
            };
            list.onItemChosen += action;
            //  list.onSelectionChanged += objects => Debug.Log(objects[0]);
            list.style.flexGrow = 1.0f;
            if (height > 0)
            {
                list.style.height = height;
            }

            return list;
        }


        private Slider CreateSlider(float min, float max, int dec, string label, string tooltip = "")
        {
            var slider = new Slider
            {
                tooltip = tooltip
            };
            var floatField = new FloatField
            {
                value = (float) Math.Round(max / 2, dec)
            };
            slider.label = label;
            slider.value = (float) Math.Round(max / 2, dec);
            slider.lowValue = min;
            slider.highValue = max;
            slider.Add(floatField);
            slider.RegisterCallback<ChangeEvent<float>>(change =>
            {
                var value = (float) Math.Round(change.newValue, dec);
                floatField.value = value;
                //  if (value > max) value = max;
                //  if (value <min) value = min;
            });
            floatField.RegisterCallback<ChangeEvent<float>>(change =>
            {
                var value = (float) Math.Round(change.newValue, 1);
                slider.value = value;
            });
            return slider;
        }

        public void RenderSettings()
        {
            var container = root.Q<VisualElement>("Settings");
            container.Clear();
            var languages = new Dictionary<int, string>()
            {
                {0, "English"},
                {1, "Russian"}
            };
            var listView = CreateListView(languages, obj =>
            {
                if (int.TryParse(((string) obj).Split('%')[0], out var id))
                {
                    Loc.SetLanguage(id);
                    window.ReDrawSettings();
                }
            }, (byte) Loc.LanguageId);
            container.Add(listView);
            
            var button = new Button
            {
                name = "ButtonBig",
                text = Loc.Get("openFile")
            };
            button.clickable.clicked += window.OnClickOpenFile;
            container.Add(button);
            
        }

        public VisualElement CreateBackedButton()
        {
            var container = new VisualElement()
            {
                name = "BackedButtonContainer"
            };
            if (backer.map.layers == null)
            {
                return NotLayer(container);
            }
            if (backer.map.layers.list == null)
            {
                return NotLayer(container);
            }
            if (backer.map.layers.list.Count == 0)
            {
                return NotLayer(container);
            }
            if (!backer.IsNotLayerAssigned)
            {
                var notLayers = new Label()
                {
                    text = Loc.Get("not_layers_selected"),
                    name = "ErrorMessage"
                };
                container.Add(notLayers);
                container.Add(AddButton(Loc.Get("assigned_layers"), () => {window.OnClickObjects();}));
                return container;
            }
            
            var button = AddButton(Loc.Get("bake"), () => { window.StartBake();});
            container.Add(button);
            

            return container;
        }

        private VisualElement NotLayer(VisualElement container)
        {
            var notLayers = new Label()
            {
                text = Loc.Get("not_layers"),
                name = "ErrorMessage"
            };
            container.Add(notLayers);
            container.Add(AddButton(Loc.Get("create_layers"), () => {window.OnClickLayers();}));
            return container;
        }
    }

}

