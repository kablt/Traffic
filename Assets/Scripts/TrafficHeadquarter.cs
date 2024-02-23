using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

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
    
    //data Loading -> 속성들 정의.
    public class EmergencyData
    {
        public int ID = -1;
        public bool IsEmergency = false;
        public EmergencyData(string id, string emergency)
        {
            ID = int.Parse(id);
            IsEmergency = emergency.Contains("1");
        }
    }

    public class TrafficData
    {
        public List<EmergencyData> datas = new List<EmergencyData>();
    }

    //data출혁한 UI 라벨
    public TMPro.TextMeshProUGUI stateLabel;
    //구슬 스프레드 시트 읽어올 로더
    public SpreadSheetLoader dataLoader;
    //읽어온 데이터 클래스
    private TrafficData trafficData;

    private void Start()
    {
        dataLoader = GetComponentInChildren<SpreadSheetLoader>();
        stateLabel = GameObject.FindWithTag("TrafficLabel").GetComponent<TMPro.TextMeshProUGUI>();
        //일정 주기로 데이터 로딩을 시킬껀데, 주의 너무 자주 빈번하게 부르면 URL 막힙니다
    }

    private void CallLoaderAndCheck()
    {
        string loadedData = dataLoader.StartLoader();
        stateLabel.text = "Traffic Status\n" + loadedData;
        if (string.IsNullOrEmpty(loadedData))
        {
            return;
        }
        //data -> class 담을게요
        trafficData = new TrafficData();
        string[] AllRow = loadedData.Split('\n');
        foreach(string oneRow in AllRow)
        {
            string[] datas = oneRow.Split("\t");//탭으로 구분됩니다
            EmergencyData data = new EmergencyData(datas[0], datas[1]);
            trafficData.datas.Add(data);
        }
    }

}
