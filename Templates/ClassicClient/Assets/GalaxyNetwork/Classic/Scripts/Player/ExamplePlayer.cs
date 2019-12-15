using GalaxyCoreCommon;
using GalaxyTemplateCommon.Messages;
using ProtoBuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplePlayer : MonoBehaviour
{
    public NetGO netGO;
    public Transform head;
    public Camera cam;
    private float mouseY;
    GoMessPlayerData lastData;
    public int heal;


    private void OnEnable()
    {
        netGO = GetComponent<NetGO>();
        //подписываемся на входящие сообщения
        netGO.OnMessageGo += OnMessageGo;
        //подписываемся на сетевой апдейт
        netGO.OnGoFrameUpdate += OnGoFrameUpdate;
        InvokeRepeating("RandHeal", 1, 1);
        lastData = new GoMessPlayerData();
    }
    
   

    private void OnDisable()
    {      
        netGO.OnMessageGo -= OnMessageGo;
        netGO.OnGoFrameUpdate -= OnGoFrameUpdate;
    }

    void RandHeal()
    {
        if (netGO.isMy)
        {
            heal = Random.Range((int)0, (int)10000);
        }
    }

    private void OnMessageGo(byte code, byte[] data)
    {
        switch ((CommandType)code)
        {
            case CommandType.MessPlayerData:
                //у нам пришел наше сообщение как пример передачи доп данных от объекта к объекту
                //распаковываем данные согласно тому типу который упаковали (в том числе для этого нужен код, или команда)
                GoMessPlayerData message = GoMessPlayerData.Deserialize<GoMessPlayerData>(data);
                //Поворачиваем голову согасно данным
                if(message.mouse!=0) head.transform.localRotation = Quaternion.Euler(message.mouse, 0, 0);
                if (message.heal != 0) heal = message.heal;
                break;
            default:
                break;
        }
    }

    private void OnGoFrameUpdate()
    {
        if (!netGO.isMy) return; // если это не наш объект то выходим
        GoMessPlayerData message = new GoMessPlayerData();
        bool send = false;
        if (lastData.mouse == head.transform.rotation.eulerAngles.x)
        {
            //если данные не изменились, то записываем значение 0
            //протобуф не будет добавлять в сериализацию дефолтные значения
            //так что 0 это по сути не отправлять данные
          message.mouse = 0;
        } else
        {
            message.mouse = head.transform.rotation.eulerAngles.x;
            lastData.mouse = message.mouse;
            send = true;
        }
        if (lastData.heal == heal)
        {            
            message.heal = 0;
        }
        else
        {
            message.heal = heal;
            lastData.heal = heal;
            send = true;
        }

        netGO.SendGoMessage((byte)CommandType.MessPlayerData,message);
    }


    void Update()
    {
        if (!netGO.isMy) return;
        cam.gameObject.SetActive(true);
        mouseY = -Input.GetAxis("Mouse Y") * 3;
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.position += (transform.forward * Input.GetAxis("Vertical"))*Time.deltaTime*5;
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.position += (transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * 5;
        }
        head.transform.Rotate(mouseY, 0, 0f);
        transform.Rotate(0, Input.GetAxis("Mouse X")*3, 0);
    }

    enum CommandType:byte
    {
        MessPlayerData = 1,
    }


}
/// <summary>
/// Пример кастомного класса с данными, насделуемся от BaseMessage для упращения серриализации
/// </summary>
[ProtoContract]
public class GoMessPlayerData : BaseMessage
{
    [ProtoMember(1)]
    public float mouse { get; set;}
    [ProtoMember(2)]
    public int heal { get; set; }

}
