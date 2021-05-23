using System;
using System.Collections.Generic;
using GalaxyCoreCommon.Navigation;
using GalaxyNetwork.Core.Alpha.Navigation;
using GalaxyNetwork.Core.Scripts;
using UnityEngine;

public class TMP_TestAgent : MonoBehaviour,IGalaxyPathResult
{
    [SerializeField]
    private Transform endPoint;
    [SerializeField]
    private bool _ignoreCost;
    private GalaxyPath path;
    private GalaxyMapBaker controller;
    [SerializeField]
    private byte[] layers;
    [SerializeField] 
    private float angle = 1;
    [SerializeField]
    private int maxIteration = 10000;
    [SerializeField]
    private bool line = true;
    [SerializeField]
    private int endPointAllowance;
    
    [SerializeField] 
    private PreLinerType _preLine;
    [SerializeField]
    private bool cashe = true;
    private NavigationMask mask;
    private float _lastTime;
    private void Start()
    {
        controller = FindObjectOfType<GalaxyMapBaker>();
        mask = new NavigationMask();
        mask.AddLayer(layers);
     //   Invoke("Work",0.5f);
    }

    private void OnEnable()
    {
        controller = FindObjectOfType<GalaxyMapBaker>();
        mask = new NavigationMask();
        mask.AddLayer(layers);
        Work();
    }

    private void Update()
    {
        if (path.Nodes == null) return;
        var lastPosition = transform.position;
        foreach (var node in new List<WorkNode>(path.Nodes))
        {
          var nextPosition = node.GetPosition();
          Debug.DrawLine(lastPosition,nextPosition);
          lastPosition = nextPosition;
        }
    }

    private void Work()
    {
        mask.MaxLiftAngle = angle;
        mask.EndPointAllowance = endPointAllowance;
        mask.MaxIteration = maxIteration;
        mask.Lianer = line;
        mask.IgnoreCash = cashe;
        mask.IgnoreCost = _ignoreCost;
        mask.PreLiner = _preLine;
        controller.Map.GetPath(transform.position,endPoint.position,this,mask);
        _lastTime = Time.time;
    }
    public void OnGalaxyPathResult(GalaxyPath path)
    {
        Debug.Log(path.Status.ToString() + " " + (Time.time - _lastTime) + "  wait:"+path.Diagnostics.WaitTime + "  PreLine:"+path.Diagnostics.PreLineTime + "  Find:"+path.Diagnostics.PathFindingTime + "  Liner:"+path.Diagnostics.LianerTime + " -->" +controller.Map.RequestCont + " / " + controller.Map.ResponseCont);
        _lastTime = Time.time;
        this.path = path;
        Work();
    }
}

