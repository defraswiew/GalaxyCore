using GalaxyCoreCommon;
using GalaxyCoreLib.NetEntity;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class ExampleGalaxyVars : MonoBehaviour
{
    [GalaxyVar(1)]
    public string text;
    public TextMesh textMesh;
    public Test test = new Test();
    void Start()
    {
        ClientNetEntity netEntity = GetComponent<UnityNetEntity>().netEntity;
        netEntity.galaxyVars.RegistrationClass(test);
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

 
