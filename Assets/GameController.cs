using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    private GameObject[] nodes;

	void Start () {
        nodes = GameObject.FindGameObjectsWithTag("node");

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

        StartCoroutine(Dijkstra(startNode, endNode));
	}

    IEnumerator Dijkstra(GameObject start, GameObject end)
    {
        yield return new WaitForSeconds(1.5f);

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

            yield return new WaitForSeconds(0.05f);

            if (openList.ContainsKey(end))
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
                    entry.Value.GetComponent<Nodes>().SetParent(entry.Value);
                    openList.Remove(entry.Value);
                    openList.Add(entry.Value, costSoFar + entry.Key);
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
