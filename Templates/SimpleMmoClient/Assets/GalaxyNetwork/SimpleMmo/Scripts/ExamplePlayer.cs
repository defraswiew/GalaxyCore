using GalaxyCoreLib.NetEntity;
using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEngine;

public class ExamplePlayer : MonoBehaviour
{
    ClientNetEntity netEntity;
    Material material;
    void Awake()
    {
        netEntity = GetComponent<UnityNetEntity>().NetEntity;
        material = GetComponentInChildren<MeshRenderer>().material;         
    }

    void OnEnable()
    {
        netEntity.OnInMessage += OnInMessage;
    }
    void OnDisable()
    {
        netEntity.OnInMessage -= OnInMessage;
    }

    private void OnInMessage(byte code, byte[] data)
    {
        switch (code)
        {
            case 200:
                material.color = Color.red;
                break;
            case 201:
                material.color = Color.green;
                break;
            default:
                break;
        }
    }
}
