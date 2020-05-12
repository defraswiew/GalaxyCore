using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Instance Info", menuName = "GalaxyNetwork/Instance Info", order = 1)]
public class GalaxyInstanceInfoScriptable : ScriptableObject
{
    [Header("Отображаемое имя")]
    public string uiName;
    [Header("Описание")]
    [TextArea(3,10)]
    public string uiDescription;
    [Header("Превью сцены")]
    public Sprite img; 
    [Header("Имя сцены (может быть пусто)")]   
    public string sceneName;
    [Header("Тип сцены (для переопределения)")]
    public byte type;
}
