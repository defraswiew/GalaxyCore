using System.Collections.Generic;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;
using GalaxyCoreLib.NetEntity;
using GalaxyNetwork.Core.Scripts;
using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Examples.Scripts
{
    public class ExampleNavBotDebag : MonoBehaviour
    {
        private ClientNetEntity entity;
        private List<Vector3> points;
        void Start()
        {
            points = new List<Vector3>();
            entity = GetComponent<UnityNetEntity>().NetEntity;
            entity.OnInMessage+= EntityOnOnInMessage;
        }

        void OnDestroy()
        {
            if (entity != null) entity.OnInMessage -= EntityOnOnInMessage;
        }

        private void EntityOnOnInMessage(byte code, byte[] data)
        {
            if (code == 1)
            {
                var message = GalaxyBitBuffer.From(data);
                points.Clear();
                while (message.BitsRemaining >= 96)
                {
                    points.Add(message.ReadVector3().Vector3());
                }
                transform.LookAt(transform.position);
            }
        }


        void Update()
        {
            var lastPosition = transform.position;
            int i = 0;
            foreach (var point in points)
            {
                if(i>0)Debug.DrawLine(lastPosition,point);
                lastPosition = point;
                i++;
            }
        }
    }
}
