﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public Entity381 entity;
    // Start is called before the first frame update
    void Start()
    {
        if(entity == null) entity = GetComponentInParent<Entity381>();
    }

    // Update is called once per frame
    void Update()
    {
        if(entity != null)
            entity.selectionCircle.SetActive(entity.isSelected);
    }

    private void OnMouseDown()
    {
        //if (Input.GetMouseButtonDown(0)) {
            SelectionMgr.inst.SelectEntity(entity);
        //}
    }


}
