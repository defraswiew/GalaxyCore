using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSpawner : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        GameObject go = Instantiate(player);
        go.transform.position = new Vector3(transform.position.x + Random.Range(-10, 10), transform.position.y, transform.position.z + Random.Range(-10, 10));
    }

    
}
