using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour {

    public Material rayMat;
    public float maxVelocity;

    private GameObject[] nodes;
    private GameObject closestNode;
    private GameObject destination;
    private GameObject target;
    private List<GameObject> lines;

    private bool isSetup;

	void Start () {
        isSetup = false;

        nodes = GameObject.FindGameObjectsWithTag("node");
        lines = new List<GameObject>();

        int rand_1 = Random.Range(0, nodes.Length);
        int rand_2;

        do
        {
            rand_2 = Random.Range(0, nodes.Length);
        }
        while (rand_2 == rand_1);

        transform.position = nodes[rand_1].transform.position;
        destination = nodes[rand_2];
	}

    IEnumerator Setup()
    {
        yield return new WaitForSeconds(0.5f);

        FindClosestNode();

        isSetup = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (isSetup)
        {
            float distance = (target.transform.position - transform.position).magnitude;

            if (distance <= 0.5f)
            {
                target = target.GetComponent<Nodes>().GetChild();
            }

            if (closestNode == null || closestNode.name == destination.name)
            {
                //destination = nodes[Random.Range(0, nodes.Length)];
            }
            else
            {
                Move();
            }
        }
	}

    private void Move()
    {
        Vector3 direction = target.transform.position - transform.position;
        Vector3 mVelocity = maxVelocity * direction.normalized;

        transform.position = transform.position + (mVelocity * Time.deltaTime);
    }

    private void FindClosestNode()
    {
        float smallestDistance = float.MaxValue;
        GameObject winner = null;

        foreach (GameObject node in nodes)
        {
            float distance = (node.transform.position - transform.position).magnitude;

            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                winner = node;
            }
        }

        closestNode = winner;

        Dijkstra(closestNode, destination);
    }

    private void Dijkstra(GameObject start, GameObject end)
    {
        Dictionary<GameObject, float> openList = new Dictionary<GameObject, float>
        {
            { start, 0 }
        };

        Dictionary<GameObject, float> closedList = new Dictionary<GameObject, float>();
        Dictionary<float, GameObject> currentNeighbors;

        foreach (GameObject node in nodes)
            node.GetComponent<Nodes>().SetChild(null);

        while (openList.Count > 0)
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
                    currentNode.GetComponent<Nodes>().SetChild(entry.Value);
                    openList.Add(entry.Value, costSoFar + entry.Key);
                }
                else if (openList.ContainsKey(entry.Value))
                {
                    float cost;
                    openList.TryGetValue(entry.Value, out cost);

                    if (cost > costSoFar + entry.Key)
                    {
                        entry.Value.GetComponent<Nodes>().SetParent(currentNode);
                        currentNode.GetComponent<Nodes>().SetChild(entry.Value);
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

        target = start.GetComponent<Nodes>().GetChild();
    }
}
