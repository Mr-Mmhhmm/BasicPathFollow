#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Pathing
{
    public class SelectObject_Window : EditorWindow
    {
        public enum ActionType { Empty, Connect, Block }
        public ActionType selectedActionType = ActionType.Empty;
        public PathNode controllingNode;
        PathNode obj;

        //Rect windowRect = new Rect(400, 100, 100, 100);

        public void OnGUI()
        {
            switch (selectedActionType)
            {
                case ActionType.Connect:
                    AddConnectionGUI();
                    break;
                case ActionType.Block:
                    AddBlockGUI();
                    break;
                default:
                    throw new System.Exception("No action type Chosen");
            }
        }

        void AddBlockGUI()
        {
            GUILayout.Label("Select a Node to Force Block", EditorStyles.boldLabel);
            // Set values retrieved from window
            obj = (PathNode)EditorGUILayout.ObjectField("Object To Block:", obj, typeof(PathNode), true);

            int choice = GUILayout.SelectionGrid(-1, new string[] { "Accept", "Reject" }, 2);
            if (choice == 0)
            {
                controllingNode.BlockConnetion(obj.gameObject);
                CloseSelf();
            }
            else if (choice == 1)
            {
                CloseSelf();
            }
        }

        void AddConnectionGUI()
        {
            GUILayout.Label("Select a Node to Connect To", EditorStyles.boldLabel);
            // Set values retrieved from window
            obj = (PathNode)EditorGUILayout.ObjectField("Object To Connect:", obj, typeof(PathNode), true);

            int choice = GUILayout.SelectionGrid(-1, new string[] { "Accept", "Reject" }, 2);
            if (choice == 0)
            {
                controllingNode.ForceBuildPath(obj.gameObject);
                CloseSelf();
            }
            else if (choice == 1)
            {
                CloseSelf();
            }
        }

        private void CloseSelf()
        {
            Selection.activeTransform = controllingNode.transform;
            Close();
        }
    }
}
#endif