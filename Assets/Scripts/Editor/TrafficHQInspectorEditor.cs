using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class TrafficHQInspectorEditor 
{
    public static void Drawinspector(TrafficHeadquarter trafficheadquarter, 
        SerializedObject serializedObject, out bool restructureSystem)
    {
        //����� ����.
        InspectorHelper.Header("����� ����");
        InspectorHelper.Toggle("����� ������?", ref trafficheadquarter.hideGizmos);
        //ȭ��ǥ ����.
        InspectorHelper.DrawArrowTypeSelection(trafficheadquarter);
        InspectorHelper.FloatField("��������Ʈ ũ��.", ref trafficheadquarter.waypointSize);
        EditorGUILayout.Space();
        //�ý��� ����.
        InspectorHelper.Header("�ý��� ����.");
        InspectorHelper.FloatField("���� ���� �ּ� �Ÿ�", ref trafficheadquarter.segDetectThresh);
        InspectorHelper.PropertyField("�浹 ���̾��,", "collisionLayers", serializedObject);
        EditorGUILayout.Space();
        //����.
        InspectorHelper.HelpBox("Ctrl + ���콺 ���� Ű : ���׸�Ʈ ���� \n" +
                                    "Shift + ���콺 ���� : ��������Ʈ ���� \n" +
                                    "Alt + ���콺 ���� : ������ ����");
        InspectorHelper.HelpBox("������ �߰��Ѵ�� ��������Ʈ�� ���� �̵��ϰ� �˴ϴ�.");

        EditorGUILayout.Space();
        restructureSystem = InspectorHelper.Button("���� �ùķ��̼� �ý��� �籸��/");

    }

}
