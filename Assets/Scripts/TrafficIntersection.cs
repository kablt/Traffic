using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public enum IntersectionType
{
    NONE=0,
    STOP, //우선 멈춤 구간
    TRAFFIC_LIGHT, // 신호등 교차로 구간
    TRAFFIC_SLOW, // 감속 구간
    EMERTGENCY //긴급 상황
}
public class TrafficIntersection : MonoBehaviour
{
    public IntersectionType intersectionType = IntersectionType.NONE;
    public int ID = 0;
    //우선 멈춤 구간들
    public List<TrafficSegment> prioritySegments = new List<TrafficSegment>();
    //신호등 구간에 필요한 속성들
    public float lightDuration = 8f;
    private float lastChangeLightTime = 0f;
    private Coroutine lightRoutine;
    public float lightRepeatrate = 8f;
    public float orangeLightDuration = 2f;
    //빨간 불 구간
    public List<TrafficSegment> lightGroup1 = new List<TrafficSegment>();
    public List<TrafficSegment> lightGroup2 = new List<TrafficSegment>();

    private List<GameObject> vehiclesQueue = new List<GameObject>();
    private List<GameObject> vehiclesIntersection = new List<GameObject>();
    private TrafficHeadquarter trafficHeadquarter;
    //현재 발간불 그룹.
    public int currentRedLightgroup = 1;

    //빨간 불 구간 입니까?
    bool IsRedLightSegment(int vehicleSegment)
    {
        if(currentRedLightgroup == 1)
        {
            foreach(var segment in lightGroup1)
            {
                if(segment.ID == vehicleSegment)
                {
                    return true;
                }
            }
        }
        else if(currentRedLightgroup ==2)
        {
            foreach(var segment in lightGroup2)
            {
                if(segment.ID == vehicleSegment)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void MoveVehicleQueue()
    {
        //큐에 있는 빨간불 신호 구간이 아닌 자동차들을 이동시킨다.
        List<GameObject> newVehicleQueue = new List<GameObject>(vehiclesQueue);
        foreach (var vehicle in newVehicleQueue)
        {
            VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
            int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();
            //빨간 신호를 받지 않는 차량이라면.
            if(IsRedLightSegment(vehicleSegment)==false)
            {
                vehicleControl.vehicleStatus = VehicleControl.Status.GO;
                newVehicleQueue.Remove(vehicle);
            }
        }

        vehiclesQueue = newVehicleQueue;
    }
  //신호 변경해주고 ,차량
    void SwitchLights()
    {
        if(currentRedLightgroup ==1)
        {
            currentRedLightgroup = 2;
        }
        else if(currentRedLightgroup ==2)
        {
            currentRedLightgroup = 1;
        }
        else
        {
            currentRedLightgroup = 1;
        }
        //다른 차량의 움직이게 하기 전에 신호 전환 후 몇 초동안 기다리게 해주자(주황불)
        Invoke("MoveVehicleQueue", orangeLightDuration);
    }

    private void Start()
    {
        vehiclesQueue = new List<GameObject>();
        vehiclesIntersection = new List<GameObject>();
        lastChangeLightTime = Time.time;
    }

    //코루틴으로 신호 변경 호출, 일정 간격(LightRepeatRate, lightDuration)
    private IEnumerator OnTrafficLight()
    {
        SwitchLights();
        yield return new WaitForSeconds(lightRepeatrate);
    }

    private void Update()
    {
        switch (intersectionType)
        {
            case IntersectionType.TRAFFIC_LIGHT:
                if(Time.time > lastChangeLightTime + lightDuration)
                {
                    lastChangeLightTime = Time.time;
                    lightRoutine = StartCoroutine("OnTrafficLight");
                }
                break;
            case IntersectionType.EMERTGENCY:
                if(lightRoutine != null)
                {
                    StopCoroutine(lightRoutine);
                    currentRedLightgroup = 0;
                }
                break;
            case IntersectionType.STOP:
                break;
        }
    }

    bool IsAlreadyInIntersection(GameObject target)
    {
        foreach(var vehicle in vehiclesIntersection)
        {
            if(vehicle.GetInstanceID() == target.GetInstanceID())
            {
                return true;
            }
        }

        foreach(var vehicle in vehiclesQueue)
        {
            if(vehicle.GetInstanceID() == target.GetInstanceID())
            {
                return true;
            }
        }


        return false;
    }

    //우선 구간?
    bool IsPrioritySegment(int vehicleSegment)
    {
        foreach(var segment in prioritySegments)
        {
            if(vehicleSegment == segment.ID)
            {
                return true;
            }
        }
        return false;
    }
    //우선 멈춘 구간 트리거.
    void TriggerStop(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        //웨이포인트 임계값에 따라 자동차는 대상 구간 또는 바로 직전 구간에 있을 수 있습니다
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();

        if(IsPrioritySegment(vehicleSegment)==false)
        {
            if(vehiclesQueue.Count > 0 || vehiclesIntersection.Count >0)
            {
                vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
            }
            //교차로에 차가 없다면
            else
            {
                vehiclesIntersection.Add(vehicle);
                vehicleControl.vehicleStatus = VehicleControl.Status.SLOW_DOWN;
            }
        }
        else
        {
            vehicleControl.vehicleStatus = VehicleControl.Status.SLOW_DOWN;
            vehiclesIntersection.Add(vehicle);
        }
    }

    void ExitStop(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
        vehiclesIntersection.Remove(vehicle);
        vehiclesQueue.Remove(vehicle);

        if(vehiclesQueue.Count > 0 && vehiclesIntersection.Count ==0)
        {
            vehiclesQueue[0].GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
        }
    }
    //신호 교차로 트리거, 차량을 멈추거나 이동시키거나.
    void TriggerLight(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();

        if (IsRedLightSegment(vehicleSegment))
        {
            vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
            vehiclesQueue.Add(vehicle);
        }
        else
        {
            vehicleControl.vehicleStatus = VehicleControl.Status.GO;
        }
    }

    void ExitLight(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
    }
    //긴급 상황 발생 트리ㅓ
    void TriggerEmergency(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();

        vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
        vehiclesQueue.Add(vehicle);
    }

    //빠져나갔다면, 긴급상황이 헤제되었을 경우
    private void ExitEmergency(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
    }

    //트리거 발생시 처리.
    private void OnTriggerEnter(Collider other)
    {
        //차량이 이미 목록에 있는지 확인하고 ,그렇다면 처리 안함
        //방금 시작한 앱이라면 처리 안함(아예 시작시 교차로에 차량이 있는 경우)
       if(IsAlreadyInIntersection(other.gameObject) || Time.timeSinceLevelLoad < 0.5f)
        {
            return;
        }
       //차량이 아니면 무시
       if(other.tag.Equals(TrafficHeadquarter.VehicleTagLayer) ==false)
        {
            return;
        }

       switch (intersectionType)
        {
            case IntersectionType.STOP:
                TriggerStop(other.gameObject);
                break;
            case IntersectionType.TRAFFIC_LIGHT:
                TriggerLight(other.gameObject);
                break;
            case IntersectionType.EMERTGENCY:
                TriggerEmergency(other.gameObject);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals(TrafficHeadquarter.VehicleTagLayer) == false)
        {
            return;
        }

        switch(intersectionType)
        {
            case IntersectionType.STOP:
                ExitStop(other.gameObject);
                break;
            case IntersectionType.TRAFFIC_LIGHT:
                ExitLight(other.gameObject);
                break;
            case IntersectionType.EMERTGENCY:
                ExitEmergency(other.gameObject);
                break;
        }
    }
}
