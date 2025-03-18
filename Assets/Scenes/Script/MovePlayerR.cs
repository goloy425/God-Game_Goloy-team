using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayerR : MonoBehaviour
{
    public float speed = 5.0f;          // 移動速度
    public float rotationSpeed = 5.0f;  // 回転速度

    public Transform cameraTransform;   // カメラのTransform
    private Rigidbody rb;               // Rigidbody
    private GameInputs inputs;          // GameInputsクラス

    private Vector2 moveInputValue; // スティックの入力を受け取る
    private Vector3 moveVector;     // カメラ基準の移動方向

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();

        // GameInputsクラスのインスタンスを作成
        inputs = new GameInputs();

        // Actionイベント登録
        inputs.PlayerR.Move.started += OnMove;
        inputs.PlayerR.Move.performed += OnMove;
        inputs.PlayerR.Move.canceled += OnMove;

        // InputActionを機能させるためには、有効化する必要がある
        inputs.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // カメラの向きに合わせてスティックの傾きから移動方向と移動度を求める
        moveVector = new Vector3(moveInputValue.x, 0.0f, moveInputValue.y);

        // スティックが傾いている時
        if (moveVector.sqrMagnitude > 0.01f )
        {
            // キャラクターの前向きを移動方向に徐々に向ける
            transform.forward = Vector3.Slerp(transform.forward, moveVector, rotationSpeed * Time.deltaTime);

        }

        // オブジェクトを移動
        rb.AddForce(moveVector * speed, ForceMode.Force);
        // 加速し続けないようにする
        rb.velocity = Vector3.zero;
    }


    private void OnDestroy()
    {
        // 自身でインスタンス化したActionクラスはIDisposableを実装しているので、必ずDisposeする必要がある
        // inputsがnullでない時、解放する
        inputs?.Dispose();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // Moveアクションの入力取得
        moveInputValue = context.ReadValue<Vector2>();
    }
}
