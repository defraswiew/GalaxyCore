using GalaxyCoreLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalaxyUIStatistic : MonoBehaviour
{
    public Text inTraffic;
    public Text inSpeed;
    public Text outTraffic;
    public Text outSpeed;
    public Text entity;
    public Text ping;
    float inTrafficValue;
    float inSpeedValue;
    float time = 0.5f;
     void Start()
    {
        InvokeRepeating("Tick", 0.5f, time);
    }

    void Tick()
    {
        if (!GalaxyApi.connection.isConnected) return;
      
        inTraffic.text = System.Math.Round((GalaxyApi.connection.statistic.inTraffic / 1024f) / 1024f, 2) + " MB";  
        inSpeed.text = System.Math.Round(GalaxyApi.connection.statistic.inTrafficInSecond / 1024f, 2) + " KB";

        outTraffic.text = System.Math.Round((GalaxyApi.connection.statistic.outTraffic / 1024f) / 1024f, 2) + " MB";
        outSpeed.text = System.Math.Round(GalaxyApi.connection.statistic.outTrafficInSecond / 1024f, 2) + " KB";
        entity.text = GalaxyApi.netEntity.Count.ToString();
        ping.text = System.Math.Round(GalaxyApi.connection.statistic.ping * 1000, 2) + " ms";
    }
}
