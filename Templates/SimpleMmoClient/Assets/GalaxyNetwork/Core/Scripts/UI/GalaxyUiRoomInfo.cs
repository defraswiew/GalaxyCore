using GalaxyCoreLib;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyUiRoomInfo : MonoBehaviour
{
    /// <summary>
    /// Ссылкан на информацию о типе комнаты
    /// </summary>
    public GalaxyInstanceInfoScriptable myInfo;
    /// <summary>
    /// Куда выводим картиночку
    /// </summary>
    Image img;
    public void Click()
    {
        // передаем информацию о клике в ведущее окно
        GalaxyUIRoomCreate.api.SelectedInfo(myInfo);
    }
    /// <summary>
    /// Инициализация строки
    /// </summary>
    /// <param name="info">Ссылкан на информацию о комнате</param>
    public void Init(GalaxyInstanceInfoScriptable info)
    {
        myInfo = info;
        img = GetComponent<Image>();
        img.sprite = info.img;
    }
}

