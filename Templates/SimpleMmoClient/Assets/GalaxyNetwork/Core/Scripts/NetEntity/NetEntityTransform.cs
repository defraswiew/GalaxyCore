using GalaxyCoreCommon;
using GalaxyCoreLib.Api;
using UnityEngine;
using GalaxyCoreLib.NetEntity;

namespace GalaxyCoreLib
{
    /// <summary>
    /// Базовый компонент - пример реализации сетевого перемещения
    /// </summary>
    [RequireComponent(typeof(UnityNetEntity))]
    public class NetEntityTransform : MonoBehaviour
    {
        /// <summary>
        ///  Ссылка на сетевую сущность в ядре
        /// </summary>
        private ClientNetEntity netEntity;
        /// <summary>
        /// отправлять ли позицию
        /// </summary>
        public bool sendMyPosition;
        /// <summary>
        /// Отправлять ли поворот
        /// </summary>
        public bool sendMyRotation;

        private void Awake()
        {
            // получаем ссылку на сетевую сущность
            netEntity = GetComponent<UnityNetEntity>().netEntity;
        }

        private void OnEnable()
        {
            // подписывемся на событие сетевого тика
            GalaxyEvents.OnFrameUpdate += OnFrameUpdate;
        }
        private void OnDisable()
        {
            // отписываемся от сетевого тика
            GalaxyEvents.OnFrameUpdate -= OnFrameUpdate;
        }
        private void OnFrameUpdate()
        {
            // если объект пренадлежит не нам, не отправляем данные
            // в прочем если их отправить, сервер все равно их отбросит
            if (!netEntity.isMy) return;
            // записываем в сетевую сущность новые координаты
            if (sendMyPosition) netEntity.transform.SendPosition(transform.position.NetworkVector3());
            // записываем в сетевую сущность новый ротейшен
            if (sendMyRotation) netEntity.transform.SendRotation(transform.rotation.NetworkQuaternion());

            // Не обязательно делать это в сетевом тике, как часто бы мы не обновляли данные в сущности,
            // они не уйдут раньше наступления клиентского сетевого фрейма
            // однако разумнее обновлять данные в момент надобности, а именно в сетевом тике
        }

        public void Update()
        {
            // если сущность наша то нам не следует обрабатывать входящую информацию о положении
            if (!netEntity.isMy)
            {
                // проверяем не пустые ли входящие данные
                if (netEntity.transform.position != GalaxyVector3.Zero)
                {
                    // интерполируем текущую позицию в сетевую позицию согласно GalaxyApi.lerpDelta (расчитывается на основе фреймрейта и fps)
                    transform.position = Vector3.Lerp(transform.position, netEntity.transform.position.Vector3(), GalaxyApi.lerpDelta);
                }
                // пороверяем не пустой ли rotation 
                if (!netEntity.transform.rotation.isZero())
                {
                    // интерполируем поворот объекта
                    transform.rotation = Quaternion.Lerp(transform.rotation, netEntity.transform.rotation.Quaternion(), GalaxyApi.lerpDelta);
                }
            }
        }
    }
}
