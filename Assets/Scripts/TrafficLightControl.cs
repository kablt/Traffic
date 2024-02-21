using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TrafficLightControl : MonoBehaviour
{

    public int LightGroupID;
    public TrafficIntersection intersection;
    //라이트..
    private Light pointLight;
    //긴급 상황시 깜빡임 정도.
    private float blink = 0f;
    //교통신호에 따라 색깔 변경.
    void SetTrafficLightColor()
    {
        if(intersection.currentRedLightgroup == LightGroupID)
        {
            pointLight.color = Color.red;
        }
        //긴급상황일떄
        else if (intersection.currentRedLightgroup == 0)
        {
            blink = Mathf.Clamp01(blink + Time.deltaTime * 2f);
            pointLight.color = new Color(blink, 0f, 0f);
            if(blink >=1f)
            {
                blink = 0f;
            }
        }
        else
        {
            pointLight.color = Color.green;
        }
    }

    private void Start()
    {
        pointLight = GetComponentInChildren<Light>();
        SetTrafficLightColor();
    }

    private void Update()
    {
        SetTrafficLightColor();
    }
  
}
