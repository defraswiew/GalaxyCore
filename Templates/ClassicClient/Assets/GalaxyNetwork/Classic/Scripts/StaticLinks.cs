using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Этот класс мы будем использовать как хранитель ссылок на разные модули
/// </summary>
public static class StaticLinks  
{
    /// <summary>
    /// События вызываемые входящими сообщениями
    /// </summary>
    public static MessageEvents messageEvents = new MessageEvents();

    /// <summary>
    /// 
    /// </summary>
    public static ClientData clientData = new ClientData();
    /// <summary>
    /// Менеджер сетевых объектов
    /// </summary>
    public static NetGOManager netGoManager = new NetGOManager();
  
}
