using GalaxyCoreServer;
using GalaxyCoreCommon;

namespace SimpleMmoServer.Examples.NetEntities
{
    public class ExamplePet : NetEntity
    {
        public NetEntity Player;

        public ExamplePet(Instance instance, GalaxyVector3 position = default, GalaxyQuaternion rotation = default,
            NetEntityAutoSync syncType = NetEntityAutoSync.position_and_rotation) : base(instance, position, rotation,
            syncType)
        {
            PrefabName = "Pet";
        }

        public override void InMessage(byte externalCode, byte[] data, Client client)
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void Start()
        {
            transform.SyncType = NetEntityAutoSync.position_and_rotation;
        }

        protected override void Update()
        {
            if (GalaxyVector3.Distance(transform.Position, Player.transform.Position) < 2) return;
            transform.Position = GalaxyVector3.Lerp(transform.Position, Player.transform.Position,
                Instance.Time.DeltaTime * 0.7f);
        }
    }
}