using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //���ǵ� ���� ����
    [SerializeField]
    private float WalkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    //���º���
    private bool isRun = false;
    private bool isGrounded = true;  //������ �����̴� true�� ���� ���� �ƴ϶�� ���� ���� ���ϰ� ���´�..!
    private bool isCrouch = true;

    //�ɾ����� �󸶳� ������ �����ϴ� ����
    [SerializeField]
    private float crouchPosY;
    private float originPosY; // ���� ������ ��
    private float applyCrouchPosY; //crouchPosY,   originPosY ������ ���� ���� ����


    //�� ���� ����
    private CapsuleCollider capsuleCollider;


    // ī�޶��� �ΰ���
    [SerializeField]
    private float lookSensitivity;

    // ī�޶��� �Ѱ�
    [SerializeField]
    private float cameraRotationLimit; // �������� ���� �ִ����� ī�޶��� ����Ʈ
    private float currentCameraRotaionX = 0;

    //�ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    private GunController theGunController;


    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRigid = GetComponent<Rigidbody>();
        applySpeed = WalkSpeed;
        theGunController = FindObjectOfType<GunController>();

        //�ʱ�ȭ
        originPosY = theCamera.transform.localPosition.y; // �� ���� �������̳�? ī�޶� ���� ����� �����̶� local�̴�. 
        applyCrouchPosY = originPosY;  //�⺻ ���ִ� ���µ���. 

    }

    void Update()
    {
        IsGrounded();
        TryJump(); // Jump ����
        TryRun();
        TryCrouch(); //Crouch ���� ����
        Move();
        CameraRotation();
        CharacterRotation();
    }

    //�ɱ�õ�
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }
    }

    //�ɱ⵿��- C�� ���� ��Ȳ������ �۵��Ǵ� �Լ�
    private void Crouch()
    {
        isCrouch = !isCrouch;   //isCrouch�� Ʈ���϶� false�� �ٲ��ְ�.  false�� true�� �ٲ��ְ�...!
      
        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = WalkSpeed;
            applyCrouchPosY = originPosY;
        }
        //v���� 
        StartCoroutine("CrouchCoroutine");
    }

    //����ó���ϴ� �ڷ�ƾ(�ε巯�� �ɱ� ����)
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;

        int count = 0;


        while(_posY != applyCrouchPosY)  //1�����Ӹ��� �� ������ ��������ְ�� posY�� ���������� ���� �ȴٸ� while�� �������� �������´�..
        {
            count++;
            //�����Լ�
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.1f);
            theCamera.transform.localPosition = new Vector3(0,_posY, 0);
            if(count>15)
            {
                break;
            }
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0f, applyCrouchPosY, 0f);
    }

    //���� üũ
    private void IsGrounded()
    { //������ ��ǥ�� �ٰ� ������ �򲨾�. 
        //ĸ���ݶ��̴��� �ٿ�忡 extents�� half�� y����ŭ �������� �ְ� �򲨾�. 
        isGrounded = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    //�����õ�
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    //����
    private void Jump()
    {
        //����Ű�� �����Ѵٸ� , �������� �ʱ�ȭ
        if(isCrouch)
        {
            Crouch();
        }
        
        //�� �����尡 �����̰� �ִ� �ӵ���  ���������� ���� �ű�� = ����
        myRigid.velocity = transform.up * jumpForce;

    }

    //�޸��� �õ�
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Running();
            theGunController.CancleFineSight();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    //�޸��� ����
    private void Running()
    {
        //���� ���¿��� �޸��� �������� �ʱ�ȭ
        if (isCrouch)
        {
            Crouch();
            theGunController.CancleFineSight();
        }

        isRun = true;
        applySpeed = runSpeed; // runspeed�� �����ؼ� 
    }

    //�޸��� ���
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = WalkSpeed; // �ٽ� walkspeed�� �������ش�.
    }

    //������ ����
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // -1 0 1
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // 3D�̱� ������ Z

        //�����¿츦 ������ �� �ִ�.
        Vector3 _moveHorizontal = transform.right * _moveDirX;// (1, 0, 0 ) * 1    
        Vector3 _moveVertical = transform.forward * _moveDirZ;// ( 0,0,1 ) * 1      

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // �ﰢ�Լ� x�� 0.5  y�� 0.5�������� walkspeed ��ŭ
                                                                                       // ���ؼ� �󸶳������� ������ �����Ѵ�.
                                                                                       //(1 ,0, 0) + (0,0,1)�� ���� 
                                                                                       //(1, 0, 1) = 2 ���ǰ�     (0.5 , 0 , 0.5) = 1    (normalized)�� �ϸ� 0.5�� �ȴ�.  ������ 1�� �������� (�ο췹������ ���� ����.)

        //��ü�� movePosition�Լ��� ����ؼ� �ش�obj�� ������ transfrom.posion�� velocity�� ���ϰ� �ð��� ����ϵ��� time.deltatime���� ������?�� �ִ´�. 
        //��ü�� movePosition�Լ��� ����ؼ� �ش�obj�� ������ transfrom.posion�� velocity�� ���ϰ� �ð��� ����ϵ��� time.deltatime���� ������?�� �ִ´�. 
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    //�¿� ĳ���� ȸ��
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity; //�����¿� �ΰ����� �����Ѵ�. 
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        //�츮�� ���� ���Ͱ��� ���ʹϾ����� ��ȯ��Ű�°�
        //(�츮�� ���� ���Ͱ�) = ���Ϸ���(_characterRotationY)�� ���ʹϾ����� �ٲ㼭 (Quaternion.)
        // (myRigid.rotation) �� ���ʹϾ� �����̼ǰ��� ����  ȸ��. 


    }

    //���� ī�޶� ȸ��
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = _xRotation * lookSensitivity; // �ΰ��� 
        currentCameraRotaionX -= cameraRotationX;  //���콺 Y����

        //currentCameraRotaionX�� ���� -45���� +45�����̿� ���� 
        currentCameraRotaionX = Mathf.Clamp(currentCameraRotaionX, -cameraRotationLimit, cameraRotationLimit);
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotaionX, 0f, 0f);
    }

}
