using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GalaxyCoreCommon;
using System;

public class TestGalaxyVars : MonoBehaviour
{
    [GalaxyVar(1)]
    public int _Int32 = 1;
    [GalaxyVar(2)]
    public string _String = "";
    [GalaxyVar(3)]
    public UInt32 _UInt32 = 3;
    [GalaxyVar(4)]
    public byte _Byte = 4;
    [GalaxyVar(5)]
    public double _Double = 5;
    [GalaxyVar(6)]
    public bool _Boolean = false;
    [GalaxyVar(7)]
    public float _Single = 7;
    [GalaxyVar(8)]
    public long _Int64 = 8;
    [GalaxyVar(9)]
    public ulong _UInt64 = 9;
    [GalaxyVar(10)]
    public short _Int16 = 10;
    [GalaxyVar(11)]
    public ushort _UInt16 = 11;
    [GalaxyVar(22)]
    public Vector3 test;
    void Start()
    {
        Invoke("Change", 5);
    }

    void Change()
    {
       // testValue = UnityEngine.Random.Range(0, 99999);
    }

}
