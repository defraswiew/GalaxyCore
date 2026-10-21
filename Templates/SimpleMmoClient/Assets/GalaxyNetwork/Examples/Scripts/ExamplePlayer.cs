using GalaxyCoreLib.NetEntity;
using GalaxyNetwork.Core.Scripts.NetEntity;
using UnityEngine;

/// <summary>Collision-message example retained by the Player prefab and octree scene.</summary>
[RequireComponent(typeof(UnityNetEntity))]
public class ExamplePlayer : MonoBehaviour
{
    private ClientNetEntity netEntity;
    private Material material;
    private void Awake()
    {
        netEntity = GetComponent<UnityNetEntity>().NetEntity;
        var renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null) material = renderer.material;
    }
    private void OnEnable() { if (netEntity != null) netEntity.OnInMessage += OnInMessage; }
    private void OnDisable() { if (netEntity != null) netEntity.OnInMessage -= OnInMessage; }
    private void OnInMessage(byte code, byte[] data)
    {
        if (material == null) return;
        if (code == 200) material.color = Color.red;
        else if (code == 201) material.color = Color.green;
    }
    private void OnDestroy() { if (material != null) Destroy(material); }
}
