using System.Collections;
using System.Collections.Generic;
using GalaxyCoreCommon;
using GalaxyCoreCommon.Octree;
using UnityEngine;

public class TMP_TREE : MonoBehaviour
{
    private PointOctree<EntityUnit> tree;
    void Start()
    {
        tree = new PointOctree<EntityUnit>(1000, new GalaxyVector3(0, 0, 0), 100);
        var entity = new EntityUnit();
        tree.Add(entity,new GalaxyVector3(0,10,20));
        tree.AddOrUpdate(entity,new GalaxyVector3(0,10,20));
        var result = tree.GetNearby(new GalaxyVector3(10, 10, 10), 20);
       tree.Remove(entity);

    }
    public struct EntityUnit
    {
        private string _uid;
    }
}
