using GalaxyCoreCommon;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "d3d04a2439110f336a560af6fd3185dde5ddcd98")]
public class UnigineNetEntity : Component
{
    /// <summary>
    /// Link to a network entity in the core of the galaxy network
    /// Сылка на сетеву сущность в ядре
    /// </summary>
    public ClientNetEntity netEntity;
    private void Init()
    {
        if (netEntity == null)
        {
            // Create a new entity if it hasn't been done before
            // Создаем новую сущность, если не сделали этого ранее
            netEntity = new ClientNetEntity();
            // Apply the current coordinates
            // Применяем текущие координаты
            netEntity.transform.position = NetworkVector3(node.WorldPosition);
            // Set the name by which we will search for the asset
            // Задаем имя по которому будем искать ассет
            netEntity.prefabName = node.Name;
            // Sending a request to create an entity
            // Отправляем запрос на создание сетевой сущности
            GalaxyApi.netEntity.Instantiate(netEntity);
        }
        // Network tick event (start of network frame)
        // Собитие сетевого тика
        GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
    }

    private void OnFrameUpdate()
    {
        // Checking who belongs to the entity
        // Проверяем кому пренадлежит сущность
        if (!netEntity.isMy) return;

        // We write down the position and rotation of the node in the network entity.This data is automatically sent in the nearest network frame
        netEntity.transform.SendPosition(NetworkVector3(node.WorldPosition));
        netEntity.transform.SendRotation(NetworkQuaternion(node.GetWorldRotation()));
    }

    private void Update()
    {
        if (!netEntity.isMy)
        {
            // Interpolating position and rotation
            // Интерполируем позицию и поворот
            node.Position = vec3.Lerp(node.Position, Vector3(netEntity.transform.position), GalaxyApi.lerpDelta);
            quat rot = Unigine.MathLib.Slerp(node.GetRotation(), Quaternion(netEntity.transform.rotation), GalaxyApi.lerpDelta);
            node.SetRotation(rot);
        }

    }


    private GalaxyVector3 NetworkVector3(vec3 vector3)
    {
        GalaxyVector3 mes = new GalaxyVector3();
        mes.x = vector3.x;
        mes.z = vector3.y;
        mes.y = vector3.z;
        return mes;
    }

    private vec3 Vector3(GalaxyVector3 vector)
    {
        vec3 tmp = new vec3();
        tmp.x = vector.x;
        tmp.y = vector.z;
        tmp.z = vector.y;
        return tmp;
    }

    private GalaxyQuaternion NetworkQuaternion(quat quaternion)
    {
        GalaxyQuaternion mes = new GalaxyQuaternion();
        mes.x = quaternion.x;
        mes.y = quaternion.y;
        mes.z = quaternion.z;
        mes.w = quaternion.w;
        return mes;
    }
    private quat Quaternion(GalaxyQuaternion quaternion)
    {
        quat tmpQ = new quat();
        tmpQ.x = quaternion.x;
        tmpQ.y = quaternion.y;
        tmpQ.z = quaternion.z;
        tmpQ.w = quaternion.w;
        return tmpQ;
    }

}