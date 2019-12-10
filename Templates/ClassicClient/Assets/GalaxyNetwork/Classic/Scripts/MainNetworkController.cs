using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
public class MainNetworkController : MonoBehaviour
{
    /// <summary>
    /// Ip адрес сервера
    /// </summary>
    public string serverIP = "127.0.0.1";
    /// <summary>
    /// Порт сервера
    /// </summary>
    public int serverPort = 30200;
    /// <summary>
    /// Конфигурация клиента
    /// </summary>
    Config config = new Config();
    private void Awake()
    {      
        config.serverIp = serverIP; // задаем указаный ип
        config.serverPort = serverPort; // задаем указанный порт
        config.app_name = "ClassicTemplate"; // должно соответствовать имени сервера
        config.FrameRate = 20; // Устанавливаем сетевой фреймрейт
        GalaxyClientCore.Initialize(config); // инициализируем сетевое ядро
        GalaxyClientCore.unityCalls.Awake(); // прокидываем Awake
        DontDestroyOnLoad(gameObject); // Помечаем объект как неразрушаемый при переходах между сценами        
    }
    private void Start()
    {
        GalaxyClientCore.unityCalls.Start(); // прокидываем Start
    }


    private void Update()
    {
        GalaxyClientCore.unityCalls.Update(Time.deltaTime); // Прокидываем update
    }


    private void OnApplicationQuit()
    {
        GalaxyApi.connection.Disconnect(); //Посылаем команду дисконекта при выходе из приложения
    }

    private void OnDisable()
    {
        GalaxyApi.connection.Disconnect();//Посылаем команду дисконекта если go был выключен
    }

}
