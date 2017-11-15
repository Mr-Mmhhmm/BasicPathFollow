
using UnityEngine;
using UnityEngine.UI;

public class ObservationController : MonoBehaviour
{
    private GameObject selected;
    private const float MaxRayDistance = float.PositiveInfinity;
    public Text output;

    private void Start()
    {
        output = GameObject.Find("HUD_INFO").GetComponent<Text>();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, LayerMasks.onlyCharacters))
        {
            selected = hitInfo.transform.gameObject;
        }

        if (selected)
        {
            if (selected.GetComponentInParent<HunterAI>())
            {
                HunterAI script = selected.GetComponentInParent<HunterAI>();
                output.text = selected.transform.parent.name + "\nCanSeeGoal:" + script.canSeeGoal + "\nState:" + script.myState.ToString();
            }
            else if (selected.GetComponentInParent<RunnerAI>())
            {
                RunnerAI script = selected.GetComponentInParent<RunnerAI>();
                output.text = selected.transform.parent.name + "\nGoal:" + (script.goal != null ? script.goal.name:"N/A") + "\nState:" + script.myState.ToString();
            }
            //else output.text = "N/A";
        }

        if (Input.GetMouseButton(0)) output.text = "N/A";
    }
}