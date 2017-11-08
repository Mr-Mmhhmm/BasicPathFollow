#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class Input_NodeDistance_Popup : PopupWindowContent
{
    float userInput;

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 80);
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.Label("Enter a node distance");
        GUILayout.Label("Currently: " + GridGeneration.DistanceBetweenNodes);
        userInput = EditorGUILayout.FloatField("Node Distance", userInput);
        if (GUILayout.Button("Accept")) ApplyChanges();
        if (GUILayout.Button("Close")) editorWindow.Close();
    }

    private void ApplyChanges()
    {
        GridGeneration.DistanceBetweenNodes = userInput;
    }
}
#endif