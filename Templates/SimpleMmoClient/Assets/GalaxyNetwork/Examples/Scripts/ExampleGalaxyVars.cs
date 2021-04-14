using GalaxyCoreCommon;
using GalaxyCoreLib.NetEntity;
using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEngine;

namespace GalaxyNetwork.Examples.Scripts
{
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
        [GalaxyVar(2,false)]
        public int hp;
        public TextMesh textMesh;
        /// <summary>
        /// Пример синхронизации внутри любых других классах
        /// </summary>
        public Test test = new Test();
        void Start()
        {
            ClientNetEntity netEntity = GetComponent<UnityNetEntity>().NetEntity;
            // регистрируем внешний класс в galaxyVars
            netEntity.GalaxyVars.RegistrationClass(test);
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
}