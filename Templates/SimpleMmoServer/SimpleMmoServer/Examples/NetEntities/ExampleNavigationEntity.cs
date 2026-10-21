using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;
using GalaxyCoreCommon.Navigation;
using GalaxyCoreServer;
using System;

#if GALAXY_DOUBLE
using Scalar = System.Double;
using Vector = GalaxyCoreCommon.GalaxyVectorD3;
#else
using Scalar = System.Single;
using Vector = GalaxyCoreCommon.GalaxyVector3;
#endif
namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExampleNavigationEntity : NetEntity, IGalaxyPathResult
    {
        public readonly NetEntity Target;
        public readonly float Speed = 10;
        private Vector _lastTargetPosition;
        private GalaxyPath _path;
        private readonly GalaxyMap _map;
        private readonly NavigationMask _mask;
        private int _currentNodeIndex;
        private Vector _targetPoint;
        private Scalar _lastDelta;

        public ExampleNavigationEntity(Instance instance, NetEntity target, GalaxyMap map) : base(instance, default,
            default)
        {
            PrefabName = "NavBot";
            Target = target;
            _map = map;
            _mask = new NavigationMask();
            _mask.AddLayer(new byte[] {1, 2, 3, 4, 5});
            _mask.MaxLiftAngle = 1;
            _targetPoint = transform.Position;
        }

        public override void InMessage(byte externalCode, byte[] data, BaseClient clientSender)
        {
        }

        protected override void OnDestroy(byte[] customData)
        {
        }

        public void OnGalaxyPathResult(GalaxyPath path)
        {
            _path = path;
            if (path.Status != PathStatus.Complete) return;
            if (path.Nodes == null) return;
            _lastDelta = float.MaxValue;
            _targetPoint = new Vector(path.Nodes[0].X, path.Nodes[0].Y, path.Nodes[0].Z);
            var message = new GalaxyBitWriter(new byte[path.Nodes.Count * 12]);
            foreach (var node in path.Nodes)
            {
                message.WriteVector3(new GalaxyVector3(node.X, node.Y, node.Z));
            }

            SendMessage(1, message.WrittenSpan.ToArray(), GalaxyDeliveryType.reliable);
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

        protected override void Update()
        {
            if (Target == null) return;
            if (Vector.Distance(Target.transform.Position, _lastTargetPosition) > 3)
            {
                _lastTargetPosition = Target.transform.Position;
                _map.GetPath(transform.Position, Target.transform.Position, this, _mask);
                _currentNodeIndex = 0;
            }

            if (_path.Nodes == null) return;
            if (_currentNodeIndex >= _path.Nodes.Count) return;
            _targetPoint = new Vector(_path.Nodes[_currentNodeIndex].X, _path.Nodes[_currentNodeIndex].Y,
                _path.Nodes[_currentNodeIndex].Z);
            var delta = (_targetPoint - transform.Position).SqrMagnitude;
            if (delta < _lastDelta)
            {
                _lastDelta = delta;
                transform.Position =
                    Vector.Move(transform.Position, _targetPoint, Speed, Instance.Time.DeltaTime);
            }
            else
            {
                _lastDelta = float.MaxValue;
                _currentNodeIndex++;
            }
        }
    }
}
