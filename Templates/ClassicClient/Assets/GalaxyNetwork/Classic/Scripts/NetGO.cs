using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetGO : MonoBehaviour
{
    [HideInInspector]
    public int localId;
    public int netID = 0;
    public bool isMy = false;
    public bool sync = false;
    private void Awake()
    {
       if(netID==0) StaticLinks.netGoManager.NetInstantiate(this);
    
    }
   
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}
