using GalaxyCoreCommon;
using GalaxyCoreCommon.Navigation;
using UnityEngine;

/// <summary>
/// Класс расширения функционала
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Преобразование юнити вектора в сетевой вектор
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    public static GalaxyVector3 NetworkVector3(this Vector3 vector3)
    {
        return new GalaxyVector3(vector3.x, vector3.y, vector3.z);
    }
    /// <summary>
    /// Преобразование сетевого вектора в юньковский
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 Vector3(this GalaxyVector3 vector)
    {
        return new Vector3(vector.x, vector.y, vector.z);
    }

    /// <summary>
    /// Приравнивание векторов
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="vector3"></param>
    public static void Vector3(this GalaxyVector3 vector, out Vector3 vector3)
    {
        vector3.x = vector.x;
        vector3.y = vector.y;
        vector3.z = vector.z;
    }

    /// <summary>
    /// Преобразование юнити кватерниона в сетевой
    /// </summary>
    /// <param name="quaternion"></param>
    /// <returns></returns>
    public static GalaxyQuaternion NetworkQuaternion(this Quaternion quaternion)
    {
        return new GalaxyQuaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
    /// <summary>
    /// Преобразование сетевого кватерниона в юньковский
    /// </summary>
    /// <param name="quaternion"></param>
    /// <returns></returns>
    public static Quaternion Quaternion(this GalaxyQuaternion quaternion)
    {
        return new Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
    public static void SetColor(this GalaxyMapLayer layer, Color32 color)
    {
        layer.b = color.b;
        layer.r = color.r;
        layer.g = color.g;
    }
    public static Color32 Color(this GalaxyMapLayer layer)
    {
        Color32 color = new Color32(layer.r,layer.g,layer.b,255);
        return color;
    }
    public static GalaxyMapLayer Copy(this GalaxyMapLayer layer)
    {
        var result = new GalaxyMapLayer();
        result.SetColor(layer.Color());
        result.cost = layer.cost;
        result.heightWalkable = layer.heightWalkable;
        result.exscind = layer.exscind;
        result.id = layer.id;
        result.name = layer.name;
        result.transperent = layer.transperent;
        result.excludeGraph = layer.excludeGraph;
        result.isWalkable = layer.isWalkable;
        return result;
    }

    public static void Apply(this GalaxyMapLayer layer, GalaxyMapLayer target)
    {
        layer.SetColor(target.Color());
        layer.cost = target.cost;
        layer.heightWalkable = target.heightWalkable;
        layer.exscind = target.exscind;
        layer.id = target.id;
        layer.name = target.name;
        layer.transperent = target.transperent;
        layer.excludeGraph = target.excludeGraph;
        layer.isWalkable = target.isWalkable;
    }

    public static void GetPath(this GalaxyMap map, Vector3 start, Vector3 end, IGalaxyPathResult listener, NavigationMask mask)
    {
        map.GetPath(start.NetworkVector3(),end.NetworkVector3(),listener,mask);
    }

    public static Vector3 GetPosition(this GalaxyNode node)
    {
        return new Vector3(node.x, node.y, node.z);
    }
}

