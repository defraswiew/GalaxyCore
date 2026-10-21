using System;
using System.Collections.Concurrent;
using System.Threading;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Physics;
using System.Collections.Generic;
using GalaxyCoreServer;
using SimpleMmoCommon.Messages;
using SimpleMmoCommon.RPGTemplate;
using SimpleMmoServer.Examples.Instances;
using SimpleMmoServer.Examples.NetEntities;
using SimpleMmoServer.RPGTemplate;

namespace SimpleMmoServer
{
    /// <summary>A bounded local SDK migration check, invoked with --smoke-test.</summary>
    internal static class SmokeTest
    {
        private static GalaxyPhysicsBake CreateTestBake()
        {
            var bake = new GalaxyPhysicsBake { CoordinateSystem = GalaxyPhysicsCoordinateSystem.LeftHandedYUp };
            bake.Shapes.Add(GalaxyPhysicsShapeBake.Box(1, new GalaxyVector3(1, 1, 1)));
            bake.Shapes.Add(GalaxyPhysicsShapeBake.Sphere(2, 0.5f));
            bake.Shapes.Add(GalaxyPhysicsShapeBake.Capsule(3, 0.5f, 2f, GalaxyPhysicsAxis.X));
            bake.Shapes.Add(GalaxyPhysicsShapeBake.Cylinder(4, 0.5f, 2f, GalaxyPhysicsAxis.Z));
            GalaxyPhysicsGeometryCodec.AddConvexHull(bake, 5, new[]
            {
                new GalaxyVector3(0, 0, 0), new GalaxyVector3(1, 0, 0),
                new GalaxyVector3(0, 1, 0), new GalaxyVector3(0, 0, 1)
            });
            GalaxyPhysicsGeometryCodec.AddTriangleMesh(bake, 6, new[]
            {
                new GalaxyVector3(0, 0, 0), new GalaxyVector3(1, 0, 0), new GalaxyVector3(0, 0, 1)
            }, new[] { 0, 1, 2 }, GalaxyPhysicsMeshWinding.Clockwise, true);
            GalaxyPhysicsGeometryCodec.AddHeightField(bake, 7, 2, 2,
                new[] { 0f, 0.25f, 0.5f, 0.75f }, new GalaxyVector3(1, 1, 1));
            for (uint shapeId = 1; shapeId <= 7; shapeId++)
                bake.Colliders.Add(new GalaxyPhysicsColliderBake
                {
                    Id = shapeId, ShapeId = shapeId, Position = new GalaxyVector3(shapeId * 10, 0, 0),
                    Tag = "GPHB smoke " + shapeId
                });
            bake.Compounds.Add(new GalaxyPhysicsCompoundBake { Id = 1, Position = new GalaxyVector3(80, 0, 0) });
            bake.Colliders.Add(new GalaxyPhysicsColliderBake
            {
                Id = 8, ShapeId = 2, CompoundId = 1, Position = new GalaxyVector3(0, 1, 0), Tag = "GPHB compound"
            });
            return bake;
        }
        public static void Run()
        {
            var errors = new ConcurrentQueue<string>();
            void OnError(string source, string message) => errors.Enqueue(source + ": " + message);
            Log.OnLogError += OnError;
            try
            {
                var auth = new MessageAuth { Login = "test-smoke", Password = "example" };
                if (GalaxyMessage.Deserialize<MessageAuth>(auth.Serialize()).Login != auth.Login)
                    throw new InvalidOperationException("Authentication serialization failed.");
                var drop = new DropList { Items = new System.Collections.Generic.List<int> { 1, 2, 3 } };
                if (GalaxyMessage.Deserialize<DropList>(drop.Serialize()).Items.Count != 3)
                    throw new InvalidOperationException("Drop list serialization failed.");

                Instance[] rooms =
                {
                    new ExampleEmptyInstance(), new ExampleRoomMovers(), new ExampleRoomPhys(),
                    new ExampleRoomPhys2(), new ExampleNavigation(), new ExampleInvoke(),
                    new ExampleNetVisible(), new Location()
                };
                foreach (var room in rooms)
                {
                    room.Name = room.GetType().Name;
                    room.AutoClose = false;
                    GalaxyCore.Instances.Create(room);
                }
                var physics = (ExampleRoomPhys)rooms[2];
                var sphere = new ExampleSphere(physics, new GalaxyVector3(0, 5, 0), GalaxyQuaternion.Identity);
                var box = new ExamplePhysBox(physics, new GalaxyVector3(3, 5, 0), GalaxyQuaternion.Identity);
                var capsule = new ExamplePhysCapsule(physics, new GalaxyVector3(6, 5, 0), GalaxyQuaternion.Identity);
                var force = new ExampleForce(physics, new GalaxyVector3(9, 5, 0), GalaxyQuaternion.Identity) { Room = physics };
                foreach (var entity in new NetEntity[] { sphere, box, capsule, force,
                    new ExampleVideo(physics), new ExampleNetVars(physics), new RPGTemplatePlayer(physics) })
                    entity.Init();

                var bakedRoom = (ExampleRoomPhys2)rooms[3];
                var bake = CreateTestBake();
                PhysicsScene.Load(bakedRoom, GalaxyPhysicsBakeFormat.Load(GalaxyPhysicsBakeFormat.Save(bake)));
                Thread.Sleep(3500);
                if (bakedRoom.Entities.Count < 9)
                    throw new InvalidOperationException("GPHB static scene did not initialize every shape.");
                foreach (var entity in bakedRoom.Entities.List)
                    if (!entity.Physics.IsCreated)
                        throw new InvalidOperationException("GPHB body did not initialize: " + entity.PrefabName);
                foreach (var room in rooms)
                {
                    if (room.IsSleep) throw new InvalidOperationException(room.Name + " unexpectedly stopped.");
                    if (room.Physics.LastError != null) throw room.Physics.LastError;
                }
                foreach (var entity in new NetEntity[] { sphere, box, capsule, force })
                    if (!entity.Physics.IsCreated) throw new InvalidOperationException("Physics body failed: " + entity.GetType().Name);
                if (errors.TryPeek(out var error)) throw new InvalidOperationException(error);
                Console.WriteLine("SMOKE PASS: 8 rooms, 4 dynamic physics bodies, GPHB primitives/mesh/hull/terrain/compound, GalaxyVars, authentication and drop serialization.");
            }
            finally { Log.OnLogError -= OnError; }
        }
    }
}
