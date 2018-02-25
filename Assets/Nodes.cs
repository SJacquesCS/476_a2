using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Nodes : MonoBehaviour
{
    public float rayThickness;
    public Material rayMat;

    private Dictionary<float, GameObject> neighbors;
    private List<GameObject> lines;
    private bool areLinksShown;
    private GameObject parentNode;

    void Start()
    {
        neighbors = new Dictionary<float, GameObject>();
        lines = new List<GameObject>();

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
                catch (ArgumentException)
                {
                    continue;
                }
            }
        }

        parentNode = null;
        areLinksShown = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleLinks();
        }
    }

    private void ToggleLinks()
    {
        if (areLinksShown)
        {
            foreach (GameObject line in lines)
                Destroy(line);

            lines.Clear();
        }
        else
        {
            foreach (KeyValuePair<float, GameObject> neighbor in neighbors)
            {
                GameObject line = new GameObject();
                line.AddComponent<LineRenderer>();
                LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.material = rayMat;
                lineRenderer.startWidth = 1;
                lineRenderer.endWidth = 1;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, neighbor.Value.transform.position);
                lines.Add(line);
            }
        }

        areLinksShown = !areLinksShown;
    }

    public Dictionary<float, GameObject> GetNeighbors()
    {
        return neighbors;
    }

    public GameObject Getparent()
    {
        return parentNode;
    }

    public void SetParent(GameObject parent)
    {
        parentNode = parent;
    }
}
