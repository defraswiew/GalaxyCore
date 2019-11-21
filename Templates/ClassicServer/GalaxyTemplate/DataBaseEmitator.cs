using System;
using System.Collections.Generic;
using System.Text;

namespace GalaxyTemplate
{
    /// <summary>
    /// Класс имущирующий работу базы данных
    /// </summary>
   public static class DataBaseEmitator
    {
        private static int lastId = 0;

        /// <summary>
        /// Возвращяем ид пользователя
        /// </summary>
        /// <returns></returns>
        public static int GetNewUserID()
        {
            lastId++;
            return lastId;
        }

    }
}
