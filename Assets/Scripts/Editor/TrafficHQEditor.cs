using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TrafficHeadquarter))]
public class TrafficHQEditor : Editor
{
    private TrafficHeadquarter headquarter;
    //��������Ʈ ��ġ�Ҷ� �ʿ��� �ӽ� ����ҵ�.
    private Vector3 startPosition;
    private Vector3 lastPoint;
    private TrafficWaypoint lastWaypoint;
    //traffic �ùķ������� ����̵Ǵ� ��ũ��Ʈ�� ����.
    [MenuItem("Component/TrafficTool/Create Traffic System")]
    private static void CreateTrafficSystem()
    {
        EditorHelper.SetUndoGroup("Create Traffic System");

        GameObject headquarterObject = EditorHelper.CreateGameObject("Traffic Headquarter");
        EditorHelper.AddComponent<TrafficHeadquarter>(headquarterObject);

        GameObject segmentsObject = EditorHelper.CreateGameObject("Segments",
            headquarterObject.transform);
        GameObject intersectionsObject = EditorHelper.CreateGameObject("Intersections",
            headquarterObject.transform);

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
    }
    private void OnEnable()
    {
        headquarter = target as TrafficHeadquarter;
    }

    //��������Ʈ �߰�.
    private void AddWaypoint(Vector3 position)
    {
        //��������Ʈ ���ӿ�����Ʈ�� ���� ����.
        GameObject go = EditorHelper.CreateGameObject(
            "Waypoint-" + headquarter.curSegment.Waypoints.Count,
            headquarter.curSegment.transform);
        //��ġ�� ���� Ŭ���� ������ �մϴ�.
        go.transform.position = position;
        TrafficWaypoint waypoint = EditorHelper.AddComponent<TrafficWaypoint>(go);
        waypoint.Refresh(headquarter.curSegment.Waypoints.Count,
            headquarter.curSegment);
        Undo.RecordObject(headquarter.curSegment, "");
        //HQ�� ������ ��������Ʈ�� ���� �۾����� ���׸�Ʈ�� �߰��մϴ�.
        headquarter.curSegment.Waypoints.Add(waypoint);
    }

    //���׸�Ʈ �߰�.
    private void AddSegment(Vector3 position)
    {
        int segID = headquarter.segments.Count;
        //Segments��� ���� �� ���ӿ�����Ʈ�� ���ϵ�� ���׸�Ʈ ���ӿ�����Ʈ�� �����մϴ�.
        GameObject segGameObject = EditorHelper.CreateGameObject(
            "Segment-" + segID, headquarter.transform.GetChild(0).transform);
        //���� ���� Ŭ���� ��ġ�� ���׸�Ʈ�� �̵���ŵ�ϴ�.
        segGameObject.transform.position = position;
        //HQ�� ���� �۾����� ���׸�Ʈ�� ���� ���� ���׸�Ʈ ��ũ��Ʈ�� �������ݴϴ�.
        //���Ŀ� �߰��Ǵ� ��������Ʈ�� ���� �۾����� ���׸�Ʈ�� �߰��ǰ� �˴ϴ�.
        headquarter.curSegment = EditorHelper.AddComponent<TrafficSegment>(segGameObject);
        headquarter.curSegment.ID = segID;
        headquarter.curSegment.Waypoints = new List<TrafficWaypoint>();
        headquarter.curSegment.nextSegments = new List<TrafficSegment>();

        Undo.RecordObject(headquarter, "");
        headquarter.segments.Add(headquarter.curSegment);
    }

