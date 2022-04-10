using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //���� ������ Hand�� ���� 
    //HandŸ������ ����������, Hand.cs�� �ִ� �������� ����� �� �־��.
    [SerializeField]
    private Hand currentHand;

    //������??
    private bool isAttack = false;
    private bool isSwing = false;

    //�������� ���� �༮�� ����
    private RaycastHit hitInfo;

    private void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {//��Ŭ�� leftContol�� �Ǳ⶧����  Axes ���� ����
        if (Input.GetButton("Fire1"))
        {
            if (!isAttack)
            {//�ڷ�ƾ �����
                StartCoroutine(AttackCoutoutine());
            }
        }
    }

    IEnumerator AttackCoutoutine()
    {
        isAttack = true; //True�� �ߺ������� ����
        //�ִϸ��̼ǿ� �ִ� Attack�� �����Ų��.0
        currentHand.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        //���� Ȱ��ȭ ����.
        StartCoroutine(HitCoroutine()); //����ؼ� �ݺ��Ǵ� HitCoroutine

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    //���������� �˾ƺ��� �ڷ�ƾ
    IEnumerator HitCoroutine()
    {
        //isSwing�� false�� �ɶ����� �ݺ�
        while (!isSwing)
        {
            if (CheckObject())
            {
                isSwing = false;
                //�浹��
                Debug.Log(hitInfo.transform.name);
            }
            yield return null;
        }
    }

    private bool CheckObject()
    {//�浹ü�� �ִٸ� hitInfo���� �����´�.
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range))
        {
            return true;
        }
        return false;
    }
}
