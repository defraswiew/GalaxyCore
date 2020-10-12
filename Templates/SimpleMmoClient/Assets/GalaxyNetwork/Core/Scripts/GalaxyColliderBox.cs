using GalaxyCoreCommon;
using UnityEngine;
/// <summary>
/// Сетевой коллайдер (box)
/// </summary>
public class GalaxyColliderBox : MonoBehaviour
{
    /// <summary>
    /// Бокс коллайдер
    /// </summary>
    private BoxCollider boxCollider;
    /// <summary>
    /// Позиция
    /// </summary>
    private Vector3 position;
    /// <summary>
    /// размер
    /// </summary>
    private Vector3 size;
    /// <summary>
    /// Физический тег (можно получить на сервере при коллизии)
    /// </summary>
    public string physTag = "";
    /// <summary>
    /// Является ли коллайдер триггером
    /// </summary>
    public bool isTrigger = false;
    /// <summary>
    /// Положение, поворот и скейл в виде матрички) 
    /// </summary>
    private Matrix4x4 parentMatrix;

#if UNITY_EDITOR
    /// <summary>
    /// Рисуем коллайдер в дебаге
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider) return;       
        parentMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        position = parentMatrix.MultiplyPoint3x4(boxCollider.center);
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
        if (isTrigger) Gizmos.color = new Color(0.9f, 0.5f, 0.0f, 0.3f);
        Matrix4x4 cubeTransform = Matrix4x4.TRS(position, boxCollider.transform.rotation, GetSize());        
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
        Gizmos.matrix *= cubeTransform;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.9f);
        if(isTrigger) Gizmos.color = new Color(1f, 0.7f, 0.1f, 0.6f);
        Gizmos.matrix = oldGizmosMatrix;
        Gizmos.matrix *= cubeTransform; 
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldGizmosMatrix;
    }
#endif
    /// <summary>
    /// Запекаем коллайдер
    /// </summary>
    /// <returns></returns>
    public PhysBakeBoxCollider Bake()
    {
        PhysBakeBoxCollider bake = new PhysBakeBoxCollider();
        Debug.Log(boxCollider.transform.rotation);
        bake.position = position.NetworkVector3();
        bake.rotation = boxCollider.transform.rotation.NetworkQuaternion();
        bake.size = size.NetworkVector3();
        bake.trigger = isTrigger;
        bake.tag = physTag;
        if (physTag == "") bake.tag = transform.name;
        return bake;
    }
    
    /// <summary>
    /// получаем размер для отрисовки
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSize()
    {
        size = boxCollider.transform.lossyScale;
        size.x = size.x * boxCollider.size.x + 0.1f;
        size.y = size.y * boxCollider.size.y + 0.1f;
        size.z = size.z * boxCollider.size.z + 0.1f;       
        return size;
    }   
}
