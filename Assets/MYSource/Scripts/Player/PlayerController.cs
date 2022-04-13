using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //스피드 조정 변수
    [SerializeField]
    private float WalkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    //상태변수
    private bool isRun = false;
    private bool isGrounded = true;  //땅에서 시작이니 true로 시작 땅이 아니라면 점프 하지 못하게 막는다..!
    private bool isCrouch = true;

    //앉았을때 얼마나 앉을지 결정하는 변수
    [SerializeField]
    private float crouchPosY;
    private float originPosY; // 원래 상태의 값
    private float applyCrouchPosY; //crouchPosY,   originPosY 각각의 값을 넣을 변수


    //땅 착지 여부
    private CapsuleCollider capsuleCollider;


    // 카메라의 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라의 한계
    [SerializeField]
    private float cameraRotationLimit; // 어디까지만 볼수 있는지의 카메라의 리미트
    private float currentCameraRotaionX = 0;

    //필요한 컴포넌트
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

        //초기화
        originPosY = theCamera.transform.localPosition.y; // 왜 로컬 포지션이냐? 카메라 월드 상대적 기준이라서 local이다. 
        applyCrouchPosY = originPosY;  //기본 서있는 상태도록. 

    }

    void Update()
    {
        IsGrounded();
        TryJump(); // Jump 포함
        TryRun();
        TryCrouch(); //Crouch 까지 포함
        Move();
        CameraRotation();
        CharacterRotation();
    }

    //앉기시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }
    }

    //앉기동작- C를 누른 상황에서만 작동되는 함수
    private void Crouch()
    {
        isCrouch = !isCrouch;   //isCrouch가 트루일때 false로 바꿔주고.  false면 true로 바꿔주고...!
      
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
        //v값은 
        StartCoroutine("CrouchCoroutine");
    }

    //병렬처리하는 코루틴(부드러운 앉기 동작)
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;

        int count = 0;


        while(_posY != applyCrouchPosY)  //1프레임마다 이 과정을 실행시켜주고ㅡ posY가 목적지까지 가게 된다면 while을 만족시켜 빠져나온다..
        {
            count++;
            //보간함수
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

    //지면 체크
    private void IsGrounded()
    { //고정된 좌표에 다가 광선을 쏠꺼야. 
        //캡슐콜라이더의 바운드에 extents의 half에 y값만큼 레이저를 주고 쏠꺼야. 
        isGrounded = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }

    //점프시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    //점프
    private void Jump()
    {
        //앉은키로 점프한다면 , 앉은상태 초기화
        if(isCrouch)
        {
            Crouch();
        }
        
        //내 리지드가 움직이고 있는 속도에  순간적으로 위로 옮긴다 = 점프
        myRigid.velocity = transform.up * jumpForce;

    }

    //달리기 시도
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

    //달리기 실행
    private void Running()
    {
        //앉은 상태에서 달릴때 앉은상태 초기화
        if (isCrouch)
        {
            Crouch();
            theGunController.CancleFineSight();
        }

        isRun = true;
        applySpeed = runSpeed; // runspeed로 대입해서 
    }

    //달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = WalkSpeed; // 다시 walkspeed로 변경해준다.
    }

    //움직임 실행
    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal"); // -1 0 1
        float _moveDirZ = Input.GetAxisRaw("Vertical"); // 3D이기 때문에 Z

        //상하좌우를 구분할 수 있다.
        Vector3 _moveHorizontal = transform.right * _moveDirX;// (1, 0, 0 ) * 1    
        Vector3 _moveVertical = transform.forward * _moveDirZ;// ( 0,0,1 ) * 1      

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed; // 삼각함수 x축 0.5  y축 0.5방향으로 walkspeed 만큼
                                                                                       // 곱해서 얼마나빠르게 갈건지 결정한다.
                                                                                       //(1 ,0, 0) + (0,0,1)의 합이 
                                                                                       //(1, 0, 1) = 2 가되고     (0.5 , 0 , 0.5) = 1    (normalized)를 하면 0.5가 된다.  가급적 1로 나오도록 (로우레벨에서 가장 빠름.)

        //강체에 movePosition함수를 사용해서 해당obj에 접근해 transfrom.posion에 velocity를 더하고 시간과 비슷하도록 time.deltatime으로 딜레이?를 넣는다. 
        //강체에 movePosition함수를 사용해서 해당obj에 접근해 transfrom.posion에 velocity를 더하고 시간과 비슷하도록 time.deltatime으로 딜레이?를 넣는다. 
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    //좌우 캐릭터 회전
    private void CharacterRotation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity; //상하좌우 민감도를 통일한다. 
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
        //우리가 구한 벡터값을 쿼터니언으로 변환시키는것
        //(우리가 구한 벡터값) = 오일러값(_characterRotationY)을 쿼터니언으로 바꿔서 (Quaternion.)
        // (myRigid.rotation) 이 쿼터니언 로테이션값과 곱함  회전. 


    }

    //상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = _xRotation * lookSensitivity; // 민감도 
        currentCameraRotaionX -= cameraRotationX;  //마우스 Y반전

        //currentCameraRotaionX의 값이 -45도와 +45도사이에 가둠 
        currentCameraRotaionX = Mathf.Clamp(currentCameraRotaionX, -cameraRotationLimit, cameraRotationLimit);
        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotaionX, 0f, 0f);
    }

}
