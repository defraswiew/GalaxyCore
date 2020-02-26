using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Сетевой коллайдер (box)
/// </summary>
public class GalaxyColliderBox : MonoBehaviour
{
    public BoxCollider boxCollider;
    private Vector3 position;
    private Vector3 size;
    private void OnDrawGizmos()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider) return;
     


        /*
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
        
              Matrix4x4 cubeTransform = Matrix4x4.TRS(GetPosition(), boxCollider.transform.localRotation, GetSize());
      //  Matrix4x4 cubeTransform = boxCollider.transform.localToWorldMatrix;
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
        Gizmos.matrix *= cubeTransform;
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.9f);
        Gizmos.matrix = oldGizmosMatrix;
        Gizmos.matrix *= cubeTransform;
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.9f);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldGizmosMatrix;
        */

        
    }

    private Vector3 GetSize()
    {
        Vector3 size = boxCollider.transform.lossyScale;
        size.x = size.x * boxCollider.size.x + 0.1f;
        size.y = size.y * boxCollider.size.y + 0.1f;
        size.z = size.z * boxCollider.size.z + 0.1f;
        this.size = size;
        return size;
    }
    private Vector3 GetPosition()
    {
        Vector3 position = boxCollider.transform.position;
        Vector3 size = GetSize();
        position.x = position.x + (boxCollider.center.x);
        position.y = position.y + (boxCollider.center.y);
        position.z = position.z + (boxCollider.center.z);
        this.position = position;
        return position;
    }

}
