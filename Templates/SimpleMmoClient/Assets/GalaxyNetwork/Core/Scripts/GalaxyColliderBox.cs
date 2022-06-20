using GalaxyCoreCommon;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    /// <summary>
    /// Сетевой коллайдер (box)
    /// </summary>
    public class GalaxyColliderBox : MonoBehaviour
    {
        /// <summary>
        /// Бокс коллайдер
        /// </summary>
        private BoxCollider _boxCollider;
        /// <summary>
        /// Позиция
        /// </summary>
        private Vector3 _position;
        /// <summary>
        /// размер
        /// </summary>
        private Vector3 _size;
        /// <summary>
        /// Физический тег (можно получить на сервере при коллизии)
        /// </summary>
        public string PhysTag = "";
        /// <summary>
        /// Является ли коллайдер триггером
        /// </summary>
        public bool IsTrigger = false;
        /// <summary>
        /// Положение, поворот и скейл в виде матрички) 
        /// </summary>
        private Matrix4x4 _parentMatrix;

#if UNITY_EDITOR
        /// <summary>
        /// Рисуем коллайдер в дебаге
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_boxCollider) _boxCollider = GetComponent<BoxCollider>();
            if (!_boxCollider) return;       
            _parentMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            _position = _parentMatrix.MultiplyPoint3x4(_boxCollider.center);
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
            if (IsTrigger) Gizmos.color = new Color(0.9f, 0.5f, 0.0f, 0.3f);
            Matrix4x4 cubeTransform = Matrix4x4.TRS(_position, _boxCollider.transform.rotation, GetSize());        
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix *= cubeTransform;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.9f);
            if(IsTrigger) Gizmos.color = new Color(1f, 0.7f, 0.1f, 0.6f);
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
            Debug.Log(_boxCollider.transform.rotation);
            bake.position = _position.NetworkVector3();
            bake.rotation = _boxCollider.transform.rotation.NetworkQuaternion();
            bake.size = _size.NetworkVector3();
            bake.trigger = IsTrigger;
            bake.tag = PhysTag;
            if (PhysTag == "") bake.tag = transform.name;
            return bake;
        }
    
        /// <summary>
        /// получаем размер для отрисовки
        /// </summary>
        /// <returns></returns>
        private Vector3 GetSize()
        {
            _size = _boxCollider.transform.lossyScale;
            _size.x = _size.x * _boxCollider.size.x + 0.1f;
            _size.y = _size.y * _boxCollider.size.y + 0.1f;
            _size.z = _size.z * _boxCollider.size.z + 0.1f;       
            return _size;
        }   
    }
}
