using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAI : PathFinder
{
    public float Speed = 15f;
    // Use this for initialization
    void Start()
    {
        SetCurrentNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (HasReachedTarget)
        {
            PickRandomNeighbouringConnection();
        }
        if (GetTargetNode) transform.position = Vector3.MoveTowards(transform.position, GetTargetNode.transform.position, Speed * Time.deltaTime);
    }
}
