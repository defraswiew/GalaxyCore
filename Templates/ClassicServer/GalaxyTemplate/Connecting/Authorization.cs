using System;
using System.Collections.Generic;
using System.Text;
using GalaxyCoreServer.Api;
using GalaxyTemplateCommon.Messages;

namespace GalaxyTemplate.Connecting
{
    /// <summary>
    /// Пример реализации авторизации
    /// </summary>
   public class Authorization
    {
        /// <summary>
        /// Конструктор, просто для удобной подписки
        /// </summary>
        public Authorization()
        {
            GalaxyEvents.OnGalaxyConnect += OnGalaxyConnect; // подписываемся на событие
        }
        /// <summary>
        /// Сюда нам придет запрос на авторизацию
        /// </summary>
        /// <param name="approvalConnection">Временное не авторизированное соеденение</param>
        /// <param name="data">Массив байт, данные присланные вместе с запросом авторизации</param>
        private void OnGalaxyConnect(ApprovalConnection approvalConnection, byte[] data)
        {
            MessageAuth message = MessageAuth.Deserialize<MessageAuth>(data); //преобразовывем массив байт в читабельное сообщение            
            //тут нам следовало бы провести какую либо проверку правильности логина и пароля
            //Но сейчас мы не станем этого делать, и просто разрешить соеденение всем в чьих логинах содержится test
            if(message.login.Contains("test"))
            {
                MessageFirst response = new MessageFirst(); //Создадим пакет который мы отправим клиенту вместе с разрешением коннекта
                response.id = DataBaseEmitator.GetNewUserID(); // Получаем ид
                ClientConnection client; // Раз мы решили авторизировать клиента, то следует создать клиента с которым мы будем в дальнейшем работать
                client = approvalConnection.Approve(response); // возвращяем данные вместе с разрешением, так же мы получим уже рабочий экземпляр авторизированного соеденения              
            } 
            else
            {
                //Ну а раз test в логине не нашлось, то соеденение не разрешаем
                //Тут нам нужно отправить два оргумента
                // первый это код ошибки, вы можете указать его любым, от вернется клиенту для дайнейшей обработки
                // второй это читабельное сообщение об ошибки, впрочим это не обязательно
                approvalConnection.Deny(1,"Нам не нравится ваш логин");
            }
        }
    }
}
