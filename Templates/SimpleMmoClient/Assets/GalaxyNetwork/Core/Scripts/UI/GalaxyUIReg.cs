using GalaxyCoreLib;
using SimpleMmoCommon.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyUIReg : MonoBehaviour
{
    public bool active = false;
    public InputField login;
    public InputField password;
    public InputField password2;
    public Text status;
    public GameObject progress;
   


    public void Reg()
    {
        if (!active)
        {
            status.text = "Регистрация отключена";
            return;
        }
        if (GalaxyApi.connection.isConnected)
        {
            status.text = "Подключение активно";
            return;
        }
        if (login.text.Length < 4)
        {
            status.text = "Какой то очень короткий логин";
            return;
        }
        if (password.text.Length < 4)
        {
            status.text = "А где пароль?";
            return;
        }
        if(password.text!= password2.text)
        {
            status.text = "Пароли не совпадают(";
            return;
        }
        MessageAuth messageAuth = new MessageAuth();
        messageAuth.login = login.text;
        messageAuth.password = password.text;
        byte[] data = messageAuth.Serialize();
        status.text = "Региструемся";
        GalaxyApi.connection.Registration(data); // Отправляем запрос на сервер    
        progress.SetActive(true);
    }

}
