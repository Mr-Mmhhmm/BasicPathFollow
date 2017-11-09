using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterAI : PathFinder
{
    public float Speed = 15f;
    private const float SIGHT_RANGE = 50;
    private Transform goal;

    private enum State { Search, Follow }
    private State myState;

    // Use this for initialization
    void Start()
    {
        SetCurrentNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSee(SIGHT_RANGE, LayerMasks.onlyRunners, LayerMasks.onlyWalls)) myState = State.Follow;
        else myState = State.Search;


        if (HasReachedTarget)
        {
            switch (myState)
            {
                case State.Search:
                    if (HasReachedTarget)
                    {
                        PickRandomNeighbouringConnection();
                    }
                    break;
                case State.Follow:
                    #region Find our goal
                    float closestRunner = float.PositiveInfinity;
                    foreach (Collider runner in Physics.OverlapSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyRunners))
                    {
                        float distance = Vector3.Distance(transform.position, runner.transform.position);
                        if (distance < closestRunner)
                        {
                            closestRunner = distance;
                            goal = runner.transform;
                        }
                    }
                    #endregion

                    if (goal) GetToDestination(goal.position);

                    break;
                default:
                    break;
            }
        }
        if (GetTargetNode) transform.position = Vector3.MoveTowards(transform.position, GetTargetNode.transform.position, Speed * Time.deltaTime);
    }



    private bool CanSee(float Range, LayerMask OnlyHit, LayerMask CanBlock)
    {
        bool canSee = false;
        if (Physics.CheckSphere(transform.position, Range, OnlyHit))
        {
            foreach (Collider obj in Physics.OverlapSphere(transform.position, Range, OnlyHit))
            {
                if (!Physics.Linecast(transform.position, obj.transform.position, CanBlock))
                {
                    canSee = true;
                    break;
                }
            }
        }

        return canSee;
    }
}
