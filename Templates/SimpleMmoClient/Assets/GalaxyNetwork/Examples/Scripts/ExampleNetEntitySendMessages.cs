using GalaxyCoreCommon;
using GalaxyCoreLib.NetEntity;
using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Examples.Scripts
{
    public class ExampleNetEntitySendMessages : MonoBehaviour
    {
        public float InstanceLiveTime;
        private ClientNetEntity _netEntity;
        private void Start()
        {
            _netEntity = GetComponent<UnityNetEntity>().NetEntity;
            _netEntity.OnInMessage += InMessage;
        }

        private void OnDestroy()
        {
            _netEntity.OnInMessage -= InMessage;
        }

        private void InMessage(byte code, byte[] data)
        {
            switch (code)
            {
                case 1:
                {
                    var message = new BitGalaxy(data);
                    Debug.Log("Send to Owner: Server Date " + message.ReadString());
                }
                    break;
                case 2:
                {
                    var message = new BitGalaxy(data);
                    InstanceLiveTime = message.ReadFloat();
                }
                    break;
            }
        }

 
    }
}
