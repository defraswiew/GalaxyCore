namespace SimpleMmoCommon.RPGTemplate
{
    /// <summary>
    /// Состояние моба
    /// </summary>
  public enum MobState
    {
        /// <summary>
        /// нет состояния
        /// </summary>
        none = 0,
        /// <summary>
        /// Ожидание
        /// </summary>
        idle = 1,
        /// <summary>
        /// Куда то ползет
        /// </summary>
        move = 2,
        /// <summary>
        /// ползет задом
        /// </summary>
        moveBack = 3,
        /// <summary>
        /// Преследует
        /// </summary>
        follow = 4,
        /// <summary>
        /// Атакует
        /// </summary>
        attack = 10,
        /// <summary>
        /// Помер
        /// </summary>
        death = 20,
    }
}
