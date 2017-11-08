using UnityEngine;

public class PlayerMovementAI : PathFinder
{
    private const float Speed = 4f;

    private void Start()
    {
        SetCurrentNode();
        text.text = GetTargetNode.usedConnections[0].target.name;
    }
    private void Update()
    {
        //if (!GetCurrentNode) SetCurrentNode();

        if (Vector3.Distance(transform.position, GetTargetNode.transform.position) < targetVarience)
        {
            PickRandomNeighbouringConnection();
        }

        //transform.LookAt(new Vector3(targetNode.transform.position.x, transform.position.y, targetNode.transform.position.z));
        if (GetTargetNode) transform.position = Vector3.MoveTowards(transform.position, GetTargetNode.transform.position, Speed * Time.deltaTime);
    }
}