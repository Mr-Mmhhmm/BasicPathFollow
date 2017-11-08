
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private PathNode currentNode;
    public PathNode GetCurrentNode { get { return currentNode; } }
    private PathNode lastNode;

    private PathNode targetNode;
    public PathNode GetTargetNode { get { return targetNode; } }
    public const float targetVarience = 1;

    public bool HasReachedTarget
    {
        get
        {
            return Vector3.Distance(transform.position, targetNode.transform.position) < targetVarience;
        }
    }


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
        }
    }

    public void PickRandomNeighbouringConnection(bool canUseLastTarget = false)
    {
        if (!targetNode || (targetNode && Vector3.Distance(transform.position, targetNode.transform.position) < targetVarience))
        {
            lastNode = currentNode;
            currentNode = targetNode;

            if (currentNode.usedConnections.Count > 0)
            {
                if (canUseLastTarget)
                {
                    targetNode = currentNode.GetComponent<PathNode>().GetRandomActiveConnection;
                }
                else
                {
                    for (int i = 0; i < 20; i++)
                    {
                        targetNode = currentNode.GetComponent<PathNode>().GetRandomActiveConnection;
                        if (targetNode && targetNode != lastNode) break;
                    }
                }
            }
            //else
            //{
            //    targetNode = currentNode;
            //}
        }
    }

    public void GetToDestination(Vector3 endDestination, bool canUseLastTarget = false)
    {
        lastNode = currentNode;
        currentNode = targetNode;
        targetNode = currentNode.NodeClosestTo(endDestination, lastNode);
    }
}
