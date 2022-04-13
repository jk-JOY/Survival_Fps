using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    private const float WALKING_FIRE = 0.08f, STANDIND_FIRE = 0.04f, CROUCHING_FIRE = 0.02f, FINESIGHT_FIRE = 0.05f;

    [SerializeField]
    private Animator animator;

    //ũ�ν� ��� ������ ���� ���� ��Ȯ��
    private float gunAccuracy;

    //ũ�ν� ��� ��Ȱ��ȭ�� ���� �θ� ��ü
    [SerializeField]
    private GameObject go_CrossHairHUD;
    [SerializeField]
    private GunController theGunController;

    public void WalkingAnimation(bool _flag)
    {
        animator.SetBool("Walking", _flag);
    }
    public void RunningAnimation(bool _flag)
    {
        animator.SetBool("Running", _flag);
    }
    public void CrouchingAnimation(bool _flag)
    {
        animator.SetBool("Crouching", _flag);
    }
    public void FineSightAnimation(bool _flag)
    {
        animator.SetBool("FineSight", _flag);
    }




    public void FireAnimation()
    {
        if(animator.GetBool("Walking"))
        {
            animator.SetTrigger("Walk_Fire");
        }
        else if(animator.GetBool("Crouching"))
        {
            animator.SetTrigger("Crouch_Fire");
        }
        else
        {
            animator.SetTrigger("Idle_Fire");
        }
    }


    public float GetAccuracy()
    {
        if (animator.GetBool("Walking"))
        {
            gunAccuracy = 0.08f;
        }
        else if (animator.GetBool("Crouching"))
        {
            gunAccuracy = 0.02f;
        }
        else if(theGunController.GetFineSightMode())
        {
            gunAccuracy = 0.001f;
        }
        else
        {
            gunAccuracy = 0.04f;
        }

        return gunAccuracy;
    }
}