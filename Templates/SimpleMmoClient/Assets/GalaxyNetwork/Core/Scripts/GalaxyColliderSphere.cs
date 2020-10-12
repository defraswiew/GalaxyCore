using GalaxyCoreCommon;
using UnityEngine;

/// <summary>
/// Сетевой коллайдер (сфера)
/// </summary>
public class GalaxyColliderSphere : MonoBehaviour
{
    /// <summary>
    /// Текущий коллайдер
    /// </summary>
    private SphereCollider col;
    /// <summary>
    /// размер коллайдера
    /// </summary>
    private float size = 1;
    /// <summary>
    /// Физический тег (можно получить на сервере при коллизии)
    /// </summary>
    public string physTag = "";

#if UNITY_EDITOR
    /// <summary>
    /// Рисуем коллайдер
    /// </summary>
    private void OnDrawGizmos()
    {
        if (col == null) col = GetComponent<SphereCollider>();
        GetSize();
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, size + 0.3f);
    }
#endif
    /// <summary>
    /// Запекание коллайдера
    /// </summary>
    /// <returns></returns>
    public PhysSphereCollider Bake()
    {
        PhysSphereCollider bake = new PhysSphereCollider();
        bake.position = transform.position.NetworkVector3();
        bake.radius = size;
        bake.tag = physTag;
        if (physTag == "") bake.tag = transform.name;
        return bake;
    }

    private float GetSize()
    {
        size = col.radius;
        size = size * transform.localScale.x + 0.1f;
        return size;
    }
}
