using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using SimpleMmoCommon;
using SimpleMmoCommon.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMmoServer.NetEntitys
{
    public class ExampleMonster : NetEntity
    {
        private NetEntity target;     
        private GalaxyVector3 startPosition;

        public ExampleMonster()
        {
            name = "Monster";
            position = new GalaxyVector3();
         //   rotation = new GalaxyQuaternion();
        }

        public override void InMessage(byte externalCode, byte[] data, ClientConnection client)
        {
          
        }

        public override void OnDestroy()
        {
          
        }

        public override void Start()
        {
            startPosition = position;
        }

        public override void Update()
        {
            if(target == null)
            {
                FindTarget();
                return;
            } 
            if (GalaxyVector3.SqrMagnitude(target.position - position) > 8)
            {
                target = null;               
                return;
            }
         
            position = GalaxyVector3.Lerp(position, target.position, instance.Time.deltaTime);
            MessageTransform message = new MessageTransform();
            message.position = position;
            SendMessage((byte)NetEntityCommand.syncTransform, message, GalaxyDeliveryType.unreliableNewest);
        }

        private void FindTarget()
        {
            foreach (var item in instance.entities.list.Where(x => x.name == "Player"))
            {
                if (GalaxyVector3.SqrMagnitude(item.position - position) < 4) target = item;
            }
            if (GalaxyVector3.SqrMagnitude(position - startPosition) < 2) return;
            position = GalaxyVector3.Lerp(position, startPosition, instance.Time.deltaTime);
            MessageTransform message = new MessageTransform();
            message.position = position;
            SendMessage((byte)NetEntityCommand.syncTransform, message, GalaxyDeliveryType.unreliableNewest);
        }
    }
}
