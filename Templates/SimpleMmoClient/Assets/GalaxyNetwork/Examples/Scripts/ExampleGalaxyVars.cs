using GalaxyCoreCommon;
using GalaxyCoreLib.NetEntity;
using UnityEngine;

/// <summary>
/// Пример работы с GalaxyVar
/// </summary>
public class ExampleGalaxyVars : MonoBehaviour
{
    /// <summary>
    /// Помечаем строку как синхронизируемую
    /// </summary>
    [GalaxyVar(1)]
    public string text;
    /// <summary>
    /// Делаем хп сетевыми
    /// </summary>
    [GalaxyVar(2)]
    public int hp;
    public TextMesh textMesh;
    /// <summary>
    /// Пример синхронизации внутри любых других классах
    /// </summary>
    public Test test = new Test();
    void Start()
    {
        ClientNetEntity netEntity = GetComponent<UnityNetEntity>().netEntity;
        // регистрируем внешний класс в galaxyVars
        netEntity.galaxyVars.RegistrationClass(test);
        netEntity.OnInMessage += OnInMessage;
    }

    private void OnInMessage(byte code, byte[] data)
    {
    //    if (code == 100) Debug.Log("Classic send");
    //    if (code == 101) Debug.Log("Octo send");
    }

    void Update()
    {
        textMesh.text = text + " heal:" + test.heal;
    }
}
public class Test
{
    [GalaxyVar(25)]
    public int heal = 100;
}

 
