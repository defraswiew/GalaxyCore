using GalaxyCoreCommon.Physics;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    [RequireComponent(typeof(BoxCollider))]
    public class GalaxyColliderBox : MonoBehaviour
    {
        public string PhysTag = "";
        public bool IsTrigger = false;

        public GalaxyPhysicsBake Bake()
        {
            var bake = GalaxyPhysicsBakeUtility.CreateBake();
            Bake(bake);
            return bake;
        }

        public void Bake(GalaxyPhysicsBake bake)
        {
            var source = GetComponent<BoxCollider>();
            var size = Vector3.Scale(source.size, GalaxyPhysicsBakeUtility.AbsoluteScale(transform));
            uint id = (uint)bake.Shapes.Count + 1;
            bake.Shapes.Add(GalaxyPhysicsShapeBake.Box(id, size.NetworkVector3()));
            GalaxyPhysicsBakeUtility.AddCollider(bake, source, id,
                transform.TransformPoint(source.center), transform.rotation, PhysTag, IsTrigger || source.isTrigger);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var source = GetComponent<BoxCollider>();
            if (source == null) return;
            var previous = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
            Gizmos.DrawCube(source.center, source.size);
            Gizmos.matrix = previous;
        }
#endif
    }
}
