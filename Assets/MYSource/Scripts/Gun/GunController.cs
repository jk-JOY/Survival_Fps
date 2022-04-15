using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // Ȱ��ȭ ����.
    public static bool isActivate = true;

    //���� ������ ��
    [SerializeField]
    private Gun currentGun;
    private PlayerController playerController;
    
    //���� �ӵ� ���
    private float currentFireRate;

    //���º���
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false; // true�� ���� �������ϵ��� �����ϴ� bool��

    //���� ������ ��
    private Vector3 originPos;


    //ȿ���� ���
    private AudioSource audioSource;

    //�浹�� ��ü�� ������ �޾ƿ�
    private RaycastHit hitInfo;


    //�ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCam;
    private CrossHair theCrossHair;

    //�ǰ�����Ʈ
    public GameObject hit_effect_prefab;

    private void Start()
    {
        originPos = Vector3.zero;
        audioSource = GetComponent<AudioSource>();
        theCrossHair = FindObjectOfType<CrossHair>();

        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;
    }

    private void Update()
    {
        if (isActivate)
        {
            GunFireRateCalc();
            TryFire();
            TryReLoad();
            TryFineSight();
        }
    }

    //����ӵ� ����
    private void GunFireRateCalc()
    {//0���� Ŭ ��쿡�� time.deltaTime��ŭ ��´�.
        //�������Ӹ��� ������   1�ʿ� 1�� currentFireRate�� ���ҽ�Ų��.
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; //1 ���� ���� 60���� 1
        }
    }

    //�߻�õ�
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }

    //�߻� �� ���
    private void Fire()
    {
        if (!isReload)
        {

            if (currentGun.currentBulletCount > 0)
            {
                Shoot();
            }
            else
            {
                CancleFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    //�߻� �� ���
    private void Shoot()
    {
        theCrossHair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //���� �ӵ� ����
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();// �� ��ƼŬ
        Hit();
        //�ѱ� �ݵ� �ڷ�ƾ ����
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    private void Hit()
    {//x ��, y ���� �� random���� 
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward +
            new Vector3(Random.Range(-theCrossHair.GetAccuracy() - currentGun.accuracy, theCrossHair.GetAccuracy() + currentGun.accuracy),
                        Random.Range(-theCrossHair.GetAccuracy() - currentGun.accuracy, theCrossHair.GetAccuracy() + currentGun.accuracy),
                        0)
            , out hitInfo, currentGun.range))
        {//��ü�� �ٶ󺸴� 
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }

    //������ �õ�
    private void TryReLoad()
    {//10�� ������ ������ 10�� �̸��϶��� ���� �����ϵ���
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancleFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void CancleReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }

    //������ �ڷ�ƾ
    IEnumerator ReloadCoroutine()
    {//�����ذ� , ������ ��Ұ� �ƿ� �ȵǵ���

        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");


            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            isReload = false;

        }
        else
        {
            Debug.Log("������ �Ѿ��� �����ϴ�.");
            //ĢĢ �Ҹ� (�Ѿ��� ������ �Ҹ�)
        }
    }

    //������ �õ�
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }

    }

    //������ ���
    public void CancleFineSight()
    {
        if (isFineSightMode)
        {
            FineSight();
        }
    }

    //������ ���� ����
    private void FineSight()
    {
        isFineSightMode = !isFineSightMode; //  isFineSightMode = true;  �� �� �Q���� ���̴�. �ڵ�ȭ ����       }

        currentGun.anim.SetBool("FindSightMode", isFineSightMode);

        theCrossHair.FineSightAnimation(isFineSightMode);

        if (isFineSightMode)
        {
            StopAllCoroutines();

            StartCoroutine(FindSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FindSightDeactivateCoroutine());
        }

    }


    //������ Ȱ��ȭ
    IEnumerator FindSightActivateCoroutine()
    {   //�������Ҷ��� ��ġ�� �ɋ����� �ݺ�
        while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    //������ ��Ȱ��ȭ // ������ ����ġ. 
    IEnumerator FindSightDeactivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    //�ݵ� �ڷ�ƾ
    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);

        if (!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos;

            //�ݵ�����
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            //����ġ
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }

        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            //�ݵ�����
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            //����ġ
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }

    }

    //���� ���
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    public void GunChange(Gun _gun)
    {
        if (WeaponManager.currentWeapon != null)
            WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;

    }
}
