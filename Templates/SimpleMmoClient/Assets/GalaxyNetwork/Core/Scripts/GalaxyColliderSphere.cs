using GalaxyCoreCommon.Physics;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    [RequireComponent(typeof(SphereCollider))]
    public class GalaxyColliderSphere : MonoBehaviour
    {
        public string PhysTag = "";

        public GalaxyPhysicsBake Bake()
        {
            var bake = GalaxyPhysicsBakeUtility.CreateBake();
            Bake(bake);
            return bake;
        }

        public void Bake(GalaxyPhysicsBake bake)
        {
            var source = GetComponent<SphereCollider>();
            var scale = GalaxyPhysicsBakeUtility.AbsoluteScale(transform);
            float radius = source.radius * Mathf.Max(scale.x, scale.y, scale.z);
            uint id = (uint)bake.Shapes.Count + 1;
            bake.Shapes.Add(GalaxyPhysicsShapeBake.Sphere(id, radius));
            GalaxyPhysicsBakeUtility.AddCollider(bake, source, id,
                transform.TransformPoint(source.center), Quaternion.identity, PhysTag, source.isTrigger);
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var source = GetComponent<SphereCollider>();
            if (source == null) return;
            var scale = transform.lossyScale;
            float radius = source.radius * Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
            Gizmos.DrawSphere(transform.TransformPoint(source.center), radius);
        }
#endif
    }
}
