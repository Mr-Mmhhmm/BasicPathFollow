#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace Pathing
{
    public class JumpDistance_Input_Popup : PopupWindowContent
    {
        private PathNode controllingNode;
        private float distance;

        /// <summary>
        /// used for changeing the one node
        /// </summary>
        /// <param name="ControllingNode"></param>
        /// <param name="currentDistance"></param>
        public JumpDistance_Input_Popup(PathNode ControllingNode, float currentDistance)
        {
            controllingNode = ControllingNode;
            distance = currentDistance;
        }

        /// <summary>
        /// Used for changing the grid spawning script and all selected nodes
        /// </summary>
        /// <param name="currentDistance"></param>
        public JumpDistance_Input_Popup(float currentDistance)
        {
            distance = currentDistance;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 70);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Enter a max Jump Distance");
            distance = EditorGUILayout.FloatField("Distance", distance);
            if (GUILayout.Button("Accept"))
            {
                if (controllingNode) controllingNode.SetMaxJumpDistance(distance);
                else
                {
                    GridGeneration.defaultMaxJump = distance;
                    foreach (Transform item in Selection.transforms)
                    {
                        if (item.GetComponent<PathNode>() && item.tag == "PathNode")
                        {
                            item.GetComponent<PathNode>().SetMaxJumpDistance(distance);
                        }
                        else if (item.GetComponentInChildren<PathNode>())
                        {
                            foreach (PathNode child in item.GetComponentsInChildren<PathNode>())
                            {
                                if (child.tag == "PathNode")
                                {
                                    child.GetComponent<PathNode>().SetMaxJumpDistance(distance);
                                }
                            }
                        }
                    }
                }
                editorWindow.Close();
            }
        }
    }
}
#endif