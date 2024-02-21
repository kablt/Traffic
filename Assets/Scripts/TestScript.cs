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
        //Ÿ�پ��̴� �������� �ʽ��ϴ�
        if(target == null)
        {
            return;
        }
        //cube transform
        Vector3 lhs = transform.forward;
        //target ���� ���ϴ� ����, ũ�⸦ �븻�������ؼ� ���⸸ ����ϴ�.
        Vector3 rhs = (target.position - transform.position).normalized;
        //������ ���մϴ� .�ִ�1 �ּ� -1
        float dot = Mathf.Clamp(Vector3.Dot(lhs, rhs), -1, 1);
        //Ÿ�� ���������� ������ �����͸� ���Ո���.
        Vector3 lineVector = transform.InverseTransformPoint(target.position);
        //���̸� �׷����ϴ�. Ÿ������ ���ϴ� ����, ť���� forward �� ��Ÿ���� ����.
        Debug.DrawRay(transform.position,lineVector, Color.red);
        Debug.DrawRay(transform.position,transform.forward, Color.cyan);
        //�ؽ�Ʈ�� ������ ���� ����մϴ�.
        textLabel.text = dot.ToString("f1");
    }
}

