using GalaxyCoreLib.Api;
using GalaxyCoreLib.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Core.Scripts.NetEntity
{
    /// <summary>
    /// Basic component - an example of implementing a network move
    /// </summary>
    [RequireComponent(typeof(UnityNetEntity))]
    public class NetEntityTransform : MonoBehaviour
    {
        /// <summary>
        ///  Link to a network entity in the kernel
        /// </summary>
        private ClientNetEntity _netEntity;

        /// <summary>
        /// whether to send a position
        /// </summary>
        public bool SendMyPosition;

        /// <summary>
        /// Whether to send a turn
        /// </summary>
        public bool SendMyRotation;
        /// <summary>
        /// Whether to send a turn
        /// </summary>
        public bool SendMyScale;

        /// <summary>
        /// interpolation method
        /// </summary>
        public InterpolationType Interpolation;
     
        public float TeleportDistance = 20;
        private Vector3 _remotePosition;
        
        private void Awake()
        {
            // получаем ссылку на сетевую сущность
            _netEntity = GetComponent<UnityNetEntity>().NetEntity;
        }

        private void OnEnable()
        {
            // подписывемся на событие сетевого тика
            _netEntity.Connection.Events.OnFrameUpdate += OnFrameUpdate;
            _netEntity.Connection.Events.OnEngineUpdate += SoftUpdate;
        }

        private void OnDisable()
        {
            // отписываемся от сетевого тика
            _netEntity.Connection.Events.OnFrameUpdate -= OnFrameUpdate;
            _netEntity.Connection.Events.OnEngineUpdate -= SoftUpdate;
        }

        private void OnFrameUpdate()
        {
            // если объект пренадлежит не нам, не отправляем данные
            // в прочем если их отправить, сервер все равно их отбросит
            if (!_netEntity.IsMy) return;
            // записываем в сетевую сущность новые координаты
            if (SendMyPosition) _netEntity.transform.Position = transform.position.NetworkVector3();
            // записываем в сетевую сущность новый ротейшен
            if (SendMyRotation) _netEntity.transform.Rotation = transform.rotation.NetworkQuaternion();
            if (SendMyScale) _netEntity.transform.Scale = transform.localScale.NetworkVector3();

            // Не обязательно делать это в сетевом тике, как часто бы мы не обновляли данные в сущности,
            // они не уйдут раньше наступления клиентского сетевого фрейма
            // однако разумнее обновлять данные в момент надобности, а именно в сетевом тике
        }

        private void SoftUpdate(float delta)
        {
            if (!_netEntity.IsInit) return;
            if (!_netEntity.IsMy)
            {
                _remotePosition = _netEntity.transform.Position.Vector3();
                transform.localScale = _netEntity.transform.Scale.Vector3();
                if ((_remotePosition - transform.position).sqrMagnitude > TeleportDistance)
                {
                    transform.position = _netEntity.transform.Position.Vector3();
                    transform.rotation = _netEntity.transform.Rotation.Quaternion();
                    return;
                }
                switch (Interpolation)
                {
                    case InterpolationType.None:
                        transform.position = _netEntity.transform.Position.Vector3();
                        transform.rotation = _netEntity.transform.Rotation.Quaternion();
                        return;
                       
                    case InterpolationType.Soft:
                        _netEntity.transform.InterpolateSoft();
                        break;
                    case InterpolationType.EndPont:
                        _netEntity.transform.InterpolateEndPoint();
                        break;
                }
                
                transform.position = _netEntity.transform.InterpolatePosition.Vector3();
                transform.rotation = _netEntity.transform.InterpolateRotation.Quaternion();
                
            }
        }

        public enum InterpolationType
        {
            None,
            Soft,
            EndPont,
        }
    }
}