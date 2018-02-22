using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Nodes : MonoBehaviour
{

    public float rayThickness;
    public Material rayMat;

    private Dictionary<float, GameObject> neighbors;

    void Start()
    {
        neighbors = new Dictionary<float, GameObject>();

        foreach (GameObject node in GameObject.FindGameObjectsWithTag("node"))
        {
            RaycastHit hit;
            Vector3 direction = node.transform.position - transform.position;

            if (Physics.SphereCast(transform.position, rayThickness, direction, out hit) && hit.collider.gameObject.tag == "node")
            {
                try
                {
                    neighbors.Add(hit.distance, hit.collider.gameObject);
                }
                catch (ArgumentException e)
                {
                    continue;
                }
            }
        }

        foreach (KeyValuePair<float, GameObject> neighbor in neighbors)
        {
            GameObject line = new GameObject();
            line.AddComponent<LineRenderer>();
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.material = rayMat;
            lineRenderer.startWidth = rayThickness;
            lineRenderer.endWidth = rayThickness;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, neighbor.Value.transform.position);
        }
    }
}
