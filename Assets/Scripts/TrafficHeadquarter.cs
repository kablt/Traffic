using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficHeadquarter : MonoBehaviour
{
    //세그먼트와 세그먼트사이의 검출 간격.
    public float segDetectThresh = 0.1f;
    //웨이포인트의 크기.
    public float waypointSize = 0.5f;
    //충돌 레이어들.
    public string[] collisionLayers;

    public List<TrafficSegment> segments = new List<TrafficSegment>();
    public TrafficSegment curSegment;

    public const string VehicleTagLayer = "AutonomousVehicle";//무인자동차.
    //교차로들.
    public List<TrafficIntersection> intersections = new List<TrafficIntersection>();
    
    //에디터용, 기즈모 속성들. 본부에서 조절하겠습니다.
    public enum ArrowDraw
    {
        FixedCount,
        ByLength,
        Off
    }

    public bool hideGizmos = false;
    public ArrowDraw arrowDrawType = ArrowDraw.ByLength;
    public int arrowCount = 1;
    public float arrowDistance = 5f;
    public float arrowSizeWaypoint = 1;
    public float arrowSizeIntersection = 0.5f;
    
    
    
    

    public List<TrafficWaypoint> GetAllWaypoints()
    {
        List<TrafficWaypoint> waypoints = new List<TrafficWaypoint>();
        foreach (var segment in segments)
        {
            waypoints.AddRange(segment.Waypoints);
        }

        return waypoints;
    }
    
    
}
