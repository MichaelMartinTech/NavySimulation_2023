using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private LineMgr lineMgr;

    private Vector3 start = Vector3.zero;
    private Vector3 end = Vector3.zero;
    private List<LineRenderer> lines = new List<LineRenderer>();

    private bool isGenerating = false;

    // Update is called once per frame
    void Update()
    {
        if(isGenerating) return;

        if(Input.GetMouseButtonDown(0))
        {
            start = mainCam.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            start.y = 0;
        }
        if (Input.GetMouseButtonDown(1))
        {
            end = mainCam.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
            end.y = 0;
            UpdateLine();
        }
    }

    private async void UpdateLine()
    {
        isGenerating = true;
        foreach (LineRenderer r in lines)
        {
            Destroy(r.gameObject);
        }

        lines.Clear();

        List<Vector3> points = await AIMgr.inst.PerformAStar(start, end);
        if (points != null)
        {
            Vector3 current = start;
            foreach (Vector3 point in points)
            {
                LineRenderer render = lineMgr.CreateMoveLine(current, point);
                render.startWidth = 40;
                render.endWidth = 40;
                lines.Add(render);
                current = point;
            }
        }
        isGenerating = false;
    }
}
