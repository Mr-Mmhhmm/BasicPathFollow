using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathing;

public class HunterAI : PathFinder
{
    public float Speed = 15f;
    private const float SIGHT_RANGE = 30;
    private const float CAPTURE_RANGE = 10;
    public Vector3 goal;
    public bool canSeeGoal;

    public enum State { Search, Follow }
    public State myState;

    // Use this for initialization
    void Start()
    {
        SetCurrentNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSee(SIGHT_RANGE, LayerMasks.onlyRunners, LayerMasks.onlyWalls) || canSeeGoal) myState = State.Follow;
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
                    if (Vector3.Distance(transform.position, goal) < CAPTURE_RANGE) canSeeGoal = false;
                    if (canSeeGoal) GetToDestination(goal);
                    break;
                default:
                    break;
            }
        }

        #region Find our goal
        Collider[] runners = Physics.OverlapSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyRunners);
        float closestRunner = float.PositiveInfinity;
        foreach (Collider runner in runners)
        {
            float distance = Vector3.Distance(transform.position, runner.transform.position);
            if (CanSee(SIGHT_RANGE, runner.transform, LayerMasks.onlyWalls) && distance < closestRunner)
            {
                closestRunner = distance;
                goal = runner.transform.position;
                canSeeGoal = true;
                CallOut(goal);
            }
        }
        #endregion

        if (canSeeGoal && CanSee(CAPTURE_RANGE, LayerMasks.onlyRunners, LayerMasks.onlyWalls))
        {
            Camera.main.GetComponent<SceneController>().MainMenu();
        }

        if (GetTargetNode) transform.position = Vector3.MoveTowards(transform.position, GetTargetNode.transform.position, Speed * Time.deltaTime);
    }

    /// <summary>
    /// Looks out from the center of my body to the center of their body
    /// </summary>
    /// <param name="Range"></param>
    /// <param name="OnlyHit"></param>
    /// <param name="CanBlock"></param>
    /// <returns></returns>
    private bool CanSee(float Range, LayerMask OnlyHit, LayerMask CanBlock)
    {
        bool canSee = false;
        if (Physics.CheckSphere(transform.position, Range, OnlyHit))
        {
            foreach (Collider obj in Physics.OverlapSphere(transform.position, Range, OnlyHit))
            {
                if (!Physics.Linecast(transform.Find("Body").position, obj.transform.position, CanBlock))
                {
                    canSee = true;
                    break;
                }
            }
        }

        return canSee;
    }

    private bool CanSee(float Range, Transform Target, LayerMask CanBlock)
    {
        return !Physics.Linecast(transform.Find("Body").position, Target.position, CanBlock);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, SIGHT_RANGE);

        if (Physics.CheckSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyRunners))
        {
            foreach (Collider obj in Physics.OverlapSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyRunners))
            {
                if (!Physics.Linecast(transform.Find("Body").position, obj.transform.position, LayerMasks.onlyWalls))
                {
                    Gizmos.DrawLine(transform.Find("Body").position, obj.transform.position);
                }
            }
        }
    }

    public void Respond(Vector3 LastSeen)
    {
        //if (!canSeeGoal)
        //{
            goal = LastSeen;
            canSeeGoal = true;
        //}
    }

    private void CallOut(Vector3 LastSeen)
    {
        foreach (GameObject hunter in GameObject.FindGameObjectsWithTag("Hunter"))
        {
            if (hunter.GetComponent<HunterAI>())
            {
                hunter.GetComponent<HunterAI>().Respond(LastSeen);
            }
        }
    }
}