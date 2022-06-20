using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    /// <summary>
    /// Functionality extension class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converting a unity vector to a network vector
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static GalaxyVector3 NetworkVector3(this Vector3 vector3)
        {
            return new GalaxyVector3(vector3.x, vector3.y, vector3.z);
        }

        /// <summary>
        /// Convert network vector to unity vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Vector3(this GalaxyVector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Equating vectors
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="vector3"></param>
        public static void Vector3(this GalaxyVector3 vector, out Vector3 vector3)
        {
            vector3.x = vector.X;
            vector3.y = vector.Y;
            vector3.z = vector.Z;
        }

        /// <summary>
        /// Converting unity quaternion to network
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static GalaxyQuaternion NetworkQuaternion(this Quaternion quaternion)
        {
            return new GalaxyQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }

        /// <summary>
        /// Convert network quaternion to unity
        /// </summary>
        /// <param name="quaternion"></param>
        /// <returns></returns>
        public static Quaternion Quaternion(this GalaxyQuaternion quaternion)
        {
            return new Quaternion(quaternion.X, quaternion.Y, quaternion.Z, quaternion.W);
        }

        public static void SetColor(this GalaxyMapLayer layer, Color32 color)
        {
            layer.B = color.b;
            layer.R = color.r;
            layer.G = color.g;
        }

        public static Color32 Color(this GalaxyMapLayer layer)
        {
            var color = new Color32(layer.R, layer.G, layer.B, 255);
            return color;
        }

        public static GalaxyMapLayer Copy(this GalaxyMapLayer layer)
        {
            var result = new GalaxyMapLayer();
            result.SetColor(layer.Color());
            result.Cost = layer.Cost;
            result.HeightWalkable = layer.HeightWalkable;
            result.Cut = layer.Cut;
            result.Id = layer.Id;
            result.Name = layer.Name;
            result.Transparent = layer.Transparent;
            result.ExcludeGraph = layer.ExcludeGraph;
            result.IsWalkable = layer.IsWalkable;
            result.ProjectionLower = layer.ProjectionLower;
            result.IgnoreProjection = layer.IgnoreProjection;
            return result;
        }

        public static void Apply(this GalaxyMapLayer layer, GalaxyMapLayer target)
        {
            layer.SetColor(target.Color());
            layer.Cost = target.Cost;
            layer.HeightWalkable = target.HeightWalkable;
            layer.Cut = target.Cut;
            layer.Id = target.Id;
            layer.Name = target.Name;
            layer.Transparent = target.Transparent;
            layer.ExcludeGraph = target.ExcludeGraph;
            layer.IsWalkable = target.IsWalkable;
            layer.ProjectionLower = target.ProjectionLower;
            layer.IgnoreProjection = target.IgnoreProjection;
        }

        public static void GetPath(this GalaxyMap map, Vector3 start, Vector3 end, IGalaxyPathResult listener,
            NavigationMask mask)
        {
            map.GetPath(start.NetworkVector3(), end.NetworkVector3(), listener, mask);
        }

        public static Vector3 GetPosition(this GalaxyNode node)
        {
            return new Vector3(node.X, node.Y, node.Z);
        }

        public static Vector3 GetPosition(this WorkNode node)
        {
            return new Vector3(node.X, node.Y, node.Z);
        }
    }
}