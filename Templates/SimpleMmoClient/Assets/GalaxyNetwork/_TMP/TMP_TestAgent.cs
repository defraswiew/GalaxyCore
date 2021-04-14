using GalaxyCoreCommon;
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
    private bool _preLine = true;
    [SerializeField]
    private bool cashe = true;
    private NavigationMask mask;
    private void Start()
    {
        controller = FindObjectOfType<GalaxyMapBaker>();
        mask = new NavigationMask();
        mask.AddLayer(layers);
        InvokeRepeating("Work",0.5f,1);
    }

    private void Update()
    {
        if (path.Nodes == null) return;
        var lastPosition = transform.position;
        foreach (var node in path.Nodes)
        {
          var nextPosition = node.GetPosition();
          Debug.DrawLine(lastPosition,nextPosition);
          lastPosition = nextPosition;
        }
    }

    private void Work()
    {
        mask.MaxLiftAngle = angle;
        mask.MaxIteration = maxIteration;
        mask.Lianer = line;
        mask.IgnoreCash = cashe;
        mask.IgnoreCost = _ignoreCost;
        mask.PreLiner = _preLine;
        controller.Map.GetPath(transform.position,endPoint.position,this,mask);
    }
    public void OnGalaxyPathResult(GalaxyPath path)
    {
        Debug.Log(path.Status.ToString());
        this.path = path;
    }
}
