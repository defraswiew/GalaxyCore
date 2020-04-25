using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoBuf;
using SimpleMmoCommon.Messages;
using GalaxyCoreCommon;
using GalaxyCoreCommon.InternalMessages;

public class ProtoTest : MonoBehaviour
{
    public Text text;

    public void TEST(string inp)
    {
      
       

        MessageAuth messageAuth = new MessageAuth();     
       
        messageAuth.login = inp;
        messageAuth.password = inp;
        byte[] data = messageAuth.Serialize();
        Debug.Log("data.Length " + data.Length);

        MessageAuth newMessage = BaseMessage.Deserialize<MessageAuth>(data);
        text.text = messageAuth.login;


        MessInstanceCreate message;
         message = new MessInstanceCreate();
        message.name = inp;
        message.maxClients = 1;
        message.type = 1;
        message.data = new byte[1];
        byte[] sendData = message.Serialize();
        Debug.Log("MessInstanceCreate.Length " + sendData.Length);
        MessInstanceCreate sss = MessInstanceCreate.Deserialize< MessInstanceCreate>(sendData);
    }

    public byte[] Serrialize(BaseMessage message)
    {
        return message.Serialize();
    }

}