    //���ͼ��� �߰�.
    private void AddIntersection(Vector3 position)
    {
        int intID = headquarter.intersections.Count;
        //���ο� �����α����� ���� Intersections ���ӿ�����Ʈ ���ϵ�� �ٿ��ݴϴ�.
        GameObject intersection = EditorHelper.CreateGameObject(
            "Intersection-" + intID, headquarter.transform.GetChild(1).transform);
        intersection.transform.position = position;

        BoxCollider boxCollider = EditorHelper.AddComponent<BoxCollider>(intersection);
        boxCollider.isTrigger = true;
        TrafficIntersection trafficIntersection = EditorHelper.AddComponent<TrafficIntersection>(
            intersection);
        trafficIntersection.ID = intID;

        Undo.RecordObject(headquarter, "");
        headquarter.intersections.Add(trafficIntersection);
    }
    //������ ���� ��ġ�غ����� �ϰڽ��ϴ�.
    //Shift 
    //Control
    //Alt
    private void OnSceneGUI()
    {
        //���콺 Ŭ�� ������ �־����� ���ɴϴ�.
        Event @event = Event.current;
        if (@event == null)
        {
            return;
        }
        //���콺������ ��ġ�� ���̸� ������ݴϴ�.
        Ray ray = HandleUtility.GUIPointToWorldRay(@event.mousePosition);
        RaycastHit hit;
        //���콺 ��ġ�� �浹ü ������ �Ǿ���, ���콺 ���� Ŭ������ ���� �߻��Ͽ���.
        //0 ���� Ŭ�� 1 ������ Ŭ�� 2�� �ٹ�ư
        if (Physics.Raycast(ray, out hit) &&
            @event.type == EventType.MouseDown &&
            @event.button == 0)
        {
            //���콺 ���� Ŭ�� + Shift -> ��������Ʈ �߰�.
            if (@event.shift)
            {
                //�������� ��������Ʈ�� ������ �� �����ϴ�.
                if (headquarter.curSegment == null)
                {
                    Debug.LogWarning("���׸�Ʈ ���� ������ּ���.");
                    return;
                }
                EditorHelper.BeginUndoGroup("Add WayPoint", headquarter);
                AddWaypoint(hit.point);
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            }
            //���콺 ���� Ŭ�� + Control -> ���׸�Ʈ �߰�.
            //ù��° ��������Ʈ�� ���� �߰��˴ϴ�.
            else if (@event.control)
            {
                EditorHelper.BeginUndoGroup("Add Segment", headquarter);
                AddSegment(hit.point);
                AddWaypoint(hit.point);
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            }
            //���콺 ���� Ŭ�� + Alt -> ���ͼ��� �߰�.
            else if (@event.alt)
            {
                EditorHelper.BeginUndoGroup("Add Intersection", headquarter);
                AddIntersection(hit.point);
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            }
        }
        //��������Ʈ �ý����� �����Ű�信�� ������ ���� ��ü�� ����.
        Selection.activeGameObject = headquarter.gameObject;
        //������ ��������Ʈ�� ó���մϴ�.
        if (lastWaypoint != null)
        {
            //���̰� �浹�� �� �ֵ��� Plane�� ����մϴ�.
            Plane plane = new Plane(Vector3.up, lastWaypoint.GetVisualPos());
            plane.Raycast(ray, out float dst);
            Vector3 hitPoint = ray.GetPoint(dst);
            //���콺 ��ư�� ó�� ��������, LastPoint �缳��.
            if (@event.type == EventType.MouseDown && @event.button == 0)
            {
                lastPoint = hitPoint;
                startPosition = lastWaypoint.transform.position;
            }
            //������ ��������Ʈ�� �̵�.
            if (@event.type == EventType.MouseDrag && @event.button == 0)
            {
                Vector3 realPos = new Vector3(
                    hitPoint.x - lastPoint.x, 0, hitPoint.z - lastPoint.z);
                lastWaypoint.transform.position += realPos;
                lastPoint = hitPoint;
            }
            //������ ��������Ʈ�� ����.
            if (@event.type == EventType.MouseUp && @event.button == 0)
            {
                Vector3 curPos = lastWaypoint.transform.position;
                lastWaypoint.transform.position = startPosition;
                Undo.RegisterFullObjectHierarchyUndo(lastWaypoint, "Move WayPoint");
                lastWaypoint.transform.position = curPos;
            }
            //�� �ϳ� �׸��ڽ��ϴ�.
            Handles.SphereHandleCap(0, lastWaypoint.GetVisualPos(), Quaternion.identity,
                headquarter.waypointSize * 2f, EventType.Repaint);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            SceneView.RepaintAll();
        }
        //��� ��������Ʈ�κ��� �Ǻ����� ���� �浹�Ǵ� ��������Ʈ�� lastWaypoint�� ����.
        if (lastWaypoint == null)
        {
            //��� ��������Ʈ���� ���� ���� ���콺 ��ġ���� ������ ���̰� �浹�Ǵ��� Ȯ��.
            lastWaypoint = headquarter.GetAllWaypoints()
                .FirstOrDefault(
                    i => EditorHelper.SphereHit(i.GetVisualPos(),
                        headquarter.waypointSize, ray));
        }
        //HQ�� ���� �������� ���׸�Ʈ�� ���� ������ ���׸�Ʈ�� ��ü�մϴ�.
        if (lastWaypoint != null && @event.type == EventType.MouseDown)
        {
            headquarter.curSegment = lastWaypoint.segment;
        }
        //���� ��������Ʈ�� �缳��.���콺�� �̵��ϸ� ������ Ǯ������.
        else if (lastWaypoint != null && @event.type == EventType.MouseMove)
        {
            lastWaypoint = null;
        }

    }
    //�ùķ����� ���� �������� �缳���� �ؼ� ������ ���� �� ���ư����� ���ݴϴ�.
    void RestructureSystem()
    {
        //������ ��������Ʈ�� �̸� �ٲٱ�, ������ ����
        List<TrafficSegment> segmentsList = new List<TrafficSegment>();
        int itSeg = 0;
        foreach (Transform trans in headquarter.transform.GetChild(0).transform)
        {
            TrafficSegment segment = trans.GetComponent<TrafficSegment>();
            if (segment != null)
            {
                List<TrafficWaypoint> waypointList = new List<TrafficWaypoint>();
                segment.ID = itSeg;
                segment.gameObject.name = "Segment-" + itSeg;

                int itWay = 0;
                foreach (Transform trans2 in segment.transform)
                {
                    TrafficWaypoint waypoint = trans2.GetComponent<TrafficWaypoint>();
                    if (waypoint != null)
                    {
                        waypoint.Refresh(itWay, segment);
                        waypointList.Add(waypoint);
                        itWay++;
                    }
                }

                segment.Waypoints = waypointList;
                segmentsList.Add(segment);
                itSeg++;
            }
        }
        // Next ���׸�Ʈ�� ���������� ����Ʈ���� ������ �� ���ش�.
        foreach (TrafficSegment segment in segmentsList)
        {
            List<TrafficSegment> nextSegmentsList = new List<TrafficSegment>();
            foreach (TrafficSegment nextSegment in segment.nextSegments)
            {
                if (nextSegment != null)
                {
                    nextSegmentsList.Add(nextSegment);
                }
            }

            segment.nextSegments = nextSegmentsList;
        }
        //���θ��� ���׸�Ʈ ����Ʈ�� �ٽ� HQ�� �Ҵ��մϴ�.
        headquarter.segments = segmentsList;
        //�����ε� ����������.
        List<TrafficIntersection> intersectionList = new List<TrafficIntersection>();
        int itInter = 0;
        foreach (Transform transInter in headquarter.transform.GetChild(1).transform)
        {
            TrafficIntersection intersection = transInter.GetComponent<TrafficIntersection>();
            if (intersection != null)
            {
                intersection.ID = itInter;
                intersection.gameObject.name = "Intersection-" + itInter;
                intersectionList.Add(intersection);
                itInter++;
            }
        }

        headquarter.intersections = intersectionList;
        //����� �� ������ ���� ���� �׸��� ������ �� ����� ������.
        //�ٲ�� �ֵ��� ����Ƽ���� �˷��ݴϴ�. ������ ����Ȱ� ��� �ѹ� �׳� ���Ž�����.
        if (!EditorUtility.IsDirty(target))
        {
            EditorUtility.SetDirty(target);
        }
        Debug.Log("[���� �ùķ��̼�] ���������� ������Ͽ����ϴ�.");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        {
            Undo.RecordObject(headquarter, "Traffic Inspector Edit");
            TrafficHQInspectorEditor.DrawInspector(headquarter, serializedObject,
                out bool restructureSystem);
            if (restructureSystem)
            {
                RestructureSystem();
            }
        }
        //���� ������ ��� ���� �ƿ� �ٽ� �ѹ� �׷�����.
        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }

        serializedObject.ApplyModifiedProperties();
    }
}