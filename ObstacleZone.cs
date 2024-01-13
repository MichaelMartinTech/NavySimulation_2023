using UnityEngine;

public class ObstacleZone : MonoBehaviour
{
    public int numberOfObstacles = 20;
    public Obstacle.Type type;

    public Obstacle circular;
    public Obstacle rectangular;

    public Collider Collider => GetComponent<Collider>();

    public GameObject AStarCollider;
    public GameObject OfficeCollider;

    // Start is called before the first frame update
    private void Start()
    {
        Build();
        AStarCollider.SetActive(false);
        OfficeCollider.SetActive(false);
    }

    public void SetType(int type)
    {
        if (type < 2)
        {
            this.type = (Obstacle.Type)type;
            Rebuild();
            return;
        }

        Clear();
        AStarCollider.SetActive(type == 2);
        OfficeCollider.SetActive(type == 3);

        if(type == 2)
        {
            Obstacle[] aStarBoxes = AStarCollider.GetComponentsInChildren<Obstacle>();
            foreach(Obstacle b in aStarBoxes)
            {
                EntityMgr.inst.AddObstacle(b);
            }
            return;
        }

        Obstacle[] officeBoxes = OfficeCollider.GetComponentsInChildren<Obstacle>();
        foreach (Obstacle b in officeBoxes)
        {
            EntityMgr.inst.AddObstacle(b);
        }
    }
    public void SetCount(int count)
    {
        numberOfObstacles = 20;
        if (count > 0) numberOfObstacles = 30;
        if (count > 1) numberOfObstacles = 100;

        if (!AStarCollider.activeSelf && !OfficeCollider.activeSelf)
        {
            Rebuild();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Rebuild();
        }
    }

    void Clear()
    {
        if (EntityMgr.inst != null) EntityMgr.inst.ClearObstacles();
        else
        {
            foreach (Obstacle o in FindObjectsByType<Obstacle>(FindObjectsSortMode.None))
            {
                Destroy(o.gameObject);
            }
        }
    }

    void Rebuild()
    {
        Clear();
        Build();
    }

    void Build()
    {
        for(int i = 0; i < numberOfObstacles; i++)
        {
            float xPos = Random.Range(Collider.bounds.min.x, Collider.bounds.max.x);
            float zPos = Random.Range(Collider.bounds.min.z, Collider.bounds.max.z);

            switch(type) {
                case Obstacle.Type.Rectangular:
                    Obstacle r = Instantiate(rectangular, new Vector3(xPos, 0, zPos), Quaternion.identity);
                    r.transform.SetParent(transform);
                    if(EntityMgr.inst != null) EntityMgr.inst.AddObstacle(r);
                    break;
                case Obstacle.Type.Circular:
                    Obstacle c = Instantiate(circular, new Vector3(xPos, 0, zPos), Quaternion.identity);
                    c.transform.SetParent(transform);
                    if (EntityMgr.inst != null) EntityMgr.inst.AddObstacle(c);
                    break;
            }
        }
    }
}
