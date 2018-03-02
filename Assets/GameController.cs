using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private GameObject[] nodes;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(RandomDijkstra());
    }

    IEnumerator RandomDijkstra()
    {
        nodes = GameObject.FindGameObjectsWithTag("node");
        Debug.Log("START");

        yield return new WaitForSeconds(0.5f);

        foreach (GameObject node in nodes)
            node.GetComponent<MeshRenderer>().material.color = Color.white;

        int rand_1 = Random.Range(0, nodes.Length);
        int rand_2;

        do
        {
            rand_2 = Random.Range(0, nodes.Length);
            Debug.Log(rand_1 + ", " + rand_2);
            yield return new WaitForSeconds(0.5f);
        }
        while (rand_2 == rand_1);

        GameObject startNode = nodes[rand_1], endNode = nodes[rand_2];
        startNode.GetComponent<MeshRenderer>().material.color = Color.green;
        endNode.GetComponent<MeshRenderer>().material.color = Color.red;

        yield return new WaitForSeconds(0.5f);

        Debug.Log("BEFORE DIJK");

        Dijkstra(startNode, endNode);

        yield return new WaitForSeconds(0.5f);

        Debug.Log("END");
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

            if (currentNode.name == end.name)
                break;

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

        GameObject parent = end.GetComponent<Nodes>().Getparent();

        while (parent != null)
        {
            parent.GetComponent<MeshRenderer>().material.color = Color.yellow;
            parent = parent.GetComponent<Nodes>().Getparent();
        }

        start.GetComponent<MeshRenderer>().material.color = Color.green;
        end.GetComponent<MeshRenderer>().material.color = Color.red;
    }
}
