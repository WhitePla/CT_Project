using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX;

    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRigid;
    
    Animator anim;
    bool jDown;
    bool isJump;
    bool isDodge;

    Vector3 _velocity;

    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;

        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }

    private void CharacterRotation()  // 좌우 캐릭터 회전
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY)); // 쿼터니언 * 쿼터니언
    }

    private void Move()
    {
        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");
        bool wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");


        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;
        transform.position += _velocity * walkSpeed * (wDown ? 0.5f : 1f) * Time.deltaTime; //(조건 ? 참 : 거짓)

        anim.SetBool("isRun", _velocity != Vector3.zero);
        anim.SetBool("isWalk", wDown);

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }

    void Jump()
    {
        if (jDown && _velocity == Vector3.zero && !isJump && !isDodge)
        {
            myRigid.AddForce(Vector3.up * 14f, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }
    void Dodge()
    {
        if (jDown && _velocity != Vector3.zero && !isJump)
        {
            walkSpeed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f); 
        }
    }

    void DodgeOut()
    {
        walkSpeed *= 0.5f;
        isDodge = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plane") 
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void Awake()
    {
        Cursor.visible = false; //마우스 커서를 보이지 않게
        Cursor.lockState = CursorLockMode.Locked; //마우스 커서 위치 고정
        myRigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        anim.SetBool("isWalk", false);
        myRigid = GetComponent<Rigidbody>();
    }

    void Update()  // 컴퓨터마다 다르지만 대략 1초에 60번 실행
    {
        Move();
        Jump();
        Dodge();
        CameraRotation();
        CharacterRotation();
        Awake();
    }
}
