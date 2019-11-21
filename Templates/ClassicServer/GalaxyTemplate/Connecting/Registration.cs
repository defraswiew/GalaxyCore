using System;
using System.Collections.Generic;
using System.Text;
using GalaxyCoreServer.Api;
namespace GalaxyTemplate.Connecting
{
    /// <summary>
    /// Класс пример регистрации
    /// </summary>
  public class Registration
    {

        public Registration()
        {
            GalaxyEvents.OnGalaxyRegistration += OnGalaxyRegistration; // подписываемся на событие регистрации
        }

        /// <summary>
        /// Запрос регистрации
        /// </summary>
        /// <param name="approvalConnection">Неавторизированное подключение запрашивающее регистрацию</param>
        /// <param name="data">массив байт, данные приложеные к запросу</param>
        private void OnGalaxyRegistration(ApprovalConnection approvalConnection, byte[] data)
        {
            //но мы не станем пока что реализовывать регистрацию, поэтому просто её запретим для любых случаев
            approvalConnection.Deny(2, "Регистрация не активна");
            //Обратите внимание, даже вслучае успешной авторизации желательно отправить Deny пакет. а в качестве маркера успеха использовать какой либо код ответа
            // например approvalConnection.Deny(0, "Регистрация успешна");
        }
    }
}
