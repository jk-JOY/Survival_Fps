using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun currentGun;
    private float currentFireRate;

    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        GunFireRateCalc();
        TryFire();

    }

    private void GunFireRateCalc()
    {//0���� Ŭ ��쿡�� time.deltaTime��ŭ ��´�.
        //�������Ӹ��� ������   1�ʿ� 1�� currentFireRate�� ���ҽ�Ų��.
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; //1 ���� ���� 60���� 1
        }

    }

    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0)
        {
            Fire();
        }
    }

    private void Fire() // �߻���
    {
        currentFireRate = currentGun.fireRate;
        Shoot();
    }

    private void Shoot() //�߻���
    {
        PlayerSE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();// �� ��ƼŬ
        Debug.Log("�߻� �Ϸ�");
    }
    

    private void PlayerSE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}

