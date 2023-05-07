using System;
using System.Collections.Generic;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using GalaxyCoreServer;
using SimpleMmoServer.Examples.Instances;
#if GALAXY_DOUBLE
using vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using vector = GalaxyCoreCommon.GalaxyVector3;
#endif
namespace SimpleMmoServer.Examples.NetEntities
{
    public class NavigationMob:NetEntity,IGalaxyPathResult
    {
        private readonly GalaxyMap _map;
        private readonly  NavigationMask _navigationMask;
        private Queue<vector> _points;
        private vector _currentPoint;
        private float _speed = 3;
        private Random _rnd;
        private GalaxyVector3[] _spawnPoints = new[]
        {
            new GalaxyVector3(-31,0,-10),
            new GalaxyVector3(-5,0,-32),
            new GalaxyVector3(-15,0,-17),
            new GalaxyVector3(-17,5,18),
            new GalaxyVector3(-32,0,-28),
            new GalaxyVector3(-40,3.5f,26),
            new GalaxyVector3(-40,3.5f,-23),
            new GalaxyVector3(24,10.8f,10),
            new GalaxyVector3(35,10.8f,-17),
        };
        
        public NavigationMob(ExampleNavigation instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = new GalaxyQuaternion()) : base(instance, position, rotation)
        {
            _map = instance.Map;
            PrefabName = "Move";
            _navigationMask = new NavigationMask
            {
                Lianer = false, 
                PreLiner = PreLinerType.OneLayer, 
                IgnoreCash = true,
                MaxLiftAngle = 50
            };
            _navigationMask.AddLayer(1);
            _navigationMask.AddLayer(2);
            _navigationMask.AddLayer(3);
            _currentPoint = transform.Position;
            _points = new Queue<vector>();
            _rnd = new Random();
        }

        protected override void OnRemotePosition(GalaxyVector3 remotePosition)
        {
             
        }

        protected override void OnRemoteScale(GalaxyVector3 remoteScale)
        {
             
        }

        protected override void OnRemoteRotation(GalaxyQuaternion remoteRotation)
        {
             
        }

        protected override void Start()
        {
            
        }

        protected override void OnDestroy()
        {
           
        }

        protected override void Update()
        {
            if ((_currentPoint - transform.Position).SqrMagnitude > 0.5f)
            {
                transform.Position = vector.Move(transform.Position,_currentPoint + vector.Up*0.5f,_speed, Instance.Time.DeltaTime);
            }
            else
            {
                if (_points.Count > 0)
                {
                    _currentPoint = _points.Dequeue();
                }
                else
                {
                    PathRequest();
                }
            }
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
            
        }
        
        private void PathRequest()
        {
            var endPoint = _spawnPoints[_rnd.Next(0, _spawnPoints.Length - 1)];

          _map.GetPath(transform.Position,endPoint,this,_navigationMask);
        }

        public void OnGalaxyPathResult(GalaxyPath path)
        {
            _points.Clear();
            if (path.Nodes != null && path.Nodes.Count > 0)
            {
                foreach (var node in path.Nodes)
                {
                    var point = new vector(node.X, node.Y, node.Z);
                    _points.Enqueue(point);
                }
            }
        }
    }
}