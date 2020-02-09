using GalaxyCoreLib;
using GalaxyCoreLib.NetEntity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityNetEntity : MonoBehaviour
{
    /// <summary>
    /// Ссылка на сетевую сущность в ядре
    /// </summary>
    public ClientNetEntity netEntity;


    void Start()
    {
        // Init необходимо вызывать именно в Start т.к внутренняя инициализация не успевает сработать к Awake или OnEnable
        Init();
    }

    void Init()
    {
        if (netEntity == null)
        {
            // если netEntity все еще null то это явно наш объект
            // и нужно отправлять запрос на создание сетевого экземпляра
            // создаем пустой экземпляр сетевой сущности
            netEntity = new ClientNetEntity();
            // убираем все лишнее из имени
            netEntity.name = gameObject.name.Split(new char[] { ' ', '(' })[0];
            // записываем текущую позицию и поворот
            netEntity.position = transform.position.NetworkVector3();
            netEntity.rotation = transform.rotation.NetworkQuaternion();
            // отправляем запрос на создание сетевого объекта
            GalaxyApi.netEntity.Instantiate(netEntity);
        }
        else
        {
            // если к моменту вызова start ид уже есть, значит это точно не наш объект
         //   isMy = false;
        }
        //    AddInterfaces();
        //  yield return null;
    }
}
