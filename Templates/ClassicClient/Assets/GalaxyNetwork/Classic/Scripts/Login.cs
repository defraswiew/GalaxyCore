using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GalaxyCoreLib;
using GalaxyCoreLib.Api;
using GalaxyTemplateCommon.Messages;
public class Login : MonoBehaviour
{
    public InputField login; 
    public InputField password;
    public Text status;
    private void OnEnable()
    {
        Events.OnGalaxyApprovalResponse += OnGalaxyApprovalResponse; //подписка на событие об ошибках авторизации
        Events.OnGalaxyConnect += OnGalaxyConnect; // собыьте успешного коннекта
    }

    private void OnGalaxyConnect(byte[] message)
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Events.OnGalaxyApprovalResponse -= OnGalaxyApprovalResponse;
        Events.OnGalaxyConnect -= OnGalaxyConnect;
    }

    private void OnGalaxyApprovalResponse(byte code, string message)
    {
        status.text = message;
    }

    /// <summary>
    /// Метод авторизации вызываемый из UI
    /// </summary>
    public void Auth()
    {
        //Создаем новое сообщение аунтефикации, которое мы положили в GalaxyTemplateCommon
        MessageAuth messageAuth = new MessageAuth();
        if (login.text.Length < 4)
        {
            status.text = "Какой то очень короткий логин";
            return;
        }
        status.text = "Подключение...";
        messageAuth.login = login.text;
        messageAuth.password = password.text;
        GalaxyApi.connection.Connect(messageAuth); // Отправляем запрос на сервер
    }
    
}
