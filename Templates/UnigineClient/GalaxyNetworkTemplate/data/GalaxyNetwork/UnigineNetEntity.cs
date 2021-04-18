using GalaxyCoreCommon;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using Unigine;

namespace UnigineApp.data.GalaxyNetwork
{
    [Component(PropertyGuid = "d3d04a2439110f336a560af6fd3185dde5ddcd98")]
    public class UnigineNetEntity : Component
    {
        /// <summary>
        /// Link to a network entity in the core of the galaxy network
        /// </summary>
        public ClientNetEntity NetEntity;

        private void Init()
        {
            if (NetEntity == null)
            {
                // Create a new entity if it hasn't been done before
                NetEntity = new ClientNetEntity();
                // Apply the current coordinates
                NetEntity.transform.Position = NetworkVector3(node.WorldPosition);
                // Set the name by which we will search for the asset
                NetEntity.PrefabName = node.Name;
                // Sending a request to create an entity
                GalaxyApi.NetEntity.Instantiate(NetEntity);
            }

            // Network tick event (start of network frame)
            GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
        }

        private void NetworkInit()
        {
            if (NetEntity == null)
            {
                // Create a new entity if it hasn't been done before
                NetEntity = new ClientNetEntity();
                // Apply the current coordinates
                NetEntity.transform.Position = NetworkVector3(node.WorldPosition);
                // Set the name by which we will search for the asset
                NetEntity.PrefabName = node.Name;
                // Sending a request to create an entity
                GalaxyApi.NetEntity.Instantiate(NetEntity);
            }

            // Network tick event (start of network frame)
            GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
        }

        private void OnFrameUpdate()
        {
            // Checking who belongs to the entity
            if (!NetEntity.IsMy) return;

            // We write down the position and rotation of the node in the network entity.This data is automatically sent in the nearest network frame
            NetEntity.transform.SendPosition(NetworkVector3(node.WorldPosition));
            NetEntity.transform.SendRotation(NetworkQuaternion(node.GetWorldRotation()));
        }

        private void Update()
        {
            if (!NetEntity.IsMy)
            {
                NetEntity.transform.InterpolateEndPoint();
                node.Position = Vector3(NetEntity.transform.Position);
                node.SetRotation(Quaternion(NetEntity.transform.Rotation));
            }
        }


        private GalaxyVector3 NetworkVector3(vec3 vector3)
        {
            var mes = new GalaxyVector3
            {
                X = vector3.x,
                Z = vector3.y,
                Y = vector3.z
            };
            return mes;
        }

        private vec3 Vector3(GalaxyVector3 vector)
        {
            var tmp = new vec3
            {
                x = vector.X,
                y = vector.Z,
                z = vector.Y
            };
            return tmp;
        }

        private GalaxyQuaternion NetworkQuaternion(quat quaternion)
        {
            var mes = new GalaxyQuaternion
            {
                x = quaternion.x,
                y = quaternion.y,
                z = quaternion.z,
                w = quaternion.w
            };
            return mes;
        }

        private quat Quaternion(GalaxyQuaternion quaternion)
        {
            var tmpQ = new quat
            {
                x = quaternion.x,
                y = quaternion.y,
                z = quaternion.z,
                w = quaternion.w
            };
            return tmpQ;
        }
    }
}