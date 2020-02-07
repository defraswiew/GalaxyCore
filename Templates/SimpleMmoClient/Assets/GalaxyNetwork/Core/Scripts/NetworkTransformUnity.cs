using GalaxyCoreCommon;
using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreCommon.NetEntity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GalaxyCoreLib
{ 
public class NetworkTransformUnity: GalaxyTransform
    {
        /// <summary>
        /// Трансформ от юнити
        /// </summary>
        private Transform transform;
        private Vector3 oldPosition;
        private Quaternion oldRotation;
        private Vector3 oldScale;
        private MessInternalTransform message;
        UnityNetObject netEntity;

        public NetworkTransformUnity(Transform transform, UnityNetObject netEntity)
        {
            this.transform = transform;
            this.netEntity = netEntity;
        }

        public void SyncTransform(bool position, bool rotation, bool scale)
        {
            if (position || rotation || scale)
            {
                message = new MessInternalTransform();
                bool update = false;

                if (position) if (SyncPosition()) update = true;
                if (rotation) if (SyncRotation())update = true;
                if (scale) if (SyncScale()) update = true;

                if (update)
                {
                    netEntity.SendMessage((byte)InternalMessageCodes.netEntitySyncTransform, message, GalaxyDeliveryType.unreliableNewest);           
                }
            }  
            
        }

        public bool SyncPosition()
        {
            if (oldPosition == transform.position) return false;
            position = transform.position.NetworkVector3();
            message.position = position;
            oldPosition = transform.position;
            return true;
        }

        public bool SyncRotation()
        {
            if (oldRotation == transform.rotation) return false;
            rotation = transform.rotation.NetworkQuaternion();
            message.rotation = rotation;
            oldRotation = transform.rotation;
            return true;
        }

        public bool SyncScale()
        {
            if (oldScale == transform.localScale) return false;
            scale = transform.localScale.NetworkVector3();
            message.scale = scale;
            oldScale = transform.localScale;
            return true;
        }
    }
}
