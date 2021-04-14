using GalaxyCoreCommon;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts
{
    /// <summary>
    /// Сетевой коллайдер (сфера)
    /// </summary>
    public class GalaxyColliderSphere : MonoBehaviour
    {
        /// <summary>
        /// Текущий коллайдер
        /// </summary>
        private SphereCollider _collider;

        /// <summary>
        /// размер коллайдера
        /// </summary>
        private float _size = 1;

        /// <summary>
        /// Физический тег (можно получить на сервере при коллизии)
        /// </summary>
        public string PhysTag = "";

#if UNITY_EDITOR
        /// <summary>
        /// Рисуем коллайдер
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_collider == null) _collider = GetComponent<SphereCollider>();
            GetSize();
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.4f);
            Gizmos.DrawSphere(transform.position, _size + 0.3f);
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
            bake.radius = _size;
            bake.tag = PhysTag;
            if (PhysTag == "") bake.tag = transform.name;
            return bake;
        }

        private float GetSize()
        {
            _size = _collider.radius;
            _size = _size * transform.localScale.x + 0.1f;
            return _size;
        }
    }
}