using GalaxyCoreCommon;
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
            _netEntity.OnInMessage -= InMessage;
        }
        public void SendToServer()
        {
            var message = new BitGalaxy();
            message.WriteValue("Hello from client");
            _netEntity.SendMessage(5, message, deliveryType, false, chennal);
        }

        private void InMessage(byte code, byte[] data)
        {
            if(code == 5)
            Debug.Log("InMessage: " + code);
        }
    }
}
