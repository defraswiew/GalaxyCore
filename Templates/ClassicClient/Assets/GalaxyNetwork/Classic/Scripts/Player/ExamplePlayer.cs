using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamplePlayer : MonoBehaviour
{
    public NetGO netGO;
    public Transform head;
    public Camera cam;

    private void Start()
    {       
        netGO = GetComponent<NetGO>();
    }

    void Update()
    {
        if (!netGO.isMy) return;
        cam.gameObject.SetActive(true);
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.position += (transform.forward * Input.GetAxis("Vertical"))*Time.deltaTime*5;
        }
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.position += (transform.right * Input.GetAxis("Horizontal")) * Time.deltaTime * 5;
        }
        head.transform.Rotate(-Input.GetAxis("Mouse Y")*3, 0, 0f);
        transform.Rotate(0, Input.GetAxis("Mouse X")*3, 0);
    }
}
