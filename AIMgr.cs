using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

//ERRORS with these two lines
//using UnityEditor.Experimental.GraphView;
//using UnityEditorInternal;

using UnityEngine;
using UnityEngine.UIElements;

public class AIMgr : MonoBehaviour
{
    public static AIMgr inst;
    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        layerMask = 1 << 9;// LayerMask.GetMask("Water");
    }

    public bool isPotentialFieldsMovement = false;
    public bool useAStar = false;
    public float potentialDistanceThreshold = 1000;
    public float attractionCoefficient = 500;
    public float attractiveExponent = -1;
    public float repulsiveCoefficient = 60000;
    public float repulsiveExponent = -2.0f;
    public int gridSize = 10;

    public RaycastHit hit;
    public int layerMask;
    public List<Vector3> LatestPath = new List<Vector3>();
    public bool handleInput = true;
    public LayerMask mask;

    public List<GameObject> PointMarkers= new List<GameObject>();
    public GameObject MarkerPrefab;

    private bool updating = false;
    // Update is called once per frame
    void Update()
    {
        if(!handleInput) return;

        if (Input.GetMouseButtonDown(1)) {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, layerMask)) {
                //Debug.DrawLine(Camera.main.transform.position, hit.point, Color.yellow, 2); //for debugging
                Vector3 pos = hit.point;
                pos.y = 0;
                Entity381 ent = FindClosestEntInRadius(pos, rClickRadiusSq);
                if (ent == null) {
                    HandleMove(SelectionMgr.inst.selectedEntities, pos);
                } else {
                    if (Input.GetKey(KeyCode.LeftControl))
                        HandleIntercept(SelectionMgr.inst.selectedEntities, ent);
                    else
                        HandleFollow(SelectionMgr.inst.selectedEntities, ent);
                }
            } else {
                //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * 1000, Color.white, 2);
            }
        }
    }

    private class AStarPoint
    {
        public Vector3 position;
        public AStarPoint from;
        public float hValue;
        public float costValue;
        public Vector3 direction;
    }

    AStarPoint getNextPoint(ref List<AStarPoint> points)
    {
        int index = 0;
        float hMax = points[0].hValue;
        for(int i = 0; i < points.Count; i++)
        {
            if (points[i].hValue < hMax)
            {
                index = i;
                hMax = points[i].hValue;
            }
        }

        AStarPoint ret = points[index];
        points.RemoveAt(index);
        return ret;
    }

    void UpdatePoint(ref List<AStarPoint> unseen, ref List<AStarPoint> seen, Vector3 point, AStarPoint from, float cost, float hValue, Vector3 direction)
    {
        for (int i = 0; i < seen.Count; ++i)
        {
            if (isSame(seen[i].position, point))
            {
                if (cost < seen[i].costValue)
                {
                    seen[i].hValue = hValue;
                    seen[i].costValue = cost;
                    seen[i].from = from;
                    seen[i].direction = direction;
                }
                return;
            }
        }

        for (int i = 0; i < unseen.Count; ++i)
        {
            if (isSame(unseen[i].position, point))
            {
                if (cost < unseen[i].costValue)
                {
                    unseen[i].hValue = hValue;
                    unseen[i].costValue = cost;
                    unseen[i].from = from;
                    unseen[i].direction = direction;
                }
                return;
            }
        }

        unseen.Add(new AStarPoint { position = point, from = from, costValue = cost, hValue = hValue, direction = direction });
    }

    bool isSame(Vector3 p, Vector3 q)
    {
        Vector3 snap = snapTo(p) - snapTo(q);
        return snap.x == 0 && snap.z == 0;
    }
    bool isBlocked(Vector3 p)
    {
        foreach (Obstacle o in EntityMgr.inst.obstacles)
        {
            if (o.Bounds.Contains(p)) return true;
        }
        return false;
    }
    public async Task<List<Vector3>> PerformAStar(Vector3 start, Vector3 point)
    {
        HashSet<Vector3> blockedPoints = new HashSet<Vector3>();
        foreach (Obstacle obstacle in EntityMgr.inst.obstacles)
        {
            foreach (Vector3 bp in obstacle.getBlockedPoints())
            {
                blockedPoints.Add(bp);
            }
        }

        //foreach (GameObject g in PointMarkers)
        //{
        //    Destroy(g);
        //}
        //PointMarkers.Clear();

        //foreach (Vector3 p in blockedPoints)
        //{
        //    PointMarkers.Add(Instantiate(MarkerPrefab, p, Quaternion.identity));
        //}

        return await Task.Run(() =>
        {
            Vector3 startPoint = snapTo(start);
            List<AStarPoint> seen = new List<AStarPoint>();
            List<AStarPoint> unseen = new List<AStarPoint>();
            List<Vector3> path = new List<Vector3>();
            unseen.Add(new AStarPoint { position = startPoint, from = null, costValue = 0, hValue = 0 });

            LatestPath.Clear();
            while (unseen.Count > 0)
            {
                Thread.Sleep(1);

                AStarPoint node = getNextPoint(ref unseen);

                if (isSame(node.position, point))
                {
                    // Found the end: Generate Path
                    // Going through to "node.from == null" so that the position the boat is in isn't included in the path
                    while (node != null)
                    {
                        path.Add(node.position);
                        node = node.from;
                    }

                    path.Reverse();
                    // Remove unneccessary points
                    Vector3 currentPoint = startPoint;
                    Vector3 direction = (path[1] - currentPoint).normalized;
                    for (int i = 1; i < path.Count; ++i)
                    {
                        Vector3 testDir = (path[i] - path[i-1]).normalized;
                        if (Vector3.Angle(direction, testDir) < 5) continue;

                        LatestPath.Add(path[i - 1]);
                        currentPoint = path[i - 1];
                        direction = testDir;

                        //bool blocked = false;
                        //foreach (Obstacle ob in EntityMgr.inst.obstacles)
                        //{
                        //    Vector3 pathTo = path[i] - currentPoint;

                        //    foreach (Vector3 block in blockedPoints)
                        //    {
                        //        Vector3 point = Vector3.Project(block - currentPoint, pathTo);
                        //        if (Vector3.Distance(point, block) < gridSize)
                        //        {
                        //            blocked = true;
                        //            break;
                        //        }
                        //    }

                        //    if (blocked)
                        //    {
                        //        break;
                        //    }
                        //}
                    }
                    LatestPath.Add(point);
                    return LatestPath;
                }

                seen.Add(node);

                Tuple<Vector3, float>[] arr = {
                    new Tuple<Vector3, float>(node.position + new Vector3( gridSize, 0, 0), gridSize),
                    new Tuple<Vector3, float>(node.position + new Vector3(-gridSize, 0, 0), gridSize),
                    new Tuple<Vector3, float>(node.position + new Vector3(0, 0, gridSize), gridSize),
                    new Tuple<Vector3, float>(node.position + new Vector3(0, 0, -gridSize), gridSize),
                    new Tuple<Vector3, float>(node.position + new Vector3( gridSize, 0,  gridSize), gridSize * 1.95f),
                    new Tuple<Vector3, float>(node.position + new Vector3(-gridSize, 0,  gridSize), gridSize * 1.95f),
                    new Tuple<Vector3, float>(node.position + new Vector3( gridSize, 0, -gridSize), gridSize * 1.95f),
                    new Tuple<Vector3, float>(node.position + new Vector3(-gridSize, 0, -gridSize), gridSize * 1.95f)
                };

                foreach (Tuple<Vector3, float> v in arr)
                {
                    //if (isBlocked(v.Item1)) continue;
                    if (blockedPoints.Contains(v.Item1))
                    {
                        //Debug.LogWarningFormat("Point {0} blocked", v.Item1);
                        continue;
                    }

                    Vector3 direction = (v.Item1 - node.position).normalized;
                    float cost = node.costValue + v.Item2 + Vector3.Angle(direction, node.direction);
                    float hValue = Mathf.Pow(Vector3.Distance(v.Item1, point), 2) + cost;

                    UpdatePoint(ref unseen, ref seen, v.Item1, node, cost, hValue, direction);
                }
            }

            if (LatestPath.Count == 0)
            {
                Debug.LogError("Unable to find valid Path");
            }

            return null;
        });
    }

    public Vector3 snapTo(Vector3 point)
    {
        point /= gridSize;
        point.x = Mathf.Floor(point.x);
        point.y = Mathf.Floor(point.y);
        point.z = Mathf.Floor(point.z);
        return point * gridSize;
    }

    public async void HandleMove(List<Entity381> entities, Vector3 point)
    {
        if (updating) return;

        updating = true;
        // Snap point to closest GridSize
        Vector3 target = snapTo(point);
        for (int j = 0; j < entities.Count; ++j)
        {
            Entity381 entity = entities[j];
            UnitAI uai = entity.GetComponent<UnitAI>();
            if (useAStar)
            {
                // Perform A* on world to find way around obsticles
                // Loop through values and create moves
                List<Vector3> path = await PerformAStar(entity.transform.position, target);
                if (path == null) continue;

                for (int i = 0; i < path.Count; i++)
                {
                    AddOrSet(new Move(entity, path[i]), uai, i != 0);
                }
                continue;
            }
            AddOrSet(new Move(entity, point), uai, false);
        }

        updating = false;
    }

    void AddOrSet(Command c, UnitAI uai, bool forceAdd = false)
    {
        if (Input.GetKey(KeyCode.LeftShift) || forceAdd)
            uai.AddCommand(c);
        else
            uai.SetCommand(c);
    }

    public void HandleFollow(List<Entity381> entities, Entity381 ent)
    {
        foreach (Entity381 entity in SelectionMgr.inst.selectedEntities) {
            Follow f = new Follow(entity, ent, new Vector3(100, 0, 0));
            UnitAI uai = entity.GetComponent<UnitAI>();
            AddOrSet(f, uai);
        }
    }

    void HandleIntercept(List<Entity381> entities, Entity381 ent)
    {
        foreach (Entity381 entity in SelectionMgr.inst.selectedEntities) {
            Intercept intercept = new Intercept(entity, ent);
            UnitAI uai = entity.GetComponent<UnitAI>();
            AddOrSet(intercept, uai);
        }

    }

    public float rClickRadiusSq = 10000;
    public Entity381 FindClosestEntInRadius(Vector3 point, float rsq)
    {
        Entity381 minEnt = null;
        float min = float.MaxValue;
        foreach (Entity381 ent in EntityMgr.inst.entities) {
            float distanceSq = (ent.transform.position - point).sqrMagnitude;
            if (distanceSq < rsq) {
                if (distanceSq < min) {
                    minEnt = ent;
                    min = distanceSq;
                }
            }    
        }
        return minEnt;
    }
}
