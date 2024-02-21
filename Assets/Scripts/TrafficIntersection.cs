using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public enum IntersectionType
{
    NONE=0,
    STOP, //�켱 ���� ����
    TRAFFIC_LIGHT, // ��ȣ�� ������ ����
    TRAFFIC_SLOW, // ���� ����
    EMERTGENCY //��� ��Ȳ
}
public class TrafficIntersection : MonoBehaviour
{
    public IntersectionType intersectionType = IntersectionType.NONE;
    public int ID = 0;
    //�켱 ���� ������
    public List<TrafficSegment> prioritySegments = new List<TrafficSegment>();
    //��ȣ�� ������ �ʿ��� �Ӽ���
    public float lightDuration = 8f;
    private float lastChangeLightTime = 0f;
    private Coroutine lightRoutine;
    public float lightRepeatrate = 8f;
    public float orangeLightDuration = 2f;
    //���� �� ����
    public List<TrafficSegment> lightGroup1 = new List<TrafficSegment>();
    public List<TrafficSegment> lightGroup2 = new List<TrafficSegment>();

    private List<GameObject> vehiclesQueue = new List<GameObject>();
    private List<GameObject> vehiclesIntersection = new List<GameObject>();
    private TrafficHeadquarter trafficHeadquarter;
    //���� �߰��� �׷�.
    public int currentRedLightgroup = 1;

    //���� �� ���� �Դϱ�?
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
        //ť�� �ִ� ������ ��ȣ ������ �ƴ� �ڵ������� �̵���Ų��.
        List<GameObject> newVehicleQueue = new List<GameObject>(vehiclesQueue);
        foreach (var vehicle in newVehicleQueue)
        {
            VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
            int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();
            //���� ��ȣ�� ���� �ʴ� �����̶��.
            if(IsRedLightSegment(vehicleSegment)==false)
            {
                vehicleControl.vehicleStatus = VehicleControl.Status.GO;
                newVehicleQueue.Remove(vehicle);
            }
        }

        vehiclesQueue = newVehicleQueue;
    }
  //��ȣ �������ְ� ,����
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
        //�ٸ� ������ �����̰� �ϱ� ���� ��ȣ ��ȯ �� �� �ʵ��� ��ٸ��� ������(��Ȳ��)
        Invoke("MoveVehicleQueue", orangeLightDuration);
    }

    private void Start()
    {
        vehiclesQueue = new List<GameObject>();
        vehiclesIntersection = new List<GameObject>();
        lastChangeLightTime = Time.time;
    }

    //�ڷ�ƾ���� ��ȣ ���� ȣ��, ���� ����(LightRepeatRate, lightDuration)
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

    //�켱 ����?
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
    //�켱 ���� ���� Ʈ����.
    void TriggerStop(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        //��������Ʈ �Ӱ谪�� ���� �ڵ����� ��� ���� �Ǵ� �ٷ� ���� ������ ���� �� �ֽ��ϴ�
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();

        if(IsPrioritySegment(vehicleSegment)==false)
        {
            if(vehiclesQueue.Count > 0 || vehiclesIntersection.Count >0)
            {
                vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
            }
            //�����ο� ���� ���ٸ�
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
    //��ȣ ������ Ʈ����, ������ ���߰ų� �̵���Ű�ų�.
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
    //��� ��Ȳ �߻� Ʈ����
    void TriggerEmergency(GameObject vehicle)
    {
        VehicleControl vehicleControl = vehicle.GetComponent<VehicleControl>();
        int vehicleSegment = vehicleControl.GetSegmentVehicleIsIn();

        vehicleControl.vehicleStatus = VehicleControl.Status.STOP;
        vehiclesQueue.Add(vehicle);
    }

    //���������ٸ�, ��޻�Ȳ�� �����Ǿ��� ���
    private void ExitEmergency(GameObject vehicle)
    {
        vehicle.GetComponent<VehicleControl>().vehicleStatus = VehicleControl.Status.GO;
    }

    //Ʈ���� �߻��� ó��.
    private void OnTriggerEnter(Collider other)
    {
        //������ �̹� ��Ͽ� �ִ��� Ȯ���ϰ� ,�׷��ٸ� ó�� ����
        //��� ������ ���̶�� ó�� ����(�ƿ� ���۽� �����ο� ������ �ִ� ���)
       if(IsAlreadyInIntersection(other.gameObject) || Time.timeSinceLevelLoad < 0.5f)
        {
            return;
        }
       //������ �ƴϸ� ����
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
