using GalaxyCoreCommon;
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

}

