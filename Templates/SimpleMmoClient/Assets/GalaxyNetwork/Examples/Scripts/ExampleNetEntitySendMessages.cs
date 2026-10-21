using System;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;
using GalaxyCoreLib.NetEntity;
using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Examples.Scripts
{
    public class ExampleNetEntitySendMessages : MonoBehaviour
    {
        public float InstanceLiveTime;
        public int chennal;
        public GalaxyDeliveryType deliveryType;
        private ClientNetEntity _netEntity;
        private void Start()
        {
            _netEntity = GetComponent<UnityNetEntity>().NetEntity;
            _netEntity.OnInMessage += InMessage;
            InvokeRepeating("SendToServer",1,0.1f);
        }

        private void OnDestroy()
        {
            CancelInvoke();
            if (_netEntity != null) _netEntity.OnInMessage -= InMessage;
        }
        public void SendToServer()
        {
            if (_netEntity == null || !_netEntity.IsInit) return;
            var message = GalaxyBitBuffer.Create();
            message.WriteString("Hello from client".AsSpan());
            _netEntity.SendMessage(5, message.WrittenSpan.ToArray(), deliveryType, false, chennal);
        }

        private void InMessage(byte code, byte[] data)
        {
            if(code == 5)
            Debug.Log("InMessage: " + code);
        }
    }
}
