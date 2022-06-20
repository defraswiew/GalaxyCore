using GalaxyCoreCommon;
using UnityEngine;

public class ExampleNetVars2 : MonoBehaviour
{
   [GalaxyVar(1)]
   public string Name;
   [GalaxyVar(2)]
   public int Hp;
   [GalaxyVar(3)]
   public float ServerTime;
}
