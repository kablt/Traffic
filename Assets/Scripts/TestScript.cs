using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;


public class TestScript : MonoBehaviour
{

    public TextMeshProUGUI textLabel;
    //sphrer
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //타겟없이는 동작하지 않습니다
        if(target == null)
        {
            return;
        }
        //cube transform
        Vector3 lhs = transform.forward;
        //target 으로 향하는 백터, 크기를 노말라이즈해서 방향만 얻습니다.
        Vector3 rhs = (target.position - transform.position).normalized;
        //내적을 구합니다 .최대1 최소 -1
        float dot = Mathf.Clamp(Vector3.Dot(lhs, rhs), -1, 1);
        //타켓 포지션으로 부터의 역백터를 구합닏다.
        Vector3 lineVector = transform.InverseTransformPoint(target.position);
        //레이를 그려봅니다. 타켓으로 향하는 레이, 큐브의 forward 를 나타내는 레이.
        Debug.DrawRay(transform.position,lineVector, Color.red);
        Debug.DrawRay(transform.position,transform.forward, Color.cyan);
        //텍스트로 내적의 값을 출력합니다.
        textLabel.text = dot.ToString("f1");
    }
}

