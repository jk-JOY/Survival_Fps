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
    {//0보다 클 경우에만 time.deltaTime만큼 깎는다.
        //매프레임마다 깎으며   1초에 1씩 currentFireRate을 감소시킨다.
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; //1 초의 역수 60분의 1
        }

    }

    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0)
        {
            Fire();
        }
    }

    private void Fire() // 발사전
    {
        currentFireRate = currentGun.fireRate;
        Shoot();
    }

    private void Shoot() //발사후
    {
        PlayerSE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();// 총 파티클
        Debug.Log("발사 완료");
    }
    

    private void PlayerSE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }
}

