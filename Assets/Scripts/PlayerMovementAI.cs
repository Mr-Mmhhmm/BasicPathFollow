using UnityEngine;

public class PlayerMovementAI : PathFinder
{
    private const float Speed = 4f;

    private void Start()
    {
        SetCurrentNode();
    }
    private void Update()
    {
        //if (!GetCurrentNode) SetCurrentNode();

        if (Vector3.Distance(transform.position, GetTargetNode.transform.position) < targetVarience)
        {
            PickRandomNeighbouringConnection();
        }
        if (GetTargetNode) transform.position = Vector3.MoveTowards(transform.position, GetTargetNode.transform.position, Speed * Time.deltaTime);
    }
}