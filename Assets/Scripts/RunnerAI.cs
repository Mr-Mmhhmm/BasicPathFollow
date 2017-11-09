using UnityEngine;

public class RunnerAI : PathFinder
{
    private const float PICKUP_RADIUS = 5;
    private const float SIGHT_RANGE = 40;

    public float Speed = 15f;
    private int KeysToFind;
    private Transform goal;

    private enum State { Wander, PickupKey, Flee, Escape }
    private State myState;

    private void Start()
    {
        SetCurrentNode();
        KeysToFind = GameObject.FindGameObjectsWithTag("Key").Length;
    }
    private void Update()
    {

        if (CanSee(SIGHT_RANGE, LayerMasks.onlyHunters, LayerMasks.onlyWalls)) myState = State.Flee;
        else if (CanSee(SIGHT_RANGE, LayerMasks.onlyKeys, LayerMasks.onlyWalls)) myState = State.PickupKey;
        else /*if (KeysToFind > 0)*/ myState = State.Wander;
        //else myState = State.Escape;



        if (HasReachedTarget)
        {
            switch (myState)
            {
                case State.Wander:
                    PickRandomNeighbouringConnection();
                    break;
                case State.PickupKey:
                    if (Physics.CheckSphere(transform.position, PICKUP_RADIUS, LayerMasks.onlyKeys))
                    {
                        Collider[] keys = Physics.OverlapSphere(transform.position, PICKUP_RADIUS, LayerMasks.onlyKeys);
                        foreach (Collider key in keys)
                        {
                            KeysToFind--;
                            Destroy(key.gameObject);
                        }
                    }
                    else
                    {
                        #region Find our goal
                        float closestKey = float.PositiveInfinity;
                        foreach (Collider key in Physics.OverlapSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyKeys))
                        {
                            float distance = Vector3.Distance(transform.position, key.transform.position);
                            if (distance < closestKey)
                            {
                                closestKey = distance;
                                goal = key.transform;
                            }
                        }
                        #endregion

                        if (goal) GetToDestination(goal.position);
                    }

                    break;
                case State.Flee:
                    //Debug.Log("Flee");
                    Collider[] hunters = Physics.OverlapSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyHunters);
                    Vector3 safeSpot = transform.position;
                    foreach (Collider hunter in hunters)
                    {
                        if (!Physics.Linecast(transform.position, hunter.transform.position, LayerMasks.onlyWalls))
                        {
                            Debug.Log("Fleeing from " + hunter.name);
                            safeSpot += transform.position - hunter.transform.position;
                        }
                    }
                    //transform.position = safeSpot;
                    GetToDestination(safeSpot);
                    break;
                case State.Escape:
                    break;
                default:
                    break;
            }
        }

        //if (Vector3.Distance(transform.position, GetTargetNode.transform.position) < targetVarience)
        //{ // Reached target
        //    if (goal)
        //    {
        //        // Go to a destination
        //        GetToDestination(goal.position);
        //    }
        //    else
        //    {
        //        // Move randomly
        //        
        //    }
        //}

        // Do the movement
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