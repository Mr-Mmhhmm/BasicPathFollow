#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class DropHeight_Input_Popup : PopupWindowContent
{
    private PathNode controllingNode;
    private float height;

    /// <summary>
    /// used for changeing the one node
    /// </summary>
    /// <param name="ControllingNode"></param>
    /// <param name="currentDistance"></param>
    public DropHeight_Input_Popup(PathNode ControllingNode, float currentHeight)
    {
        controllingNode = ControllingNode;
        height = currentHeight;
    }

    /// <summary>
    /// Used for changing the grid spawning script and all selected nodes
    /// </summary>
    /// <param name="currentDistance"></param>
    public DropHeight_Input_Popup(float currentHeight)
    {
        height = currentHeight;
    }

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 70);
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Label("Enter a max Drop Height");
        height = EditorGUILayout.FloatField("Height", height);
        if (GUILayout.Button("Accept"))
        {
            if (controllingNode) controllingNode.SetMaxDropHeight(height);
            else
            {
                GridGeneration.defaultMaxDrop = height;
                foreach (Transform item in Selection.transforms)
                {
                    if (item.GetComponent<PathNode>() && item.tag == "PathNode")
                    {
                        item.GetComponent<PathNode>().SetMaxDropHeight(height);
                    }
                    else if (item.GetComponentInChildren<PathNode>())
                    {
                        foreach (PathNode child in item.GetComponentsInChildren<PathNode>())
                        {
                            if (child.tag == "PathNode")
                            {
                                child.GetComponent<PathNode>().SetMaxDropHeight(height);
                            }
                        }
                    }
                }
            }
            editorWindow.Close();
        }
    }
}
#endif