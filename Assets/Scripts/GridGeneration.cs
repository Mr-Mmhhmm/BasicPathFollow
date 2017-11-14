#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace Pathing
{
    public class GridGeneration : Editor
    {
        private static float distanceBetweenNodes = 5;
        public static float DistanceBetweenNodes
        {
            get { return distanceBetweenNodes; }
            set { distanceBetweenNodes = value; }//Undo.RegisterCompleteObjectUndo(this, "Changed DistanceBetweenNodes"); }
        }
        private static float nodeSightRange = 5;
        public static float NodeSightRange
        {
            get { return nodeSightRange; }
            set { nodeSightRange = value; } //Undo.RegisterCompleteObjectUndo(this, "Changed NodeSightRange"); } }
        }

        public static float defaultMaxJump = 10;
        public static float defaultMaxDrop = 5;

        private static int m_count = 0;
        private static float PATHNODE_RADIOUS = 0.5f;

        /// <summary>
        /// Returns true if there is a node or node container selected
        /// </summary>
        private static bool HasNodeSelected
        {
            get
            {
                // true if a node is selected
                bool hasNode = false;
                foreach (Transform transform in Selection.transforms)
                {
                    if (transform.tag == "PathNode" || transform.tag == "PathNode_Container")
                    {
                        hasNode = true;
                        break;
                    }
                }
                return hasNode;
            }
        }


        // ------------------------------Spawn------------------------------
        [MenuItem("Grid Manager/Spawn Trigger Pathnodes", priority = 0)]
        private static void SpawnTriggerNodes()
        {
            //ClearSpheres();

            // Load the objects i need
            GameObject pathNode = (GameObject)Resources.Load("PathNode");
            LayerMask everything = -1;

            // Get the triggers i will spawn nodes in
            Transform[] triggers = Selection.transforms;
            foreach (Transform trigger in triggers)
            {
                if (trigger.GetComponent<BoxCollider>() && trigger.GetComponent<BoxCollider>().isTrigger) // Is it really a trigger?
                {
                    // Find the area where i need to spawn the nodes
                    BoxCollider spawnBox = trigger.GetComponent<BoxCollider>();
                    Vector3 topFrontRight = spawnBox.bounds.extents + spawnBox.transform.position; // Top front right position
                    Vector3 topBackLeft = new Vector3(spawnBox.transform.position.x - spawnBox.bounds.extents.x, spawnBox.bounds.extents.y + spawnBox.transform.position.y, spawnBox.transform.position.z - spawnBox.bounds.extents.z); // Top back left position

                    for (float x = topBackLeft.x; x <= topFrontRight.x; x += distanceBetweenNodes)
                    {
                        for (float z = topBackLeft.z; z <= topFrontRight.z; z += distanceBetweenNodes)
                        {
                            RaycastHit hitInfo;
                            if (Physics.Raycast(new Vector3(x, topFrontRight.y, z), Vector3.down, out hitInfo, spawnBox.bounds.extents.y * 2, everything, QueryTriggerInteraction.Ignore))
                            {
                                GameObject pathnodeInstance = Instantiate(pathNode, new Vector3(x, hitInfo.point.y + PATHNODE_RADIOUS, z), spawnBox.transform.rotation, hitInfo.transform);
                                pathnodeInstance.GetComponent<PathNode>().SetupNode(nodeSightRange, "Pathnode" + m_count.ToString());

                                Undo.RegisterCompleteObjectUndo(pathnodeInstance, "Created Node");

                                m_count++;
                            }
                        }
                    }
                }
            }
        }

        [MenuItem("Grid Manager/Spawn Pathnodes On Floor", priority = 1)]
        private static void SpawnNodesOnFloor()
        {
            // Load the objects i need
            GameObject pathNode = (GameObject)Resources.Load("PathNode");

            //ClearSpheres();

            // Find the area where i need to spawn the nodes
            if (Selection.transforms.Length >= 1) // If we have items selected
            {
                foreach (Transform possibleFloorItem in Selection.transforms)
                {
                    if (possibleFloorItem.tag == "Floor") // If we have a floor item
                    {
                        BoxCollider spawnBox = possibleFloorItem.GetComponent<BoxCollider>();
                        Vector3 topFrontRight = spawnBox.bounds.extents + spawnBox.transform.position; // Top front right position
                        Vector3 topBackLeft = new Vector3(spawnBox.transform.position.x - spawnBox.bounds.extents.x, spawnBox.bounds.extents.y + spawnBox.transform.position.y, spawnBox.transform.position.z - spawnBox.bounds.extents.z); // Top back left position

                        for (float x = topBackLeft.x; x <= topFrontRight.x; x += distanceBetweenNodes)
                        {
                            for (float z = topBackLeft.z; z <= topFrontRight.z; z += distanceBetweenNodes)
                            {
                                GameObject pathnodeInstance = Instantiate(pathNode, new Vector3(x, topBackLeft.y + PATHNODE_RADIOUS, z), spawnBox.transform.rotation, possibleFloorItem);
                                pathnodeInstance.GetComponent<PathNode>().SetupNode(nodeSightRange, "Pathnode" + m_count.ToString());

                                Undo.RegisterCompleteObjectUndo(pathnodeInstance, "Created Node");

                                m_count++;
                            }
                        }
                    }
                }
            }
        }

        // ------------------------------Clear------------------------------
        [MenuItem("Grid Manager/Clear All Pathnodes", priority = 20)]
        private static void ClearAllNodes()
        {
            GameObject[] nodes = GameObject.FindGameObjectsWithTag("PathNode");
            foreach (GameObject node in nodes)
            {
                DestroyImmediate(node);
            }
        }
        [MenuItem("Grid Manager/Clear All Pathnodes", true, priority = 20)]
        private static bool ValidateClearAllNodes()
        {
            return GameObject.FindGameObjectsWithTag("PathNode").Length > 0;
        }


        [MenuItem("Grid Manager/Clear Floor Pathnodes", priority = 21)]
        private static void ClearFloorNodes()
        {
            Transform[] containers = Selection.transforms;
            if (containers.Length <= 1)
            {
                foreach (Transform container in containers)
                {
                    PathNode[] nodes = container.GetComponentsInChildren<PathNode>();
                    foreach (PathNode node in nodes)
                    {
                        if (node.tag == "PathNode") DestroyImmediate(node.gameObject);
                    }
                }
            }
        }
        [MenuItem("Grid Manager/Clear Floor Pathnodes", true, priority = 21)]
        private static bool ValidateClearFloorNodes()
        {
            bool flag = false;
            Transform[] containers = Selection.transforms;
            if (containers.Length >= 1)
            {
                foreach (Transform container in containers)
                {
                    if (container.GetComponentInChildren<PathNode>())
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        }



        #region Colors
        // ------------------------------Red------------------------------
        [MenuItem("Grid Manager/Color/Red", priority = 40)]
        static void ColorRed()
        {
            Debug.Log("Red");
            foreach (Transform item in Selection.transforms)
            {
                if (item.GetComponent<PathNode>()) item.GetComponent<PathNode>().Color = Color.red;
                else if (item.GetComponentInChildren<PathNode>())
                {
                    foreach (PathNode node in item.GetComponentsInChildren<PathNode>()) node.Color = Color.red;
                }
            }
        }

        [MenuItem("Grid Manager/Color/Red", true)]
        static bool ValidateColorRed()
        {
            return HasNodeSelected;
        }

        // ------------------------------Blue------------------------------
        [MenuItem("Grid Manager/Color/Blue", priority = 41)]
        private static void ColorBlue()
        {
            foreach (Transform item in Selection.transforms)
            {
                if (item.GetComponent<PathNode>()) item.GetComponent<PathNode>().Color = Color.blue;
                else if (item.GetComponentInChildren<PathNode>())
                {
                    foreach (PathNode node in item.GetComponentsInChildren<PathNode>()) node.Color = Color.blue;
                }
            }

        }
        [MenuItem("Grid Manager/Color/Blue", true)]
        static bool ValidateColorBlue()
        {
            return HasNodeSelected;
        }

        // ------------------------------Green------------------------------
        [MenuItem("Grid Manager/Color/Green", priority = 42)]
        private static void ColorGreen()
        {
            foreach (Transform item in Selection.transforms)
            {
                if (item.GetComponent<PathNode>()) item.GetComponent<PathNode>().Color = Color.green;
                else if (item.GetComponentInChildren<PathNode>())
                {
                    foreach (PathNode node in item.GetComponentsInChildren<PathNode>()) node.Color = Color.green;
                }
            }
        }

        [MenuItem("Grid Manager/Color/Green", true)]
        static bool ValidateColorGreen()
        {
            return HasNodeSelected;
        }
        #endregion

        // ------------------------------Match------------------------------
        [MenuItem("Grid Manager/Match", priority = 60)]
        private static void Match()
        {
            Color firstColor = Selection.transforms[0].GetComponent<PathNode>().Color;
            if (Selection.transforms[1].GetComponent<PathNode>().Color == firstColor && Selection.transforms[2].GetComponent<PathNode>().Color == firstColor)
            {
                Debug.Log("They match! " + firstColor.ToString());
                DestroyImmediate(Selection.transforms[0].gameObject);
                DestroyImmediate(Selection.transforms[0].gameObject);
                DestroyImmediate(Selection.transforms[0].gameObject);
            }
            else Debug.Log("They do not match.");
        }
        [MenuItem("Grid Manager/Match", true)]
        private static bool ValidateMatch()
        {
            return Selection.transforms.Length == 3 && Selection.transforms[0].tag == "PathNode" && Selection.transforms[1].tag == "PathNode" && Selection.transforms[2].tag == "PathNode";
        }

        // ------------------------------Paths------------------------------
        [MenuItem("Grid Manager/Build Selected Node's Paths", priority = 80)]
        private static void BuildPaths()
        {
            GameObject[] allNodes = GameObject.FindGameObjectsWithTag("PathNode");
            if (Selection.transforms.Length != 0)
            {
                // Clear connections
                foreach (Transform selected in Selection.transforms)
                {
                    if (selected.GetComponent<PathNode>())
                    {
                        selected.GetComponent<PathNode>().ClearPaths();
                    }
                    // If a selected object is a pathnode container then we will build paths for that containers pathnode children.
                    else if (selected.GetComponentInChildren<PathNode>())
                    {
                        foreach (PathNode selectedChild in selected.GetComponentsInChildren<PathNode>())
                        {
                            selectedChild.ClearPaths();
                        }
                    }
                }

                // Create connections
                foreach (Transform selected in Selection.transforms)
                {
                    if (selected.GetComponent<PathNode>())
                    {
                        // build paths for the selected node
                        foreach (GameObject node in allNodes)
                        {
                            selected.GetComponent<PathNode>().BuildPath(node);
                        }
                    }
                    // If a selected object is a pathnode container then we will build paths for that containers pathnode children.
                    else if (selected.GetComponentInChildren<PathNode>())
                    {
                        foreach (PathNode selectedChild in selected.GetComponentsInChildren<PathNode>())
                        {
                            foreach (GameObject node in allNodes)
                            {
                                selectedChild.GetComponent<PathNode>().BuildPath(node);
                            }
                        }
                    }
                }
            }
        }
        [MenuItem("Grid Manager/Build Selected Node's Paths", true)]
        private static bool ValidateBuildPaths()
        {
            bool flag = false;
            if (Selection.transforms.Length != 0)
            {
                foreach (Transform selected in Selection.transforms)
                {
                    if (selected.GetComponent<PathNode>())
                    {
                        flag = true;
                        break;
                    }
                    else if (selected.GetComponentInChildren<PathNode>())
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        }

        [MenuItem("Grid Manager/Clear All Paths", priority = 81)]
        private static void ClearAllPaths()
        {
            GameObject[] allNodes = GameObject.FindGameObjectsWithTag("PathNode");
            foreach (GameObject node in allNodes) node.GetComponent<PathNode>().ClearAllPaths();
        }
        [MenuItem("Grid Manager/Clear All Paths", true)]
        private static bool ValidateClearAllPaths()
        {
            return GameObject.FindGameObjectsWithTag("PathNode").Length > 0;
        }

        // ------------------------------Change Variables------------------------------
        [MenuItem("Grid Manager/Change Node Spacing", priority = 100)]
        private static void ChangeNodeSpacing()
        {
            PopupWindow.Show(new Rect(20, 20, 100, 100), new Input_NodeDistance_Popup());
        }

        [MenuItem("Grid Manager/Change Arrow Distance", priority = 101)]
        private static void ChangeArrowDistance()
        {
            PopupWindow.Show(new Rect(20, 20, 100, 100), new Input_NodeRange_Popup());
        }

        [MenuItem("Grid Manager/Change Max Jump Distance", priority = 102)]
        private static void ChangeMaxJumpDistance()
        {
            PopupWindow.Show(new Rect(100, 100, 200, 100), new JumpDistance_Input_Popup(defaultMaxJump));
        }

        [MenuItem("Grid Manager/Change Max Drop Height", priority = 103)]
        private static void ChangeMaxDropHeight()
        {
            PopupWindow.Show(new Rect(100, 100, 200, 100), new DropHeight_Input_Popup(defaultMaxDrop));
        }
    }
}
#endif