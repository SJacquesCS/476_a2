using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public Material rayMat;

    private bool isDoingDijkstra;
    private List<GameObject> lines;

    private void Start()
    {
        //lines = new List<GameObject>();
        //isDoingDijkstra = false;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space) && !isDoingDijkstra)
        //{
        //    isDoingDijkstra = true;
        //    RandomDijkstra();
        //    isDoingDijkstra = false;
        //}
    }

    private void RandomDijkstra()
    {
        GameObject[] nodes;

        nodes = GameObject.FindGameObjectsWithTag("node");

        foreach (GameObject node in nodes)
            node.GetComponent<MeshRenderer>().material.color = Color.white;

        int rand_1 = Random.Range(0, nodes.Length);
        int rand_2;

        do
        {
            rand_2 = Random.Range(0, nodes.Length);
        }
        while (rand_2 == rand_1);

        GameObject startNode = nodes[rand_1], endNode = nodes[rand_2];
        startNode.GetComponent<MeshRenderer>().material.color = Color.green;
        endNode.GetComponent<MeshRenderer>().material.color = Color.red;

        Dijkstra(startNode, endNode);
    }

    private void Dijkstra(GameObject start, GameObject end)
    {
        Dictionary<GameObject, float> openList = new Dictionary<GameObject, float>
        {
            { start, 0 }
        };

        Dictionary<GameObject, float> closedList = new Dictionary<GameObject, float>();
        Dictionary<float, GameObject> currentNeighbors;

        while(openList.Count > 0)
        {
            float smallestDistance = float.MaxValue;
            float costSoFar;
            GameObject currentNode = null;
            GameObject bestNode = null;

            foreach (KeyValuePair<GameObject, float> entry in openList)
            {
                if (entry.Value < smallestDistance)
                {
                    smallestDistance = entry.Value;
                    currentNode = entry.Key;
                }
            }

            openList.Remove(currentNode);

            currentNeighbors = currentNode.GetComponent<Nodes>().GetNeighbors();
            costSoFar = smallestDistance;
            smallestDistance = float.MaxValue;

            foreach (KeyValuePair<float, GameObject> entry in currentNeighbors)
            {
                if (entry.Key < smallestDistance)
                {
                    smallestDistance = entry.Key;
                    bestNode = entry.Value;
                }

                if (!openList.ContainsKey(entry.Value) && !closedList.ContainsKey(entry.Value))
                {
                    entry.Value.GetComponent<MeshRenderer>().material.color = Color.blue;
                    entry.Value.GetComponent<Nodes>().SetParent(currentNode);
                    openList.Add(entry.Value, costSoFar + entry.Key);
                }
                else if (openList.ContainsKey(entry.Value))
                {
                    float cost;
                    openList.TryGetValue(entry.Value, out cost);

                    if (cost > costSoFar + entry.Key)
                    {
                        entry.Value.GetComponent<Nodes>().SetParent(currentNode);
                        openList.Remove(entry.Value);
                        openList.Add(entry.Value, costSoFar + entry.Key);
                    }
                }
            }

            costSoFar += smallestDistance;

            if (!closedList.ContainsKey(currentNode))
            {
                currentNode.GetComponent<MeshRenderer>().material.color = Color.black;
                closedList.Add(currentNode, costSoFar);
            }
            else
            {
                closedList.Remove(currentNode);
                closedList.Add(currentNode, costSoFar);
            }

            start.GetComponent<MeshRenderer>().material.color = Color.green;
            end.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        GameObject parent = end;

        foreach (GameObject line in lines)
            Destroy(line);

        lines.Clear();

        while (parent != null && parent.name != start.name)
        {
            parent.GetComponent<MeshRenderer>().material.color = Color.yellow;
            GameObject newParent = parent.GetComponent<Nodes>().Getparent();

            GameObject line = new GameObject();
            line.AddComponent<LineRenderer>();
            LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.material = rayMat;
            lineRenderer.material.color = Color.yellow;
            lineRenderer.startWidth = 1;
            lineRenderer.endWidth = 1;
            lineRenderer.SetPosition(0, parent.transform.position);
            lineRenderer.SetPosition(1, newParent.transform.position);
            lines.Add(line);

            parent = newParent;
        }

        start.GetComponent<MeshRenderer>().material.color = Color.green;
        end.GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
