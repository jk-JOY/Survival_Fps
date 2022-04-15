using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseWeapon : MonoBehaviour
{
    public string closeWeaponName; //너클 or 맨손 구분 //근접 무기 이름

    //웨폰 유형
    public bool isHand; //손
    public bool isAxe; //도끼
    public bool isPickAxe; // 곡괭이

    public float range; // 공격범휘 
    public int damage; //공격력
    public float workSpeed; // 작업속도

    public float attackDelay; // 공격 딜레이
    public float attackDelayA; //공격 활성화 시점
    public float attackDelayB;//공격 비활성화 시점

    public Animator anim; // 애니메이션


}
