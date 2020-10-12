using GalaxyCoreLib;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Основной сетевой контроллер 
/// </summary>
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
    private Config config = new Config();
    /// <summary>
    /// показывать ли метки над сущностями
    /// </summary>
    public bool drawLables = true;
    /// <summary>
    /// ссылка на самого себя
    /// </summary>
    public static GalaxyNetworkController api;

    private void Awake()
    {
        if (api != null)
        {
            Destroy(gameObject);
            return;
        }
        api = this;
        // записываем ип
        config.serverIp = serverIP;
        // записываем порт
        config.serverPort = serverPort;
        // задаем имя (должно соответствовать имени сервера)
        config.app_name = "SimpleMmoServer";
        // Устанавливаем дефлотный сетевой фреймрейт
        // Однако сервер может переназначать это значение в рамках инстансов
        config.FrameRate = 25;
        // инициализируем сетевое ядро   
        GalaxyClientCore.Initialize(config);
        // прокидываем Awake
        GalaxyClientCore.unityCalls.Awake();
        // Помечаем объект как неразрушаемый при переходах между сценами      
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        // прокидываем Start
        GalaxyClientCore.unityCalls.Start();
        // подписываемся на смену сцену
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
        // Прокидываем update
        GalaxyClientCore.unityCalls.Update(Time.deltaTime);
    }

    private void OnApplicationQuit()
    {
        //Посылаем команду дисконекта при выходе из приложения
        GalaxyApi.connection.Disconnect();
    }


}
