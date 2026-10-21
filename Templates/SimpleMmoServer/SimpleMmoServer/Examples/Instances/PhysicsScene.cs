using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Physics;
using GalaxyCoreServer;
using GalaxyCoreServer.Physics;

namespace SimpleMmoServer.Examples.Instances
{
    /// <summary>Imports Unity's engine-neutral GPHB collision geometry into the optional physics backend.</summary>
    internal static class PhysicsScene
    {
        public static void LoadOrCreateFloor(Instance instance)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Content", "physics", "ExamplePhys2.gphb");
            if (File.Exists(path))
            {
                using var stream = File.OpenRead(path);
                Load(instance, GalaxyPhysicsBakeFormat.Load(stream));
                return;
            }

            // The old repository did not ship its .phys file. Keep the demo usable out of the box.
            new BakedCollider(instance, new GalaxyVector3(0, -0.5f, 0), GalaxyQuaternion.Identity,
                new PhysicsBodyOptions
                {
                    MotionType = PhysicsBodyMotionType.Static,
                    Shape = PhysicsShape.Box(new GalaxyVector3(200, 1, 200))
                }, "Example floor").Init();
            Log.Info(nameof(PhysicsScene), "Using the built-in floor. Export ExamplePhys2.gphb to Content/physics to load scene colliders.");
        }

        public static void Load(Instance instance, GalaxyPhysicsBake bake)
        {
            if (bake.CoordinateSystem != GalaxyPhysicsCoordinateSystem.LeftHandedYUp)
                throw new InvalidDataException("This Unity example requires a LeftHandedYUp GPHB bake.");
            float scale = 1f / bake.UnitsPerMeter;
            var shapes = bake.Shapes.ToDictionary(shape => shape.Id);
            var materials = bake.Materials.ToDictionary(material => material.Id);
            var compounds = bake.Compounds.ToDictionary(compound => compound.Id);
            var pending = new List<BakedCollider>();
            foreach (var collider in bake.Colliders)
            {
                if (!collider.Enabled) continue;
                var position = collider.Position * scale;
                var rotation = collider.Rotation;
                if (collider.CompoundId != 0)
                {
                    var compound = compounds[collider.CompoundId];
                    if (!compound.Enabled) continue;
                    position = compound.Position * scale + compound.Rotation * position;
                    rotation = compound.Rotation * rotation;
                }
                var shape = shapes[collider.ShapeId];
                if ((shape.Kind == GalaxyPhysicsShapeKind.Capsule || shape.Kind == GalaxyPhysicsShapeKind.Cylinder)
                    && shape.Axis != GalaxyPhysicsAxis.Y)
                {
                    var axis = shape.Axis == GalaxyPhysicsAxis.X ? GalaxyVector3.Right : GalaxyVector3.Forward;
                    rotation *= GalaxyQuaternion.FromToRotation(GalaxyVector3.Up, axis);
                }
                var options = new PhysicsBodyOptions
                {
                    MotionType = PhysicsBodyMotionType.Static,
                    Shape = CreateShape(bake, shape, scale),
                    Layer = collider.Layer,
                    CollisionMask = collider.CollisionMask,
                    IsTrigger = collider.IsTrigger
                };
                if (collider.MaterialId != 0)
                {
                    var material = materials[collider.MaterialId];
                    options.Friction = material.DynamicFriction;
                    options.Restitution = material.Restitution;
                }
                pending.Add(new BakedCollider(instance, position, rotation, options, collider.Tag));
            }
            foreach (var entity in pending) entity.Init();
        }

        private static PhysicsShape CreateShape(GalaxyPhysicsBake bake, GalaxyPhysicsShapeBake shape, float scale)
        {
            switch (shape.Kind)
            {
                case GalaxyPhysicsShapeKind.Box: return PhysicsShape.Box(shape.Size * scale);
                case GalaxyPhysicsShapeKind.Sphere: return PhysicsShape.Sphere(shape.Radius * scale);
                case GalaxyPhysicsShapeKind.Capsule: return PhysicsShape.Capsule(shape.Radius * scale, shape.Height * scale);
                case GalaxyPhysicsShapeKind.Cylinder: return PhysicsShape.Cylinder(shape.Radius * scale, shape.Height * scale);
                case GalaxyPhysicsShapeKind.ConvexHull:
                    return PhysicsShape.ConvexHull(GalaxyPhysicsGeometryCodec.DecodeVertices(bake, shape.Id)
                        .Select(vertex => vertex * scale).ToArray());
                case GalaxyPhysicsShapeKind.TriangleMesh:
                    var vertices = GalaxyPhysicsGeometryCodec.DecodeVertices(bake, shape.Id)
                        .Select(vertex => vertex * scale).ToArray();
                    var source = GalaxyPhysicsGeometryCodec.DecodeTriangleIndices(bake, shape.Id);
                    var indices = new int[source.Length * (shape.IsTwoSided ? 2 : 1)];
                    Array.Copy(source, indices, source.Length);
                    if (shape.Winding == GalaxyPhysicsMeshWinding.Clockwise)
                        for (int i = 0; i < source.Length; i += 3)
                            (indices[i + 1], indices[i + 2]) = (indices[i + 2], indices[i + 1]);
                    if (shape.IsTwoSided)
                        for (int i = 0; i < source.Length; i += 3)
                        {
                            indices[source.Length + i] = indices[i];
                            indices[source.Length + i + 1] = indices[i + 2];
                            indices[source.Length + i + 2] = indices[i + 1];
                        }
                    return PhysicsShape.TriangleMesh(vertices, indices);
                case GalaxyPhysicsShapeKind.HeightField:
                    var heights = GalaxyPhysicsGeometryCodec.DecodeHeights(bake, shape.Id);
                    // Both APIs store local heights; convert bake units to metres.
                    for (int i = 0; i < heights.Length; i++) heights[i] *= scale;
                    return PhysicsShape.HeightField(shape.HeightFieldHeight, shape.HeightFieldWidth, heights,
                        shape.HeightFieldSize.X * scale / (shape.HeightFieldWidth - 1),
                        shape.HeightFieldSize.Z * scale / (shape.HeightFieldHeight - 1));
                default: throw new InvalidDataException($"Unsupported baked shape: {shape.Kind}.");
            }
        }

        private sealed class BakedCollider : NetEntityStandard
        {
            private readonly PhysicsBodyOptions _options;
            public BakedCollider(Instance instance, GalaxyVector3 position, GalaxyQuaternion rotation,
                PhysicsBodyOptions options, string tag) : base(instance, position, rotation)
            {
                _options = options;
                IsStatic = true;
                // Static collision geometry is already present in the Unity scene.
                IncludeInOctree = false;
                PrefabName = tag;
            }
            protected override void Start() => Physics.Create(_options);
        }
    }
}
