using GalaxyCoreCommon.InternalMessages;
using GalaxyCoreLib;
using ProtoBuf.Meta;
using SimpleMmoCommon.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GalaxyNetworkController : MonoBehaviour
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

    public bool drawLables = true;

    public static GalaxyNetworkController api;
    private void Awake()
    {      
        /*
        if (GalaxyApi.connection.isConnected)
        {
            Destroy(gameObject);
            return;
        }     
        */
        if (api != null)
        {
            Destroy(gameObject);
            return;
        }       
        api = this;
        config.serverIp = serverIP; // задаем указаный ип
        config.serverPort = serverPort; // задаем указанный порт
        config.app_name = "SimpleMmoServer"; // должно соответствовать имени сервера
        config.FrameRate = 25; // Устанавливаем сетевой фреймрейт       
        //config.simulate_latency = 0.05f;
       GalaxyClientCore.Initialize(config); // инициализируем сетевое ядро        
        GalaxyClientCore.unityCalls.Awake(); // прокидываем Awake
        DontDestroyOnLoad(gameObject); // Помечаем объект как неразрушаемый при переходах между сценами      
       
    }
    private void Start()
    {       
        GalaxyClientCore.unityCalls.Start(); // прокидываем Start
        SceneManager.activeSceneChanged += SceneChanged;
        SceneManager.sceneLoaded += SceneLoaded;           
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        GalaxyClientCore.unityCalls.OnSceneLoaded();
    }

    private void SceneChanged(Scene arg0, Scene arg1)
    {
        GalaxyClientCore.unityCalls.OnSceneChange();
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
       // GalaxyApi.connection.Disconnect();//Посылаем команду дисконекта если go был выключен
    }
}
