using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPM_Spawn : MonoBehaviour
{
    public GameObject test;
    void Start()
    {
        InvokeRepeating("Spawn", 10, 1);
    }

    void Spawn()
    {
        GameObject go = Instantiate(test,transform);
        go.transform.position = new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
    }
    
}
