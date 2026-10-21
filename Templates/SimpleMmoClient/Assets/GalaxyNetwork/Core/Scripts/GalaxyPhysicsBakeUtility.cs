using System;
using GalaxyCoreCommon.Physics;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    /// <summary>Unity-specific source metadata for the engine-neutral GPHB container.</summary>
    public static class GalaxyPhysicsBakeUtility
    {
        public static GalaxyPhysicsBake CreateBake() => new GalaxyPhysicsBake
        {
            CoordinateSystem = GalaxyPhysicsCoordinateSystem.LeftHandedYUp,
            UnitsPerMeter = 1f
        };

        public static GalaxyPhysicsColliderBake AddCollider(GalaxyPhysicsBake bake,
            Collider source, uint shapeId, Vector3 position, Quaternion rotation, string tag,
            bool isTrigger)
        {
            uint mask = 0;
            for (int layer = 0; layer < 32; layer++)
                if (!Physics.GetIgnoreLayerCollision(source.gameObject.layer, layer)) mask |= 1u << layer;
            uint materialId = 0;
            var material = source.sharedMaterial;
            if (material != null)
            {
                var existing = bake.Materials.Find(item => item.Name == material.name &&
                    item.StaticFriction == material.staticFriction && item.DynamicFriction == material.dynamicFriction &&
                    item.Restitution == material.bounciness &&
                    item.FrictionCombine == Combine(material.frictionCombine) &&
                    item.RestitutionCombine == Combine(material.bounceCombine));
                if (existing != null) materialId = existing.Id;
                else
                {
                    materialId = (uint)bake.Materials.Count + 1;
                    bake.Materials.Add(new GalaxyPhysicsMaterialBake
                    {
                        Id = materialId, Name = material.name,
                        StaticFriction = material.staticFriction, DynamicFriction = material.dynamicFriction,
                        Restitution = material.bounciness, FrictionCombine = Combine(material.frictionCombine),
                        RestitutionCombine = Combine(material.bounceCombine)
                    });
                }
            }
            var result = new GalaxyPhysicsColliderBake
            {
                Id = (uint)bake.Colliders.Count + 1, ShapeId = shapeId,
                Position = position.NetworkVector3(), Rotation = rotation.NetworkQuaternion(),
                MaterialId = materialId, IsTrigger = isTrigger, Layer = (byte)source.gameObject.layer,
                CollisionMask = mask, Enabled = source.enabled && source.gameObject.activeInHierarchy,
                Tag = string.IsNullOrEmpty(tag) ? source.name : tag
            };
            bake.Colliders.Add(result);
            return result;
        }

        private static GalaxyPhysicsMaterialCombine Combine(PhysicsMaterialCombine mode)
        {
            switch (mode)
            {
                case PhysicsMaterialCombine.Minimum: return GalaxyPhysicsMaterialCombine.Minimum;
                case PhysicsMaterialCombine.Maximum: return GalaxyPhysicsMaterialCombine.Maximum;
                case PhysicsMaterialCombine.Multiply: return GalaxyPhysicsMaterialCombine.Multiply;
                default: return GalaxyPhysicsMaterialCombine.Average;
            }
        }

        public static Vector3 AbsoluteScale(Transform transform)
        {
            var scale = transform.lossyScale;
            if (scale.x == 0 || scale.y == 0 || scale.z == 0)
                throw new InvalidOperationException("Cannot bake zero-scale collider: " + transform.name);
            return new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        }
    }
}
