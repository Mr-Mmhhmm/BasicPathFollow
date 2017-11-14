using System.Collections.Generic;
using UnityEngine;
using Pathing;

public class RunnerAI : PathFinder
{
    private const float PICKUP_RADIUS = 5;
    private const float SIGHT_RANGE = 40;

    public float NormalSpeed = 15f;
    public float SprintSpeed = 30f;
    private float endSprintTime;
    private const float DELTA_SPRINT_TIME = 2;

    private List<GameObject> keys;
    private Transform goal;

    private enum State { Wander, PickupKey, Flee, Escape }
    private State myState;

    private void Start()
    {
        SetCurrentNode();
        keys = new List<GameObject>(GameObject.FindGameObjectsWithTag("Key"));
    }
    private void Update()
    {

        if (CanSee(SIGHT_RANGE, LayerMasks.onlyHunters, LayerMasks.onlyWalls)) myState = State.Flee;
        else if (CanSee(SIGHT_RANGE, LayerMasks.onlyKeys, LayerMasks.onlyWalls)) myState = State.PickupKey;
        else if (keys.Count == 0 && CanSee(SIGHT_RANGE, LayerMasks.onlyDoors, LayerMasks.onlyHunters)) myState = State.Escape;
        else myState = State.Wander;



        if (HasReachedTarget)
        {
            switch (myState)
            {
                case State.Wander:
                    PickRandomNeighbouringConnection();
                    break;
                case State.PickupKey:
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (Vector3.Distance(transform.position, keys[i].transform.position) < PICKUP_RADIUS)
                        {
                            Destroy(keys[i].gameObject);
                            keys.Remove(keys[i]);

                            if (keys.Count == 0)
                            {
                                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Exit"))
                                {
                                    obj.GetComponentInChildren<PathNode>().isDoorClosed = false;
                                }
                            }
                        }
                    }

                    #region Find next key
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

                    break;
                case State.Flee:
                    endSprintTime = Time.time + DELTA_SPRINT_TIME;
                    Collider[] hunters = Physics.OverlapSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyHunters);
                    Vector3 safeSpot = transform.position;
                    foreach (Collider hunter in hunters)
                    {
                        if (!Physics.Linecast(transform.position, hunter.transform.position, LayerMasks.onlyWalls))
                        {
                            //Debug.Log("Fleeing from " + hunter.transform.parent.name);
                            safeSpot += transform.position - hunter.transform.position;
                        }
                    }
                    //transform.position = safeSpot;
                    GetToDestination(safeSpot);
                    break;
                case State.Escape:
                    if (Vector3.Distance(transform.position, Physics.OverlapSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyDoors)[0].transform.parent.GetComponentInChildren<PathNode>().transform.position) <= targetVarience)
                    {
                        Camera.main.GetComponent<SceneController>().MainMenu();
                    }

                    goal = Physics.OverlapSphere(transform.position, SIGHT_RANGE, LayerMasks.onlyDoors)[0].transform;
                    if (goal) GetToDestination(goal.position);
                    break;
                default:
                    break;
            }
        }

        // Do the movement
        if (GetTargetNode) transform.position = Vector3.MoveTowards(transform.position, GetTargetNode.transform.position, (Time.time < endSprintTime ? SprintSpeed : NormalSpeed) * Time.deltaTime);
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