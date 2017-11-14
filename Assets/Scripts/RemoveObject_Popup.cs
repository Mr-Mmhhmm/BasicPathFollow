#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Pathing
{
    public class RemoveObject_Popup : PopupWindowContent
    {
        private PathNode controllingNode;
        private List<GameObject> connections;
        private List<bool> choices;
        public enum PlaceToRemoveFrom { forced, blocked }
        private PlaceToRemoveFrom placeToRemoveFrom;

        public RemoveObject_Popup(PathNode ControllingNode, List<GameObject> Connections, PlaceToRemoveFrom PlaceToRemoveFrom)
        {
            controllingNode = ControllingNode;
            connections = Connections;
            FalsifyChoices();
            placeToRemoveFrom = PlaceToRemoveFrom;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 60);
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Select Connetions To end them");
            if (connections != null && connections.Count > 0)
            {
                if (choices == null) FalsifyChoices();
                for (int i = 0; i < connections.Count; i++)
                {
                    choices[i] = GUILayout.Toggle(choices[i], connections[i].name);
                }
            }
            if (GUILayout.Button("Accept"))
            {
                Modify();
            }
        }

        private void FalsifyChoices()
        {
            choices = new List<bool> { };
            foreach (GameObject obj in connections) choices.Add(false);
        }

        private void Modify()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (choices[i])
                {
                    if (placeToRemoveFrom == PlaceToRemoveFrom.forced) controllingNode.forcedConnections.Remove(connections[i]);
                    else if (placeToRemoveFrom == PlaceToRemoveFrom.blocked) controllingNode.blockedConnections.Remove(connections[i]);
                }
            }
            editorWindow.Close();
        }

    }
}
#endif