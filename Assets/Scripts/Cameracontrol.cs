using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameracontrol : MonoBehaviour
{
    private Transform myTransform = null;
    //Ÿ������ ������ ������ �Ÿ�.
    public float distance = 5f;
    //Ÿ������ ������ ����
    public float height = 1.5f;
    //���̰� ���� �ӵ�
    public float heightDamping = 2.0f;
    //ȸ���� ���� �ӵ�
    public float rotationDamping = 3.0f;
    //Ÿ��.
    public Transform target = null;


    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        //Ź���� ���ٸ� Player��� �±׸� ������ �ִ°��ӿ�����Ʈ�� Ÿ���̴�.
        if(target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }
    }

    private void LateUpdate()
    {
        if(target ==null)
        {
            return;
        }
        //ī�޶� ��ǥ�� �ϰ� �ִ� ���� Y�ప�� ���̰�.
        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;
        //���� ī�޶� �ٶ󺸰� �ִ� ȸ�� Y���� ���̰�.
        float currentRotationangle = myTransform.eulerAngles.y;
        float currentHeight = myTransform.position.y;

        currentRotationangle = Mathf.LerpAngle(currentRotationangle,
            wantedRotationAngle, rotationDamping * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationangle, 0.0f);
        //ī�޶� Ÿ���� ��ġ���� ȸ���ϰ� �ϴ� ���͸�ŭ �ڷ� �̵��Ѵ�
        myTransform.position = target.position;
        myTransform.position -= currentRotation * Vector3.forward * distance;
        //�̵��� ��ġ���� ���ϴ� ���̰����� �ö󰣴�.
        myTransform.position = new Vector3(myTransform.position.x,
            currentHeight, myTransform.position.z);
        //Ÿ���� �׻� �ٶ󺸵��� �Ѵ�. forward -> target
        myTransform.LookAt(target);

    }

}
