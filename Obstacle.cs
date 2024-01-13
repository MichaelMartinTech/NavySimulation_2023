using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum Type
    {
        Circular,
        Rectangular
    }

    public Type type;
    public Collider Collider { get; private set; }
    public Bounds Bounds { get; private set; }

    public bool RandomScale = true;

    private Vector3 snapCenter;
    private Vector3 scale;

    private void Start()
    {
        if (!RandomScale)
        {
            Collider = GetComponent<Collider>();
            Bounds = new Bounds(transform.position, transform.localScale * 1.3f);
            snapCenter = AIMgr.inst.snapTo(transform.position);
            scale = transform.localScale;
            return;
        };


        float xScale = Random.Range(25, 50) * 10;
        float zScale = xScale;

        if(type == Type.Rectangular)
        {
            zScale = Random.Range(25, 50) * 10;
        }
        transform.localScale = new Vector3(xScale, 20, zScale);

        Collider = GetComponent<Collider>();
        Bounds = new Bounds(transform.position, transform.localScale * 1.3f);
        snapCenter = AIMgr.inst.snapTo(transform.position);
        scale = transform.localScale;
    }
        
    public HashSet<Vector3> getBlockedPoints()
    {
        HashSet<Vector3> points = new HashSet<Vector3>();

        Vector3 center = new Vector3(snapCenter.x, 0, snapCenter.z);

        for(float x = Bounds.min.x; x <= Bounds.max.x; x+=AIMgr.inst.gridSize)
        {
            for (float y = Bounds.min.z; y <= Bounds.max.z; y += AIMgr.inst.gridSize)
            {
                Vector3 point = AIMgr.inst.snapTo(new Vector3(x, 0, y));

                if(type == Type.Circular && Vector3.Distance(point, center) > scale.x)
                {
                    continue;
                }

                points.Add(point);
            }
        }

        return points;
    }
}
