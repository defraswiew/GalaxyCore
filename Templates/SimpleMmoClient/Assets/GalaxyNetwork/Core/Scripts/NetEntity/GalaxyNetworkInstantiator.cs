using UnityEngine;
using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using System.Collections.Generic;
/// <summary>
/// Компонент отвечающий за создание новый сетевых объектов
/// </summary>
public class GalaxyNetworkInstantiator : MonoBehaviour
{
    /// <summary>
    /// Кеш сетевых объектов собранных из ресурсов
    /// </summary>
    private Dictionary<string, UnityNetEntity> gos = new Dictionary<string, UnityNetEntity>();
    private void OnEnable()
    {
        // подписываемся на событие о создании сетевой сущности
        GalaxyEvents.OnGalaxyNetEntityInstantiate += OnGalaxyNetEntityInstantiate;
        foreach (var item in Resources.LoadAll<UnityNetEntity>(""))
        {         
            gos.Add(item.name, item);
        }
        Resources.UnloadUnusedAssets();
    }

    private void OnDisable()
    {
        // отписываемся от события создания новых сущностей
        GalaxyEvents.OnGalaxyNetEntityInstantiate -= OnGalaxyNetEntityInstantiate;
    }

    private ClientNetEntity OnGalaxyNetEntityInstantiate(ClientNetEntity netEntity)
    {
        // если нет имени у сетевой сущности, значит не нужно создавать для неё go
        // возвращяем null, в этом случае сетевая сущность будет существовать лишь в ядре
        // в дальнейшем её можно получить к примеру по ид
        if (netEntity.prefabName == "") return null;
        UnityNetEntity prefab;
        // проверяем есть ли у нас префаб с заданным именем
        if (!gos.TryGetValue(netEntity.prefabName, out prefab))
        {
            Debug.LogWarning("Не найдет префаб с именем " + netEntity.prefabName);
            return null;
        }
        // создаем нужный го
        UnityNetEntity go = Instantiate(prefab, netEntity.transform.position.Vector3(), netEntity.transform.rotation.Quaternion());       
        go.netEntity.transform.position = netEntity.transform.position;
        // возвращяем целевой экземпляр ClientNetEntity из нового go
        return go.netEntity;
    }
}
