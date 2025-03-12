using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayerR : MonoBehaviour
{
    public float speed = 5.0f;      // �ړ����x

    private Rigidbody rb;           // Rigidbody�R���|�[�l���g
    private GameInputs inputs;      // GameInputs�N���X

    private Vector2 moveInputValue; // �X�e�B�b�N�̓��͂��󂯎��
    private Vector3 moveVector;     // �ړ��x

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
        // ���[���h���W��ŃX�e�B�b�N�̌X������ړ������ƈړ��x�����߂�
        moveVector = new Vector3(-moveInputValue.y, 0.0f, moveInputValue.x);

        // �I�u�W�F�N�g���ړ�
        rb.AddForce(moveVector * speed, ForceMode.Force);

        // ���͂��Ă��Ȃ��Ԃ͓����Ȃ��悤�ɂ���
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
