using System.Collections.Generic;
using GalaxyCoreCommon;
using GalaxyCoreLib.NetEntity;
using UnityEngine;

public class ExampleNavBotDebag : MonoBehaviour
{
    private ClientNetEntity entity;
    private List<Vector3> points;
    void Start()
    {
        points = new List<Vector3>();
        entity = GetComponent<UnityNetEntity>().netEntity;
        entity.OnInMessage+= EntityOnOnInMessage;
    }

    private void EntityOnOnInMessage(byte code, byte[] data)
    {
        if (code == 1)
        {
            var message = new BitGalaxy(data);
            points.Clear();
            while (message.position<message.data.Length)
            {
                points.Add(message.ReadGalaxyVector3().Vector3());
            }
            transform.LookAt(transform.position);
        }
    }

   
    void Update()
    {
        var lastPosition = transform.position;
        int i = 0;
        foreach (var point in points)
        {
            if(i>0)Debug.DrawLine(lastPosition,point);
            lastPosition = point;
            i++;
        }
    }
}
