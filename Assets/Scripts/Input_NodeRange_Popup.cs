#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Pathing
{
    internal class Input_NodeRange_Popup : PopupWindowContent
    {
        float userInput;

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 80);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Enter a sight distance");
            GUILayout.Label("Currently: " + GridGeneration.NodeSightRange);
            userInput = EditorGUILayout.FloatField("Sight Distance", userInput);
            if (GUILayout.Button("Accept")) ApplyChanges();
            if (GUILayout.Button("Close")) editorWindow.Close();
        }

        private void ApplyChanges()
        {
            GridGeneration.NodeSightRange = userInput;
            Debug.Log("Distance Between Nodes Updated");

            if (EditorUtility.DisplayDialog("Change Node Range", "Do you want to update selected nodes to reflect this change?", "Yes", "No"))
            {
                foreach (Transform item in Selection.transforms)
                {
                    if (item.GetComponent<PathNode>())
                    {
                        item.GetComponent<PathNode>().sightRange = userInput;
                    }
                    else if (item.GetComponentInChildren<PathNode>())
                    {
                        foreach (PathNode child in item.GetComponentsInChildren<PathNode>())
                        {
                            child.sightRange = userInput;
                        }
                    }
                }
            }
        }
    }
}
#endif