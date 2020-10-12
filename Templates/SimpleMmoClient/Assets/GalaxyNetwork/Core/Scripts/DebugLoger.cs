using UnityEngine;
using GalaxyCoreLib.Api;

/// <summary>
/// Пример логгера занимающегося перехватом сообщений ядра
/// Просто перекидываем события в юньковский Debug
/// </summary>
public class DebugLoger : MonoBehaviour
{
    private void OnEnable()
    {
        // сообщение информационного характера
        GalaxyEvents.OnGalaxyLogInfo += OnGalaxyLogInfo;
        // сообщение предупреждение
        GalaxyEvents.OnGalaxyLogWarnig += OnGalaxyLogWarnig;
        // сообщение об ошибке
        GalaxyEvents.OnGalaxyLogError += OnGalaxyLogError;        
    }

    private void OnDisable()
    {
        GalaxyEvents.OnGalaxyLogInfo -= OnGalaxyLogInfo;
        GalaxyEvents.OnGalaxyLogWarnig -= OnGalaxyLogWarnig;
        GalaxyEvents.OnGalaxyLogError -= OnGalaxyLogError;
    }

    private void OnGalaxyLogError(string publisher, string message)
    {
        Debug.LogError(publisher + "-> " + message);
    }

    private void OnGalaxyLogWarnig(string publisher, string message)
    {
        Debug.LogWarning(publisher + "-> " + message);
    }

    private void OnGalaxyLogInfo(string publisher, string message)
    {
        Debug.Log(publisher + "-> " + message);
    }
}
