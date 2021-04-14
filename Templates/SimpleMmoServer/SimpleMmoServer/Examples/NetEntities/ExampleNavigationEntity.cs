using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using GalaxyCoreServer;
using System;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleNavigationEntity : NetEntity, IGalaxyPathResult
    {
        public readonly NetEntity Target;
        public readonly float Speed = 10;
        private GalaxyVector3 _lastTargetPosition;
        private GalaxyPath _path;
        private readonly GalaxyMap _map;
        private readonly NavigationMask _mask;
        private int _currentNodeIndex;
        private GalaxyVector3 _targetPoint;
        private float _lastDelta;

        public ExampleNavigationEntity(Instance instance, NetEntity target, GalaxyMap map) : base(instance, default,
            default, NetEntityAutoSync.position_and_rotation)
        {
            PrefabName = "NavBot";
            Target = target;
            _map = map;
            _mask = new NavigationMask();
            _mask.AddLayer(new byte[] {1, 2, 3, 4, 5});
            _mask.MaxLiftAngle = 1;
            _targetPoint = transform.Position;
        }

        public override void InMessage(byte externalCode, byte[] data, Client clientSender)
        {
        }

        protected override void OnDestroy()
        {
        }

        public void OnGalaxyPathResult(GalaxyPath path)
        {
            _path = path;
            if (path.Status != PathStatus.Complete) return;
            if (path.Nodes == null) return;
            _lastDelta = float.MaxValue;
            _targetPoint = new GalaxyVector3(path.Nodes[0].X, path.Nodes[0].Y, path.Nodes[0].Z);
            var message = new BitGalaxy();
            foreach (var node in path.Nodes)
            {
                message.WriteValue(new GalaxyVector3(node.X, node.Y, node.Z));
            }

            SendMessage(1, message.Data, GalaxyDeliveryType.reliable);
        }

        protected override void Start()
        {
        }

        protected override void Update()
        {
            if (Target == null) return;
            if (GalaxyVector3.Distance(Target.transform.Position, _lastTargetPosition) > 3)
            {
                _lastTargetPosition = Target.transform.Position;
                _map.GetPath(transform.Position, Target.transform.Position, this, _mask);
                _currentNodeIndex = 0;
            }

            if (_path.Nodes == null) return;
            if (_currentNodeIndex >= _path.Nodes.Count) return;
            _targetPoint = new GalaxyVector3(_path.Nodes[_currentNodeIndex].X, _path.Nodes[_currentNodeIndex].Y,
                _path.Nodes[_currentNodeIndex].Z);
            float delta = Math.Abs((_targetPoint - transform.Position).Summ());
            if (delta < _lastDelta)
            {
                _lastDelta = delta;
                transform.Position =
                    GalaxyVector3.Move(transform.Position, _targetPoint, Speed, Instance.Time.DeltaTime);
            }
            else
            {
                _lastDelta = float.MaxValue;
                _currentNodeIndex++;
            }
        }
    }
}