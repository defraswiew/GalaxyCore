using GalaxyCoreCommon;
using GalaxyCoreLib.Api;
using SimpleMmoCommon.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace GalaxyCoreLib
{ 
public class GalaxyUILogin : MonoBehaviour
{
    [Header("Событие при успешной авторизации")]
    public UnityEvent OnLoginSucesseEvent;
    [Header("Событие ошибки авторизации")] 
    public UnityOnLoginError OnLoginErrorEvent;

    public GameObject[] enableObject;
    public GameObject[] disableObject;
    [Space(15)]
    public InputField login;
    public InputField password;
    public Text status;
    public GameObject progress;
        private int count = 1; 

    private void OnEnable()
    {
            //подписка на событие об ошибках авторизации
            GalaxyEvents.OnGalaxyApprovalResponse += OnGalaxyApprovalResponse;
            // событие успешного коннекта      
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect;
            progress.SetActive(false);
        }

  


        private void OnGalaxyConnect(byte[] message)
        {
            CancelInvoke("Waiting");
            if (OnLoginSucesseEvent != null) OnLoginSucesseEvent.Invoke();
            MessageApproval messageApproval = new MessageApproval();
            messageApproval = BaseMessage.Deserialize<MessageApproval>(message);
            Debug.Log("Наше имя " + messageApproval.name);
            gameObject.SetActive(false);
            for (int i = 0; i < enableObject.Length; i++)
            {
                enableObject[i].SetActive(true);
            }
            for (int i = 0; i < disableObject.Length; i++)
            {
                disableObject[i].SetActive(false);
            }
        }

        private void OnDisable()
        {
            GalaxyEvents.OnGalaxyApprovalResponse -= OnGalaxyApprovalResponse;
            GalaxyEvents.OnGalaxyConnect -= OnGalaxyConnect;
        }

        private void OnGalaxyApprovalResponse(byte code, string message)
        {
            status.text = message;
            progress.SetActive(false);
            if (OnLoginErrorEvent != null) OnLoginErrorEvent.Invoke(code, message);
        }

        public void Auth()
        {
            CancelInvoke("Waiting");
            //Создаем новое сообщение аунтефикации, которое мы положили в GalaxyTemplateCommon
            MessageAuth messageAuth = new MessageAuth();
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
          if(count==1)  status.text = "Подключение...";
            messageAuth.login = login.text;
            messageAuth.password = password.text;
            byte[] data = messageAuth.Serialize();
            GalaxyApi.connection.Connect(data); // Отправляем запрос на сервер    
            progress.SetActive(true);
            Debug.Log("Client version: " + GalaxyClientCore.version.ToString());
            Invoke("Waiting", 5f);
        }
        public void DropCount()
        {
             count = 1;
        }

        /// <summary>
        /// Ожидание подключения
        /// </summary>
        private void Waiting()
        {
            count++;
            if (count > 3)
            {
                status.text = "Подключиться не удалось";
                progress.SetActive(false);
                return;
            }
            status.text = "Попытка № " + count;
            Auth();
        }

    }
}
/// <summary>
/// Эвент ошибки авторизации
/// </summary>
[System.Serializable]
public class UnityOnLoginError : UnityEvent<byte, string>
{

}