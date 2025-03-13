using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayerR : MonoBehaviour
{
    public float speed = 5.0f;      // 移動速度

    private Rigidbody rb;           // Rigidbodyコンポーネント
    private GameInputs inputs;      // GameInputsクラス

    private Vector2 moveInputValue; // スティックの入力を受け取る
    private Vector3 moveVector;     // 移動度

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
        // ワールド座標基準でスティックの傾きから移動方向と移動度を求める
        moveVector = new Vector3(-moveInputValue.y, 0.0f, moveInputValue.x);

        // オブジェクトを移動
        rb.AddForce(moveVector * speed, ForceMode.Force);

        // 入力していない間は動かないようにする
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
