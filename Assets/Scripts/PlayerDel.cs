using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDel : MonoBehaviour
{
    private Rigidbody rigid;
    private float rotx;
    private float roty;

    public float rotateSpeed = 10f;
    float hAxis;
    float vAxis;
    float currentCameraRotationX;
    bool wDown;

    Vector3 moveVec;

    Animator anim;

    Camera mainCamera;

    public bool toggleCameraRotation;
    public float smoothness = 10f;

    private void RotatePlayer()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 rotateY = new Vector3(0f, _yRotation, 0f) * rotateSpeed;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(rotateY));
    }
    private void rotateCamera()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float rotateX = _xRotation * rotateSpeed;

        currentCameraRotationX -= rotateX;

        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -45, 45);
        //���Ʒ� �����ִ� �ִ� ����

    }

    void Update()
    {

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk"); // õõ�� �ȱ�

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * rotateSpeed * (wDown ? 0.5f : 1f) * Time.deltaTime; //(���� ? �� : ����)
        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
        //������ �ִϸ��̼�

        transform.LookAt(transform.position + moveVec);

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            toggleCameraRotation = true;
        }
        else
        {
            toggleCameraRotation = false;
        }
        //ĳ���� ������ ī�޶� �����̱�
    }

    private void LateUpdate()
    {
        if(toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(mainCamera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
        RotatePlayer();
        rotateCamera();

        mainCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        // �¿�� �����̸� �ȵǹǷ� 0f���� �༭ ������Ű��

    }

}
