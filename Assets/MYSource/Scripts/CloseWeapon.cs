using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWeapon : MonoBehaviour
{
    public string handName; //��Ŭ or �Ǽ� ����
    public float range; // ���ݹ��� 
    public int damage; //���ݷ�
    public float workSpeed; // �۾��ӵ�

    public float attackDelay; // ���� ������
    public float attackDelayA; //���� Ȱ��ȭ ����
    public float attackDelayB;//���� ��Ȱ��ȭ ����

    public Animator anim; // �ִϸ��̼�
}
