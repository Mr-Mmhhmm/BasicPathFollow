using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Pathing
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class PathNode : MonoBehaviour
    {
        public float SIGHT_RANGE;
#if UNITY_EDITOR
    [SerializeField] [ReadOnly]
#endif
        private float maxJumpDistance = 10;
#if UNITY_EDITOR
    [SerializeField] [ReadOnly]
#endif
        private float maxDropHeight = 5;

#if UNITY_EDITOR
    [SerializeField] [ReadOnly]
#endif
        public List<GameObject> connections; // updated with "BuildConnections"
#if UNITY_EDITOR
    [SerializeField] [ReadOnly]
#endif
        public List<GameObject> forcedConnections; // updated by user forcing connections
#if UNITY_EDITOR
    [SerializeField] [ReadOnly]
#endif
        public List<GameObject> blockedConnections; // updated by user blocking connections

        public List<Connection> usedConnections;

        public Color nodeColor;
        public Color defaultNodeColor = Color.yellow;
#if UNITY_EDITOR
    private float connectionArrowPercent = 0;
#endif
        public float ARROW_SPEED;
        public bool isDoorClosed;

        public Color Color
        {
            get
            {
                return nodeColor;
            }
            set
            {
                nodeColor = value;
#if UNITY_EDITOR
                Undo.RegisterCompleteObjectUndo(this, "Changed color");
#endif
            }
        }

        public float sightRange
        {
            set
            {
                if (value > 0)
                {
                    SIGHT_RANGE = value;
#if UNITY_EDITOR
                    Undo.RegisterCompleteObjectUndo(this, "Changed sightRange");
#endif
                }
            }
        }

        /// <summary>
        /// returns a path to an open door, or null if it cant find an open door
        /// </summary>
        public PathNode GetRandomActiveConnection
        {
            get
            {
                PathNode targetNode = null;
                for (int i = 0; i <= 10; i++) // TODO: get rid of the while and use a logical elimination for the last node
                {
                    PathNode tryNode = usedConnections[Random.Range(0, usedConnections.Count)].target.GetComponent<PathNode>();
                    if (!tryNode.isDoorClosed)
                    {
                        targetNode = tryNode;
                        break;
                    }
                }
                return targetNode;
            }
        }

        public void SetupNode(float SightRange, string Name)
        {
            sightRange = SightRange;
            transform.name = Name;
        }

        private void Start()
        {
            Gizmos.color = nodeColor;
            //AssembleUsedConnectionList();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            connectionArrowPercent += ARROW_SPEED / 100;
            if (connectionArrowPercent >= 100) connectionArrowPercent = 20;

            if (nodeColor.a == 0) nodeColor = defaultNodeColor;
            Gizmos.color = nodeColor;

            Gizmos.DrawSphere(transform.position, 1);

            // Connections
            if (usedConnections != null && usedConnections.Count > 0)
            {
                for (int i = 0; i < usedConnections.Count; i++)
                {
                    if (usedConnections[i].target) usedConnections[i].Draw(this, connectionArrowPercent);
                    else
                    {
                        usedConnections.Remove(usedConnections[i]);
                        i--;
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (nodeColor.a == 0) nodeColor = defaultNodeColor;
            Gizmos.color = nodeColor;

            //connectionArrowPercent = 50f;

            Gizmos.color = Color.black;
            if (Selection.Contains(gameObject))
            {
                Gizmos.DrawWireSphere(transform.position, SIGHT_RANGE);
            }
        }
#endif

        /// <summary>
        /// Clears the connections list and the forced list
        /// </summary>
        public void ClearAllPaths()
        {
            connections.Clear(); // Empty list
            forcedConnections.Clear();
            AssembleUsedConnectionList();
        }

        /// <summary>
        /// Clears only the connections list
        /// </summary>
        public void ClearPaths()
        {
            connections.Clear(); // Empty list
                                 //forcedConnections.Clear();
            AssembleUsedConnectionList();
        }

        /// <summary>
        /// Does not build blocked paths. Does regard distance and line-of-sight. Checks to see if the target has a pathnode script
        /// </summary>
        /// <param name="target"></param>
        public void BuildPath(GameObject target)
        {
            if (target.GetComponent<PathNode>()) // it is a pathnode
            {
                if (target && target.transform != transform) // Not myself and not already connected
                {
                    if (Vector3.Distance(transform.position, target.transform.position) <= SIGHT_RANGE) // In range
                    {
                        if (!Physics.Linecast(transform.position, target.transform.position, LayerMasks.onlyWalls, QueryTriggerInteraction.Ignore)) // Can see
                        {
                            if (!blockedConnections.Contains(target) && !forcedConnections.Contains(target) && !connections.Contains(target)) // not in a list
                            {
                                connections.Add(target);
                                AssembleUsedConnectionList();
#if UNITY_EDITOR
                                Undo.RegisterCompleteObjectUndo(this, "Built Path");
#endif
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does not care about distances or walls
        /// </summary>
        /// <param name="target"></param>
        public void ForceBuildPath(GameObject target)
        {
            if (target && target.transform != transform) // not null and Not myself
            {
                blockedConnections.Remove(target); // Remove it from the blocked list
                connections.Remove(target); // Remove it from the normal list
                forcedConnections.Remove(target); // Remove it from the forced list

                if (target != gameObject) // Is it not myself?
                {
                    forcedConnections.Add(target);
                    AssembleUsedConnectionList();
#if UNITY_EDITOR
                    Undo.RegisterCompleteObjectUndo(this, "Forced Path");
#endif
                }
            }
        }

        public void BlockConnetion(GameObject target)
        {
            blockedConnections.Remove(target); // Remove it from the blocked list
            connections.Remove(target); // Remove it from the normal list
            forcedConnections.Remove(target); // Remove it from the forced list

            if (target != gameObject) // Is it not myself?
            {
                // Block it
                blockedConnections.Add(target);
                AssembleUsedConnectionList();
#if UNITY_EDITOR
                Undo.RegisterCompleteObjectUndo(this, "Blocked Path");
#endif
            }
        }

        public void AssembleUsedConnectionList()
        {
            if (usedConnections == null) usedConnections = new List<Connection>();
            else usedConnections.Clear();

            if (connections.Count > 0)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    usedConnections.Add(Connection.CreateInstance(transform.position, connections[i], maxJumpDistance, maxDropHeight));
                }
            }
            if (forcedConnections.Count > 0)
            {
                for (int i = 0; i < forcedConnections.Count; i++)
                {
                    usedConnections.Add(Connection.CreateInstance(transform.position, forcedConnections[i], maxJumpDistance, maxDropHeight));
                }
            }
#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(this, "Assembled Path to Use");
#endif
        }

        public PathNode NodeClosestTo(Vector3 endDestination, PathNode cantUse = null)
        {
            PathNode target = null;
            float shortestDistance = float.PositiveInfinity;
            foreach (Connection connection in usedConnections)
            {
                float distance = Vector3.Distance(connection.target.transform.position, endDestination);
                if (!connection.target.GetComponent<PathNode>().isDoorClosed && distance < shortestDistance && connection.target.GetComponent<PathNode>() != cantUse)
                {
                    shortestDistance = distance;
                    target = connection.target.GetComponent<PathNode>();
                }
            }
            return target;
        }

        public void SetMaxJumpDistance(float distance)
        {
            maxJumpDistance = distance;
            RefreshUsedConnectionsVariables();
        }

        public void SetMaxDropHeight(float height)
        {
            maxDropHeight = height;
            RefreshUsedConnectionsVariables();
        }

#if UNITY_EDITOR
        // ---------------------------------Jumping and Dropping---------------------------------
        [ContextMenu("Refresh Connections", false, 780)]
#endif
        public void RefreshUsedConnectionsVariables()
        {
            foreach (Connection connection in usedConnections)
            {
                connection.RefreshConnectionInfo(transform.position, maxJumpDistance, maxDropHeight);
            }
        }
#if UNITY_EDITOR
        [ContextMenu("Change Max Jump Distance", false, 781)]
        public void ChangeJumpDistance()
        {
            PopupWindow.Show(new Rect(100, 100, 200, 100), new JumpDistance_Input_Popup(this, maxJumpDistance));
        }
        [ContextMenu("Change Max Drop Height", false, 781)]
        public void ChangeDropHeight()
        {
            PopupWindow.Show(new Rect(100, 100, 200, 100), new DropHeight_Input_Popup(this, maxDropHeight));
        }




        // ---------------------------------Connecting---------------------------------
        [ContextMenu("Build Area Connections", false, 800)]
        private void BuildMyOwnConnections()
        {
            ClearPaths();

            foreach (GameObject target in GameObject.FindGameObjectsWithTag("PathNode"))
            {
                BuildPath(target);
            }
        }

        // ---------------------------------Forcing Connections---------------------------------
        [ContextMenu("Add Forced Connection", false, 820)]
        private void AddNewForcedConnection()
        {
            SelectObject_Window window = (SelectObject_Window)EditorWindow.GetWindow(typeof(SelectObject_Window));
            window.controllingNode = this;
            window.selectedActionType = SelectObject_Window.ActionType.Connect;
        }

        [ContextMenu("Remove Forced Connection", false, 821)]
        private void RemoveForcedConnection()
        {
            try
            {
                PopupWindow.Show(new Rect(20, 20, 100, 100), new RemoveObject_Popup(this, forcedConnections, RemoveObject_Popup.PlaceToRemoveFrom.forced));
            }
            catch (ExitGUIException e)
            {
                Debug.Log(e.ToString());
            }
        }
        [ContextMenu("Remove Forced Connection", true)]
        private bool ValidateRemoveConnection()
        {
            return forcedConnections.Count > 0;
        }

        // ---------------------------------Blocking---------------------------------
        [ContextMenu("Add Blocked Connection", false, 840)]
        private void AddNewBlock()
        {
            SelectObject_Window window = (SelectObject_Window)EditorWindow.GetWindow(typeof(SelectObject_Window));
            window.controllingNode = this;
            window.selectedActionType = SelectObject_Window.ActionType.Block;
        }
        [ContextMenu("Remove Blocked Connection", false, 841)]
        private void RemoveBlock()
        {
            try
            {
                PopupWindow.Show(new Rect(20, 20, 100, 100), new RemoveObject_Popup(this, blockedConnections, RemoveObject_Popup.PlaceToRemoveFrom.blocked));
            }
            catch (ExitGUIException e)
            {
                Debug.Log(e.ToString());
            }
        }
        [ContextMenu("Remove Blocked Connection", true)]
        private bool ValidateRemoveBlock()
        {
            return blockedConnections.Count > 0;
        }

        // ---------------------------------Toggle Door---------------------------------
        [ContextMenu("Toggle Door", false, 860)]
        public void ToggleDoor()
        {
            isDoorClosed = !isDoorClosed;
            Undo.RegisterCompleteObjectUndo(this, "Door Toggled");
        }
#endif
    }
}