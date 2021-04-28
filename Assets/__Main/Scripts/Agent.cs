using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public PathPoint NextPoint;
    public int AgentId
    {
        get; set;
    }
    public float Speed;

    void Update()
    {
        if (NextPoint == null)
        {
            return;
        }

        var dist = (transform.position - NextPoint.transform.position).magnitude;
        if (dist < 0.1f)
        {
            NextPoint = NextPoint.next;
        }

        transform.LookAt(NextPoint.transform);

        var pos = transform.position;
        pos += transform.forward * Speed * Time.deltaTime;
        transform.position = pos;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
