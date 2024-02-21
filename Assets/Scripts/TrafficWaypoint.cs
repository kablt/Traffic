using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficWaypoint : MonoBehaviour
{
    public TrafficSegment segment;
    public void RemoveCollider()
    {

        if(GetComponent<SphereCollider>())
        {

            Debug.Log("Remove Collider");
            DestroyImmediate(gameObject.GetComponent<SphereCollider>());
        }

    }

    public void Refresh(int newID, TrafficSegment newSegment)
    {
        segment = newSegment;
        //Waypoint -1 , WayPoint-10
        name = "Waypoint-"+ newID.ToString();
        tag = "WayPoint";
        gameObject.layer = LayerMask.NameToLayer("Deafault");
        RemoveCollider();
    }

    public Vector3 GetVisualPos()
    {
        return transform.position + new Vector3(0.0f, 0.5f, 0.0f);
    }

}
