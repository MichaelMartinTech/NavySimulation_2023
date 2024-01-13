using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMgr : MonoBehaviour
{
    public static EntityMgr inst;
    private void Awake()
    {
        inst = this;
        entities = new List<Entity381>();
        foreach(Entity381 ent in movableEntitiesRoot.GetComponentsInChildren<Entity381>()) {
            entities.Add(ent);
        }

        obstacles = new List<Obstacle>(FindObjectsOfType<Obstacle>());
    }

    public GameObject movableEntitiesRoot;
    public List<GameObject> entityPrefabs;
    public GameObject entitiesRoot;
    public List<Entity381> entities;
    public List<Obstacle> obstacles;

    public static int entityId = 0;

    public Entity381 CreateEntity(EntityType et, Vector3 position, Vector3 eulerAngles)
    {
        Entity381 entity = null;
        GameObject entityPrefab = entityPrefabs.Find(x => (x.GetComponent<Entity381>().entityType == et));
        if (entityPrefab != null) {
            GameObject entityGo = Instantiate(entityPrefab, position, Quaternion.Euler(eulerAngles), entitiesRoot.transform);
            if (entityGo != null) {
                entity = entityGo.GetComponent<Entity381>();
                entityGo.name = et.ToString() + entityId++;
                entities.Add(entity);
            }
        }
        return entity;
    }

    public void AddObstacle(Obstacle obstacle)
    {
        obstacles.Add(obstacle);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void ClearObstacles()
    {
        foreach(Obstacle o in obstacles)
        {
            Destroy(o.gameObject);
        }

        obstacles.Clear();
    }

    internal Entity381 CreateEntity(EntityType entityType, Vector3 position, Quaternion quaternion)
    {
        throw new NotImplementedException();
    }
}
