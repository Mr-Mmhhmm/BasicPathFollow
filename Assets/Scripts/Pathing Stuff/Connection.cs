
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Pathing
{
    public class Connection : ScriptableObject
    {
        public GameObject target;
        public float width;
        private float[] WidthsToTest = new float[] { 3, 5, 10 }; // Red, Blue, Green, White
        public enum ConnectionType { Null, Level, Drop, Jump }
        public ConnectionType connectionType;
        private const float heightVarience = 1;
        private const float depthRequiredToBeConsideredAJump = 10;
        private const float maxDrop = 5;
        public Color color
        {
            get
            {
                Color myColor = Color.red;
                if (width == 3) myColor = Color.blue;
                else if (width == 5) myColor = Color.green;
                else if (width >= 10) myColor = Color.white;
                return myColor;
            }
        }

        private void Init(Vector3 MyPosition, GameObject Target, float MaxJumpDistance, float MaxDropHeight)
        {
            target = Target;
            width = GetWidth(MyPosition);
            DetermineConnectionType(MyPosition, MaxJumpDistance, MaxDropHeight);
#if UNITY_EDITOR
            Undo.RegisterCompleteObjectUndo(this, "Connection Made");
#endif
        }

        public static Connection CreateInstance(Vector3 MyPosition, GameObject Target, float MaxJumpDistance, float MaxDropHeight)
        {
            Connection newConnection = CreateInstance<Connection>();
            newConnection.Init(MyPosition, Target, MaxJumpDistance, MaxDropHeight);
            return newConnection;
        }

        private float GetWidth(Vector3 myPosition)
        {
            float minWidth = 0;
            Ray ray = new Ray(myPosition, target.transform.position - myPosition);
            foreach (float width in WidthsToTest)
            {
                if (Physics.OverlapSphere(myPosition, width / 2, LayerMasks.onlyWalls, QueryTriggerInteraction.Ignore).Length == 0 && !Physics.SphereCast(ray, width / 2, Vector3.Distance(myPosition, target.transform.position), LayerMasks.onlyWalls, QueryTriggerInteraction.Ignore))
                {
                    minWidth = width;
                }
            }
            return minWidth;
        }

        private void DetermineConnectionType(Vector3 MyPosition, float MaxJumpDistance, float MaxDropHeight)
        {
            //Debug.Log(Vector3.Distance(MyPosition, target.transform.position));
            if (!Physics.Raycast(new Vector3((MyPosition.x + target.transform.position.x) / 2, (MyPosition.y + target.transform.position.y) / 2 + heightVarience, (MyPosition.z + target.transform.position.z) / 2), Vector3.down, depthRequiredToBeConsideredAJump + heightVarience) // Is there a gap?
             && Vector3.Distance(MyPosition, target.transform.position) <= MaxJumpDistance) // Can I Jump it?
            {
                connectionType = ConnectionType.Jump;
            }
            else if (Mathf.Abs(MyPosition.y - target.transform.position.y) < heightVarience) // Is it level?
            {
                connectionType = ConnectionType.Level;
            }
            else if (MyPosition.y - target.transform.position.y > heightVarience && MyPosition.y - target.transform.position.y < MaxDropHeight) // Is it a drop that i can take?
            {
                connectionType = ConnectionType.Drop;
            }
            else
            {
                connectionType = ConnectionType.Null;
            }
        }

#if UNITY_EDITOR
        public void Draw(PathNode Me, float ConnectionArrowPercent)
        {
            if (!Me.isDoorClosed && !target.GetComponent<PathNode>().isDoorClosed) // Door is open
            {
                if (Selection.Contains(Me.gameObject))
                {
                    Handles.color = color;
                    Handles.Label(Me.transform.position + (target.transform.position - Me.transform.position) * (ConnectionArrowPercent / 100), connectionType.ToString());
                    Handles.Label(new Vector3(target.transform.position.x, target.transform.position.y + 1, target.transform.position.z), "Target");
                }
                DrawArrow(Me.transform.position, target.transform.position - Me.transform.position, color, ConnectionArrowPercent);
            }
        }
#endif

        private void DrawArrow(Vector3 pos, Vector3 direction, Color color, float ConnectionArrowPercent, float arrowHeadLength = 2f, float arrowHeadAngle = 20.0f)
        {
            Color startColor = Gizmos.color; // Get the starting color so we can set it back later
            Gizmos.color = color; // Set the color to our arrow color

            pos = pos + direction * (ConnectionArrowPercent / 100);

            Gizmos.DrawRay(pos, -direction / 5);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos, right * arrowHeadLength * (direction.magnitude / 15));
            Gizmos.DrawRay(pos, left * arrowHeadLength * (direction.magnitude / 15));

            Gizmos.color = startColor; // reset the arrow color to the start
        }

        public void RefreshConnectionInfo(Vector3 MyPosition, float MaxJumpDistance, float MaxDropHeight)
        {
            width = GetWidth(MyPosition);
            DetermineConnectionType(MyPosition, MaxJumpDistance, MaxDropHeight);
        }
    }
}
