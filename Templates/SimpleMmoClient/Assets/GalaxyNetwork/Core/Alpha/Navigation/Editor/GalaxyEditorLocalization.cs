using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class GalaxyEditorLocalization
{
    private static readonly Dictionary<string, string> english;
    private static readonly Dictionary<string, string> russian;
    private static Dictionary<string, string> current;
    private static GalaxyEditorLanguage _language; 

    static GalaxyEditorLocalization()
    {
        english = new Dictionary<string, string>();
        russian = new Dictionary<string, string>();
        Keys();
        SetLanguage((GalaxyEditorLanguage) PlayerPrefs.GetInt("GalaxyEditorLanguage"));
    }

    private static void Keys()
    {
        AddKey("scene_settings","Scene settings:","Настройки сцены:");
        AddKey("range","Zone size:","Размер зоны:");
        AddKey("cell_size","Cell Size:","Размер ячейки:");
        AddKey("settings","Settings","Настройки");
        AddKey("language","Language:","Язык:");
        AddKey("scene","Scene","Сцена");
        AddKey("layers","Layers","Слои");
        AddKey("object","Object","Объект");
        AddKey("notBacker","There is no local bake controller in the scene.","На сцене отсутствует локальный контроллер запекания.");
        AddKey("addBacker","Create controller automatically","Создать контроллер автоматически");
        AddKey("notFile","Could not find save file for this scene","Не удалось найти файл с сохранением для этой сцены");
        AddKey("openFile","Open file","Открыть файл");
        AddKey("createFile","Create file","Создать файл");
        AddKey("or","or","или");
        AddKey("cost","Multiplier Cost","Множитель стоимости");
        AddKey("name","Name","Название");
        AddKey("color","Color","Цвет");
        AddKey("createLayer","Add new layer","Создать новый слой");
        AddKey("maxHeightLink","Maximum height difference","Максимальный перепад высот");
        AddKey("layerTransparent","Continue scanning under layer","Продолжить сканирование под слоем");
        AddKey("graphInclude","Do not include in graph","Исключить из графа");
        AddKey("trimNodes","Trim other nodes","Обрезать другие ноды");
        AddKey("notColliders","The object cannot be baked because it does not have a collider.","Объект не может быть запечен, потому что не имеет коллайдера");
    }
    
    private static void SetLanguage(GalaxyEditorLanguage lang)
    {
        _language = lang;
        switch (lang)
        {
            case GalaxyEditorLanguage.english:
                current = english;
                break;
            case GalaxyEditorLanguage.russian:
                current = russian;
                break;
        }
        PlayerPrefs.SetInt("GalaxyEditorLanguage",(int)lang);
    }
    private static void AddKey(string key, string en, string ru)
    {
        english.Add(key,en);
        russian.Add(key,ru);
    }
    
    public static string Get(string key)
    {
        if (current.TryGetValue(key,out var result))
        {
            return result;
        }
        return english.TryGetValue(key,out result) ? result : key;
    }

    public static void DrawSettings()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(Get("language"));
        _language = (GalaxyEditorLanguage) EditorGUILayout.EnumPopup(_language);
        SetLanguage(_language);
        GUILayout.EndHorizontal();
    }

    private enum GalaxyEditorLanguage:int
    {
        english,
        russian
    }
}

 


