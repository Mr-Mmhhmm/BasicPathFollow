
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathFinder : MonoBehaviour
{
    public Text text;

    private PathNode currentNode;
    public PathNode GetCurrentNode { get { return currentNode; } }
    private PathNode lastNode;

    private PathNode targetNode;
    public PathNode GetTargetNode { get { return targetNode; } }
    public const float targetVarience = 1;


    public void SetCurrentNode()
    {
        // Get a current node if I do not have one
        if (!currentNode)
        {
            GameObject[] allNodes = GameObject.FindGameObjectsWithTag("PathNode");
            float shortestConnection = float.PositiveInfinity;
            foreach (GameObject node in allNodes)
            {
                float distance = Vector3.Distance(transform.position, node.transform.position);
                if (node.GetComponent<PathNode>() && distance < shortestConnection)
                {
                    shortestConnection = distance;
                    currentNode = node.GetComponent<PathNode>();
                }
            }
            int randomNum = Random.Range(0, currentNode.usedConnections.Count);
            targetNode = currentNode.usedConnections[randomNum].target.GetComponent<PathNode>();
            text.text = targetNode.name;
        }
    }

    public void PickRandomNeighbouringConnection(bool canUseLastTarget = false)
    {
        if (!targetNode || (targetNode && Vector3.Distance(transform.position, targetNode.transform.position) < targetVarience))
        {
            if (currentNode.usedConnections.Count > 0)
            {
                lastNode = currentNode;
                currentNode = targetNode;
                if (canUseLastTarget)
                {
                    int randomNum = Random.Range(0, currentNode.usedConnections.Count);
                    targetNode = currentNode.usedConnections[randomNum].target.GetComponent<PathNode>();
                }
                else
                {
                    for (int i = 0; i <= 10; i++) // TODO: get rid of the while and use a logical elimination for the last node
                    {
                        int randomNum = Random.Range(0, currentNode.usedConnections.Count);
                        //    int indexOfLastNode = targetNode.usedConnections.Count + 1;
                        //    for (int i = 0; i < targetNode.usedConnections.Count; i++)
                        //    {

                        //        if (targetNode.usedConnections[i].target == currentNode)
                        //        {
                        //            indexOfLastNode = i;
                        //            Debug.Log(i);
                        //            break;
                        //        }
                        //    }
                        //    if (randomNum == indexOfLastNode)
                        //    {
                        //        randomNum++ ;
                        //        randomNum = randomNum % targetNode.usedConnections.Count;
                        //        Debug.Log(randomNum);
                        //    }

                        //}

                        targetNode = currentNode.usedConnections[randomNum].target.GetComponent<PathNode>();
                        if (targetNode != lastNode) break;
                    }
                    text.text = targetNode.usedConnections[0].target.name;
                }
            }
            else
            {
                targetNode = currentNode;
            }
        }
    }
}
