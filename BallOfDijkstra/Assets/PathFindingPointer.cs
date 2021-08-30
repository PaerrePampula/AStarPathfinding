using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PathFindingPointer : MonoBehaviour
{
    [SerializeField] GraphicRaycaster graphicRaycaster;
    [SerializeField] EventSystem eventSystem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown((KeyCode.Mouse0)))
        {

            Vector3 worldPosition = new Vector3();
            Plane plane = new Plane(Vector3.up, 0);

            float distance;
            //Its really easy to move the agent when trying to adjust the speed slider, so lets first of all make sure that there is no UI elements in front of the raycasting
            PointerEventData pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);
            if (results.Count == 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out distance))
                {
                    worldPosition = ray.GetPoint(distance);
                }
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePosOn2D = new Vector2(Mathf.Round(Mathf.Abs(worldPosition.x)), Mathf.Round(Mathf.Abs(worldPosition.z)));
                PathFinding.Singleton.PathFindTo(mousePosOn2D);
            }

        }
    }
}
