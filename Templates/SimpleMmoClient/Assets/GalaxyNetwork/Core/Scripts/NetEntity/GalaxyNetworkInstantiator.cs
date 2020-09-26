using UnityEngine;
using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using System.Collections.Generic;
/// <summary>
/// Компонент отвечающий за создание новый сетевых объектов
/// </summary>
public class GalaxyNetworkInstantiator : MonoBehaviour
{
    private Dictionary<string, UnityNetEntity> gos = new Dictionary<string, UnityNetEntity>();
    private void OnEnable()
    {
        // подписываемся на событие о создании сетевой сущности
        GalaxyEvents.OnGalaxyNetEntityInstantiate += OnGalaxyNetEntityInstantiate;
        foreach (var item in Resources.LoadAll<UnityNetEntity>(""))
        {
            Debug.Log(item.name);
            gos.Add(item.name, item);
        }
        Resources.UnloadUnusedAssets();
    }

    private void OnDisable()
    {
        // отписываемся
        GalaxyEvents.OnGalaxyNetEntityInstantiate -= OnGalaxyNetEntityInstantiate;
    }

    private ClientNetEntity OnGalaxyNetEntityInstantiate(ClientNetEntity netEntity)
    {
        Debug.Log(netEntity.prefabName);
        // если нет имени у сетевой сущности, значит не нужно создавать для неё go
        if (netEntity.prefabName == "") return null;
        //  Debug.Log(netEntity.transform.position.Vector3());
        UnityNetEntity go = Instantiate(gos[netEntity.prefabName], netEntity.transform.position.Vector3(), netEntity.transform.rotation.Quaternion());
        //   UnityNetEntity go = Instantiate(Resources.Load<UnityNetEntity>(netEntity.prefabName), netEntity.transform.position.Vector3(), netEntity.transform.rotation.Quaternion());
        go.netEntity.transform.position = netEntity.transform.position;
        // возвращяем целевой экземпляр ClientNetEntity из нового go
        return go.netEntity;
    }
}
