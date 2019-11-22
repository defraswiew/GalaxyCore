using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplateCommon
{
    /// <summary>
    /// Коды команд
    /// </summary>
 public enum CommandType:byte
    {
        /// <summary>
        /// Создать комнату
        /// </summary>
        roomCreate = 20,
        /// <summary>
        /// Войти в комнату
        /// </summary>
        roomEnter = 21,
        /// <summary>
        /// Выйти из комнаты
        /// </summary>
        roomExit = 22,
        /// <summary>
        /// Получить список комнат
        /// </summary>
        roomGetList = 23
    }
    /// <summary>
    /// Типы реализаций инстансов
    /// </summary>
   public enum InstanceType:byte
    {
        /// <summary>
        /// Комната
        /// </summary>
        room = 1,
    }

}
