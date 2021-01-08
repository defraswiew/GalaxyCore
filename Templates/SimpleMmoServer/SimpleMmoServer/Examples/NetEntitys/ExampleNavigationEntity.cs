using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using GalaxyCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleMmoServer.Examples.NetEntitys
{
    public class ExampleNavigationEntity : NetEntity, IGalaxyPathResult
    {
        public NetEntity target;
        public float speed = 10;
        private GalaxyVector3 lastTargetPosition;
        private GalaxyPath path;
        private GalaxyMap map;
        private NavigationMask mask;
        private int currentNodeIndex;
        private GalaxyVector3 targetPoint;
        private float lastDelta;
        public ExampleNavigationEntity(Instance instance, NetEntity target, GalaxyMap map) : base(instance, default, default, NetEntityAutoSync.position_and_rotation)
        {
            prefabName = "NavBot";
            this.target = target;
            this.map = map;
            mask = new NavigationMask();
            mask.AddLayer(new byte[] { 1, 2, 3, 4, 5 });
            mask.maxLiftAngle = 1;
            targetPoint = transform.position;
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
            
        }

        public override void OnDestroy()
        {
           
        } 

        public void OnGalaxyPathResult(GalaxyPath path)
        {
            this.path = path;
            if (path.Status != PathStatus.complete) return;
            if (path.Nodes == null) return;
            lastDelta = float.MaxValue;
            targetPoint = new GalaxyVector3(path.Nodes[0].x, path.Nodes[0].y, path.Nodes[0].z);
            BitGalaxy message = new BitGalaxy();
            foreach (var node in path.Nodes)
            {
                message.WriteValue(new GalaxyVector3(node.x, node.y, node.z));
            }
            SendMessage(1, message.data, GalaxyDeliveryType.reliable);
        }

        public override void Start()
        {
           
        }

        public override void Update()
        {
            /*
            if (target == null) return;
            if (GalaxyVector3.Distance(target.transform.position, lastTargetPosition) > 3)
            {
                lastTargetPosition = target.transform.position;
                map.GetPath(transform.position, target.transform.position, this, mask);
                currentNode = 1;               
            }
            if (path.Nodes == null) return;
            if (currentNode >= path.Nodes.Count) return;
            var node = path.Nodes[currentNode];
            var movePoint = new GalaxyVector3(node.x, node.y, node.z);
            if (GalaxyVector3.Distance(target.transform.position, movePoint) > speed)
            {               
                transform.position = GalaxyVector3.Move(transform.position, movePoint, speed,0.1f);
            } else
            {
                currentNode++;
            }
            */


            if (target == null) return;
            if (GalaxyVector3.Distance(target.transform.position, lastTargetPosition) > 3)
            {
                lastTargetPosition = target.transform.position;
                map.GetPath(transform.position, target.transform.position, this, mask);
                currentNodeIndex = 0;          
            }
            if (path.Nodes == null) return;
            if (currentNodeIndex >= path.Nodes.Count) return;
            targetPoint = new GalaxyVector3(path.Nodes[currentNodeIndex].x, path.Nodes[currentNodeIndex].y, path.Nodes[currentNodeIndex].z);
            float delta = Math.Abs((targetPoint - transform.position).Summ());
            if (delta < lastDelta)
            {
                lastDelta = delta;
                transform.position = GalaxyVector3.Move(transform.position, targetPoint, speed, instance.Time.deltaTime);
            } else
            {
                lastDelta = float.MaxValue;
                currentNodeIndex++;
            }
        }
       
    }
}
