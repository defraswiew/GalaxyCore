using GalaxyCoreCommon;
using GalaxyCoreLib.NetEntity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleBitGalaxy : MonoBehaviour
{
    /// <summary>
    /// ссылка на энтити
    /// </summary>
    ClientNetEntity netEntity;
    /// <summary>
    /// последний размер для сравнения
    /// </summary>
    Vector3 scale;
    /// <summary>
    /// цвет объекта
    /// </summary>
    public Color color;
    /// <summary>
    /// последний цвет
    /// </summary>
    Color _color;
    /// <summary>
    /// текущий материал
    /// </summary>
    Material mat;


    void OnEnable()
    {
        // получаем ссыль на энтити
        netEntity = GetComponent<UnityNetEntity>().netEntity;
        // подписываемся на получение сообщений
        netEntity.OnInMessage += OnInMessage;
        // получаем текущий материал
        mat = GetComponent<MeshRenderer>().material;
    }
    void OnDisable()
    {
        // отписываемся от получения сообщений
        netEntity.OnInMessage -= OnInMessage;
    }


    private void OnInMessage(byte code, byte[] data)
    {
        // если объект наш то нас не интересует этот блок
        if (netEntity.isMy) return;
        switch (code)
        {
            case 1:
                {
                    // создаем контейнер на основе имеющихся данных
                    BitGalaxy buffer = new BitGalaxy(data);
                    // читаем ожидаемый вектор и приводим его в юньковскому вектору
                    transform.localScale = buffer.ReadGalaxyVector3().Vector3();
                }
                break;
            case 2:
                {                     
                    BitGalaxy buffer = new BitGalaxy(data);
                    // читаем данные в той же последовательности что и записывали
                    color.r = buffer.ReadFloat();
                    color.g = buffer.ReadFloat();
                    color.b = buffer.ReadFloat();
                    color.a = buffer.ReadFloat();
                    mat.color = color;
                }
                break;
        }
    }

    void Update()
    {
        // если объект не наш то мы не можем ничего менять
        if (!netEntity.isMy) return;
        // проверяем изменился ли размер
        if (scale != transform.localScale)
        {          
            scale = transform.localScale;
            // создаем пустой контейнер
            BitGalaxy buffer = new BitGalaxy();
            // пишем в него вектор
            buffer.WriteValue(new GalaxyVector3(scale.x, scale.y, scale.z));
            // отправляем сообщение
            netEntity.SendMessage(1, buffer.data);
        }

        // проверяем цвет
        if(_color != color)
        {
            _color = color;
            // создаем контейнер
            BitGalaxy buffer = new BitGalaxy();      
            
            buffer.WriteValue(color.r);
            buffer.WriteValue(color.g);
            buffer.WriteValue(color.b);
            buffer.WriteValue(color.a);
            netEntity.SendMessage(2, buffer.data);

            mat.color = color;

        }
    }



}
