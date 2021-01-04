using GalaxyCoreCommon.Navigation;
using UnityEngine;

public class TMP_TestAgent : MonoBehaviour,IGalaxyPathResult
{
    [SerializeField]
    private Transform endPoint;

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
    private bool cashe = true;
    private NavigationMask mask;
    private void Start()
    {
        controller = FindObjectOfType<GalaxyMapBaker>();
        mask = new NavigationMask();
        mask.AddLayer(layers);
    }

    void Update()
    {
        mask.maxLiftAngle = angle;
        mask.maxIteration = maxIteration;
        mask.lianer = line;
        mask.ignoreCash = cashe;
        controller.map.GetPath(transform.position,endPoint.position,this,mask);
    
        var lastPosition = transform.position;
       if (path.Nodes == null) return;
        foreach (var node in path.Nodes)
        {
          var nextPosition = node.GetPosition();
          Debug.DrawLine(lastPosition,nextPosition);
          lastPosition = nextPosition;
        }
    }

    public void OnGalaxyPathResult(GalaxyPath path)
    {
        Debug.Log(path.Status.ToString());
        this.path = path;
    }
}
