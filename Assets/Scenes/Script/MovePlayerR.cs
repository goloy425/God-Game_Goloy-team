using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayerR : MonoBehaviour
{
    public float speed = 5.0f;          // �ړ����x
    public float rotationSpeed = 5.0f;  // ��]���x

    public Transform cameraTransform;   // �J������Transform
    private Rigidbody rb;               // Rigidbody
    private GameInputs inputs;          // GameInputs�N���X

    private Vector2 moveInputValue; // �X�e�B�b�N�̓��͂��󂯎��
    private Vector3 moveVector;     // �J������̈ړ�����

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbody�R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();

        // GameInputs�N���X�̃C���X�^���X���쐬
        inputs = new GameInputs();

        // Action�C�x���g�o�^
        inputs.PlayerR.Move.started += OnMove;
        inputs.PlayerR.Move.performed += OnMove;
        inputs.PlayerR.Move.canceled += OnMove;

        // InputAction���@�\�����邽�߂ɂ́A�L��������K�v������
        inputs.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // �J�����̌����ɍ��킹�ăX�e�B�b�N�̌X������ړ������ƈړ��x�����߂�
        moveVector = new Vector3(moveInputValue.x, 0.0f, moveInputValue.y);

        // �X�e�B�b�N���X���Ă��鎞
        if (moveVector.sqrMagnitude > 0.01f )
        {
            // �L�����N�^�[�̑O�������ړ������ɏ��X�Ɍ�����
            transform.forward = Vector3.Slerp(transform.forward, moveVector, rotationSpeed * Time.deltaTime);

        }

        // �I�u�W�F�N�g���ړ�
        rb.AddForce(moveVector * speed, ForceMode.Force);
        // �����������Ȃ��悤�ɂ���
        rb.velocity = Vector3.zero;
    }


    private void OnDestroy()
    {
        // ���g�ŃC���X�^���X������Action�N���X��IDisposable���������Ă���̂ŁA�K��Dispose����K�v������
        // inputs��null�łȂ����A�������
        inputs?.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Move�A�N�V�����̓��͎擾
        moveInputValue = context.ReadValue<Vector2>();
    }
}
