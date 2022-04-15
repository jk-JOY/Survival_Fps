using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    private Vector3 originPos; //기존위치
    private Vector3 currentPos;//현재 위치

    [SerializeField]
    private Vector3 limitPos; // Sway의 한계

    [SerializeField]
    private Vector3 fineSightLimitPos;//정조준 Sway한계

    [SerializeField]
    private Vector3 smoothSway; //부드러운 정도

    //필요한 컴포넌
    [SerializeField]
    private GunController theController;

    // Start is called before the first frame update
    void Start()
    {
        originPos = this.transform.localPosition;// 현재 오브젝트위 위치값을 originPos
    }

    // Update is called once per frame
    void Update()
    {
        TrySway();
    }
    //조건분기로 두가지 함수를 따로 호출할것이다.



    /// <summary>ㅑ
    /// 1. 마우스를 움직인다면     Swaying() 실행 inSwaying에는 마우스에 각각 _moveX,y에 대입시켜서
    /// 2. 현재의 currentPos에 넣어주는데 math.clmp로 화면밖으로 나가지 못하도록 막아주고 , ㅡLerp로 부드럽게 처리해주었다. xyz
    /// 3. 실
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
            //부드럽게 움직이도록 가두기
            //정조준 상태가 아닐때는 화면 가장자리까지 흔들린다.
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
    {//다시 
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}
