
using UnityEngine;

public class Door : MonoBehaviour
{

    private PathNode controllingNode;
    private Vector3 closedPosition;
    public Vector3 distanceToOpen;
    private const float DOOR_SPEED = 0.5f;

    // Use this for initialization
    void Start()
    {
        closedPosition = transform.position;
        controllingNode = transform.parent.GetComponentInChildren<PathNode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controllingNode)
        {
            if (controllingNode.isDoorClosed) transform.position = Vector3.MoveTowards(transform.position, closedPosition, DOOR_SPEED);
            else transform.position = Vector3.MoveTowards(transform.position, closedPosition + distanceToOpen, DOOR_SPEED);
        }
    }
#if UNITY_EDITOR
    [ContextMenu("Toggle Door", false, 860)]
    public void ToggleDoor()
    {
        controllingNode.ToggleDoor();
    }
#endif
}
