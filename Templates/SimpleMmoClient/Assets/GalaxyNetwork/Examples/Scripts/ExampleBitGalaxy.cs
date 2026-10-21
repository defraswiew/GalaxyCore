using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;
using GalaxyCoreLib.NetEntity;
using GalaxyNetwork.Core.Scripts;
using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Examples.Scripts
{
    /// <summary>
    /// Пример работы с GalaxyBitBuffer (имя компонента сохранено для существующих сцен)
    /// </summary>
    public class ExampleBitGalaxy : MonoBehaviour
    {
        /// <summary>
        /// ссылка на энтити
        /// </summary>
        private ClientNetEntity netEntity;
        /// <summary>
        /// последний размер для сравнения
        /// </summary>
        private Vector3 scale;
        /// <summary>
        /// цвет объекта
        /// </summary>
        public Color color;
        /// <summary>
        /// последний цвет
        /// </summary>
        private Color _color;
        /// <summary>
        /// текущий материал
        /// </summary>
        private Material mat;


        void OnEnable()
        {
            // получаем ссыль на энтити
            netEntity = GetComponent<UnityNetEntity>().NetEntity;
            // подписываемся на получение сообщений
            netEntity.OnInMessage += OnInMessage;
            // получаем текущий материал
            mat = GetComponent<MeshRenderer>().material;
        }
        void OnDisable()
        {
            // отписываемся от получения сообщений
            netEntity.OnInMessage -= OnInMessage;
        }


        private void OnInMessage(byte code, byte[] data)
        {
            // если объект наш то нас не интересует этот блок
            if (netEntity.IsMy) return;
            switch (code)
            {
                case 1:
                {
                    // создаем контейнер на основе имеющихся данных
                    var buffer = GalaxyBitBuffer.From(data);
                    // читаем ожидаемый вектор и приводим его в юньковскому вектору
                    transform.localScale = buffer.ReadVector3().Vector3();
                }
                    break;
                case 2:
                {
                    var buffer = GalaxyBitBuffer.From(data);
                    // читаем данные в той же последовательности что и записывали
                    color.r = buffer.ReadSingle();
                    color.g = buffer.ReadSingle();
                    color.b = buffer.ReadSingle();
                    color.a = buffer.ReadSingle();
                    mat.color = color;
                }
                    break;
            }
        }

        void Update()
        {
            // если объект не наш то мы не можем ничего менять
            if (!netEntity.IsMy) return;
            // проверяем изменился ли размер
            if (scale != transform.localScale)
            {
                scale = transform.localScale;
                // создаем пустой контейнер
                var buffer = GalaxyBitBuffer.Create(32);
                // пишем в него вектор
                buffer.WriteVector3(new GalaxyVector3(scale.x, scale.y, scale.z));
                // отправляем сообщение
                netEntity.SendMessage(1, buffer.WrittenSpan.ToArray());
            }

            // проверяем цвет
            if(_color != color)
            {
                _color = color;
                // создаем контейнер
                var buffer = GalaxyBitBuffer.Create(32);

                buffer.WriteSingle(color.r);
                buffer.WriteSingle(color.g);
                buffer.WriteSingle(color.b);
                buffer.WriteSingle(color.a);
                netEntity.SendMessage(2, buffer.WrittenSpan.ToArray());
                mat.color = color;
            }
        }
    }
}
