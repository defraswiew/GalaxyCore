using GalaxyCoreCommon;
using GalaxyCoreCommon.Serialization;
using System.Collections.Generic;

namespace SimpleMmoCommon.RPGTemplate
{
    /// <summary>
    /// Пример отправки списка ид итемов
    /// </summary>
    [GalaxySerializable]
    public class DropList:GalaxyMessage
    {
        /// <summary>
        /// список ид итемов
        /// </summary>
        [GalaxyField(0)]
        public List<int> Items;
    }
}
