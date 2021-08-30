using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AStarAgent : MonoBehaviour
{
    //This is a demo/assignment of a pathfinding algorithm, so lets just make this a singleton for simplicitys sake
    static AStarAgent singleton;
    LineRenderer lineRenderer;
    public static AStarAgent Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindObjectOfType<AStarAgent>();
            }
            return singleton;
        }
    }
    CharacterController controller;
    public List<Vector3> transformTargets = new List<Vector3>();
    TargetController currentTarget;
    float agentSpeed = 3f;
    Vector3 moveDir;
    public void MoveToTargets(List<Vector2> targets)
    {
        transformTargets.Clear();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = targets.Count - 1;
        for (int i = 0; i < targets.Count -1; i++)
        {
            Vector3 pos = new Vector3();
            if (i == 0)
            {
                pos = transform.position;
            }
            else
            {
                if (i < targets.Count - 1)
                {
                    pos = new Vector3(targets[i + 1].x,0.5f, targets[i+1].y);
                }
            }
            lineRenderer.SetPosition(i, pos);
            transformTargets.Add(new Vector3(targets[i].x, transform.position.y, targets[i].y));
        }
        AddCurrentTarget();

        
    }
    public void changeSpeed(Slider slider)
    {
        agentSpeed = slider.value;
    }

    private void AddCurrentTarget()
    {
        if (transformTargets.Count > 0)
        {
            Vector3 newTarget = transformTargets[0];
            if (newTarget != null)
            {
                if (currentTarget == null) currentTarget = new TargetController();
                currentTarget.Target = newTarget;
                moveDir = currentTarget.Target - transform.position;
            }
        }
        else
        {
            currentTarget = null;
            moveDir = Vector3.zero;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget != null)
        {

            controller.Move(moveDir*Time.deltaTime * agentSpeed);
            if (Vector3.Distance(transform.position, currentTarget.Target) < 0.2f)
            {

                transformTargets.RemoveAt(0);


                AddCurrentTarget();
            }
        }
    }
    public Vector2 getAgentLocation()
    {
        return new Vector2((int)(Math.Round(transform.position.x)), (int)(Math.Round(transform.position.z)));
    }
}
//Lets wrap vector3 under target class to allow nulling the target (vec3 is a struct)
class TargetController
{
    private Vector3 target;

    public Vector3 Target { get => target; set => target = value; }
}
