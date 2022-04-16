using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [SerializeField] float m_force = 0f; //흔들림 세기를 결정지을 변수
    [SerializeField] Vector3 m_offset = Vector3.zero; //흔들릴 방향을 결정지을 벡터

    Quaternion m_originBot; 

    private void Start()
    {
        m_originBot = transform.rotation;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(ShakeCoroutine());
        }
    }

    public IEnumerator ShakeCoroutine()
    {
        Vector3 t_originEuler = transform.eulerAngles;

        while(true)
        {
            float t_rotX = Random.Range(-m_offset.x, -m_offset.x);
            float t_rotY = Random.Range(-m_offset.y, -m_offset.y);
            float t_rotZ = Random.Range(-m_offset.z, -m_offset.z);

            Vector3 t_rangdomRot = t_originEuler + new Vector3(t_rotX, t_rotY, t_rotZ);
            Quaternion t_rot = Quaternion.Euler(t_rangdomRot);

            while (Quaternion.Angle(transform.rotation, t_rot) >0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_originBot, m_force * Time.deltaTime);
                yield return null;
            }
        }
    }
} 
