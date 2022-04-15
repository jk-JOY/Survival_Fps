using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    private Vector3 originPos; //������ġ
    private Vector3 currentPos;//���� ��ġ

    [SerializeField]
    private Vector3 limitPos; // Sway�� �Ѱ�

    [SerializeField]
    private Vector3 fineSightLimitPos;//������ Sway�Ѱ�

    [SerializeField]
    private Vector3 smoothSway; //�ε巯�� ����

    //�ʿ��� ������
    [SerializeField]
    private GunController theController;

    // Start is called before the first frame update
    void Start()
    {
        originPos = this.transform.localPosition;// ���� ������Ʈ�� ��ġ���� originPos
    }

    // Update is called once per frame
    void Update()
    {
        TrySway();
    }
    //���Ǻб�� �ΰ��� �Լ��� ���� ȣ���Ұ��̴�.



    /// <summary>��
    /// 1. ���콺�� �����δٸ�     Swaying() ���� inSwaying���� ���콺�� ���� _moveX,y�� ���Խ��Ѽ�
    /// 2. ������ currentPos�� �־��ִµ� math.clmp�� ȭ������� ������ ���ϵ��� �����ְ� , ��Lerp�� �ε巴�� ó�����־���. xyz
    /// 3. ��
    /// </summary>    
    public void TrySway()
    {
        if(Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            Swaying();
        }
        else
        {
            BackToOriginPos();
        }
    }
    private void Swaying()
    {//
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");
        
        if(theController.isFineSightMode)
        {
            //�ε巴�� �����̵��� ���α�
            //������ ���°� �ƴҶ��� ȭ�� �����ڸ����� ��鸰��.
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x),
                          Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -limitPos.y, limitPos.y),
                          originPos.z);
        }
        else
        {
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -fineSightLimitPos.x, fineSightLimitPos.x),
                        Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                        originPos.z);
        }
        transform.localPosition = currentPos;
    }

    private void BackToOriginPos()
    {//�ٽ� 
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}
