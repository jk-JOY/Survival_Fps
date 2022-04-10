using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //현재 장착된 Hand형 무기 
    //Hand타입으로 선언했을때, Hand.cs에 있는 변수들을 사용할 수 있어요.
    [SerializeField]
    private Hand currentHand;

    //공격중??
    private bool isAttack = false;
    private bool isSwing = false;

    //레이저에 닿은 녀석의 정보
    private RaycastHit hitInfo;

    private void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {//좌클릭 leftContol도 되기때문에  Axes 에서 제거
        if (Input.GetButton("Fire1"))
        {
            if (!isAttack)
            {//코루틴 실행됨
                StartCoroutine(AttackCoutoutine());
            }
        }
    }

    IEnumerator AttackCoutoutine()
    {
        isAttack = true; //True로 중복실행을 막고
        //애니메이션에 있는 Attack을 실행시킨다.0
        currentHand.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        //공격 활성화 시점.
        StartCoroutine(HitCoroutine()); //계속해서 반복되는 HitCoroutine

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    //공격적중을 알아보는 코루틴
    IEnumerator HitCoroutine()
    {
        //isSwing이 false가 될때까지 반복
        while (!isSwing)
        {
            if (CheckObject())
            {
                isSwing = false;
                //충돌됨
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    private bool CheckObject()
    {//충돌체가 있다면 hitInfo에서 가져온다.
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true;
        }
        return false;
    }
}
