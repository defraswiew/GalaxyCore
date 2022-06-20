using GalaxyNetwork.Core.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace GalaxyCoreLib
{
    /// <summary>
    /// Окошко статистики
    /// </summary>
    public class GalaxyUIStatistic : MonoBehaviour
    {
        [SerializeField]
        private Text inTraffic;
        [SerializeField]
        private Text inSpeed;
        [SerializeField]
        private Text outTraffic;
        [SerializeField]
        private Text outSpeed;
        [SerializeField]
        private Text entity;
        [SerializeField]
        private Text ping;  
        private GalaxyConnection _connection;
        void Start()
        {
            _connection = GalaxyNetworkController.Api.MainConnection;
            // вызываем обновление окна 2 раза в сек
            InvokeRepeating("Tick", 0.5f, 0.5f);
        }

        void Tick()
        {
            // если подключения нет, выходим
            if (!_connection.Api.Transport.IsConnected) return;
            // устанавливаем входящий трафик
            inTraffic.text = System.Math.Round((_connection.Api.Transport.Statistic.InTraffic / 1024f) / 1024f, 2) + " MB";
            // объем входящих данных за секунду
            inSpeed.text = System.Math.Round(_connection.Api.Transport.Statistic.InTrafficInSecond / 1024f, 2) + " KB";
            // объем исходщяего трафика
            outTraffic.text = System.Math.Round((_connection.Api.Transport.Statistic.OutTraffic / 1024f) / 1024f, 2) + " MB";
            // объем исходщяего в секунду
            outSpeed.text = System.Math.Round(_connection.Api.Transport.Statistic.OutTrafficInSecond / 1024f, 2) + " KB";
            // число сетевых объектов 
            entity.text = _connection.Api.NetEntity.Count.ToString();
            // GalaxyApi.connection.statistic.ping это PingPong в микро секундах, нам нужна половина умноженная на 1000
            // значит для получения нормального понятного пинга надо умножить на 500 (1000/2)
            ping.text = System.Math.Round(_connection.Api.Transport.Statistic.Ping * 500, 2) + " ms";
        }
    }
}
