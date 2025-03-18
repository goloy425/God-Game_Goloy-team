using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayerL : MonoBehaviour
{
    public float speed = 5.0f;          // �ړ����x
    public float rotationSpeed = 5.0f;  // ��]���x

    public Transform cameraTransform;   // �J������Transform
    private Rigidbody rb;               // Rigidbody
    private GameInputs inputs;          // GameInputs�N���X

    private Vector2 moveInputValue; // �X�e�B�b�N�̓��͂��󂯎��
    private Vector3 moveForward;     // �J������̈ړ�����

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbody�R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();

        // GameInputs�N���X�̃C���X�^���X���쐬
        inputs = new GameInputs();

        // Action�C�x���g�o�^
        inputs.PlayerL.Move.started += OnMove;
        inputs.PlayerL.Move.performed += OnMove;
        inputs.PlayerL.Move.canceled += OnMove;

        // InputAction���@�\�����邽�߂ɂ́A�L��������K�v������
        inputs.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // �J�����̕�������AX-Z���ʂ̒P�ʃx�N�g�����擾
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        // �J�����̌����ɍ��킹�ăX�e�B�b�N�̌X������ړ������ƈړ��x�����߂�
        moveForward = cameraForward * moveInputValue.y + cameraTransform.right * moveInputValue.x;
        // �ړ�������
        rb.AddForce(moveForward * speed, ForceMode.Force);
        // �����������Ȃ��悤�ɂ���
        rb.velocity = Vector3.zero;

        // �ړ��������[���x�N�g���łȂ���
        if (moveForward != Vector3.zero)
        {
            // �L�����N�^�[�̌������ړ������ɏ��X�Ɍ�����
            transform.forward = Vector3.Slerp(transform.forward, moveForward, rotationSpeed * Time.deltaTime);

        }
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
