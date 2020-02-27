using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Сетевой коллайдер (box)
/// </summary>
public class GalaxyColliderBox : MonoBehaviour
{
    private BoxCollider boxCollider;
    private Vector3 position;
    private Vector3 size;
 
 
 

    Matrix4x4 parentMatrix;


    private void OnDrawGizmos()
    {
        if (!boxCollider) boxCollider = GetComponent<BoxCollider>();
        if (!boxCollider) return;

       
        parentMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        position = parentMatrix.MultiplyPoint3x4(boxCollider.center);

        Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
        
        Matrix4x4 cubeTransform = Matrix4x4.TRS(position, boxCollider.transform.localRotation, GetSize());        
        Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
        Gizmos.matrix *= cubeTransform;
     Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.9f);
        Gizmos.matrix = oldGizmosMatrix;
        Gizmos.matrix *= cubeTransform;
        Gizmos.color = new Color(1f, 0.8f, 0f, 0.9f);
       Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = oldGizmosMatrix;

        
            

 
 

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

    Vector3 DivideVectors(Vector3 num, Vector3 den)
    {

        return new Vector3(num.x / den.x, num.y / den.y, num.z / den.z);

    }
}
