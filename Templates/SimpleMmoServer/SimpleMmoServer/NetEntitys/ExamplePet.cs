﻿using GalaxyCoreCommon;
using GalaxyCoreServer;
using GalaxyCoreServer.Api;
using GalaxyCoreServer.Physics;
using SimpleMmoCommon;
using SimpleMmoCommon.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleMmoServer.NetEntitys
{
    public class ExamplePet : NetEntity
    {
        private NetEntity target;
        private ClientConnection ownerConnection;
        float speed = 2;

        public override void InMessage(byte externalCode, byte[] data, ClientConnection client)
        {
       
        }

        public override void OnDestroy()
        {
           
        }

        public override void Start()
        {
            ColliderBox collider = new ColliderBox(new GalaxyVector3(0.4f, 0.4f, 0.4f));
            physics.Activate(collider);
            //physics.isStatic = false;
            ownerConnection = instance.GetClientById(owner); 
        }

        public override void Update()
        {
            physics.SetPhys();
            if (target == null)
            {        
                FindTarget();
                return;
            }

            if (GalaxyVector3.SqrMagnitude(position - target.position) < 2) return;
            position = GalaxyVector3.Lerp(position, target.position, instance.Time.deltaTime * speed);
            MessageTransform message = new MessageTransform();
            message.position = position;           
            SendMessage((byte)NetEntityCommand.syncTransform, message, GalaxyDeliveryType.unreliableNewest);           
        }

        private void FindTarget()
        {
            if (ownerConnection == null) return;
            target = ownerConnection.GetUserEntities().Where(x => x.name == "Player").FirstOrDefault();
        }

    }
}