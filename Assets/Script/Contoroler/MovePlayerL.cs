//------------------------------------------------
// 本田洸都
//------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayerL : MonoBehaviour
{
    public float moveSpeed = 5.0f;      // 移動速度
    public float rotationSpeed = 5.0f;  // 回転速度

    [Header("移動の基準となるカメラ")]
    public Transform cameraTransform;   // カメラのTransform

    private Rigidbody rb;                       // Rigidbody
    private GameInputs inputs;                  // GameInputsクラス
    private PlaySEAtRegularIntervals playSE;    // PlaySEAtRegularIntervalsコンポーネント

    private Vector2 moveInputValue;     // スティックの入力を受け取る
    private Vector3 moveForward;        // カメラ基準の移動方向

    // Start is called before the first frame update
    void Start()
    {
        // Rigidbodyコンポーネントを取得
        rb = GetComponent<Rigidbody>();
        // PlaySEAtRegularIntervalsコンポーネントを取得
        playSE = GetComponent<PlaySEAtRegularIntervals>();
        // GameInputsクラスのインスタンスを作成
        inputs = new GameInputs();

        // Actionイベント登録
        inputs.PlayerL.Move.started += OnMove;
        inputs.PlayerL.Move.performed += OnMove;
        inputs.PlayerL.Move.canceled += OnMove;

        // InputActionを機能させるためには、有効化する必要がある
        inputs.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
        // カメラの向きに合わせてスティックの傾きから移動方向と移動度を求める
        moveForward = cameraForward * moveInputValue.y + cameraRight * moveInputValue.x;
        // 移動させる
        rb.velocity = moveForward * moveSpeed;

        // 移動方向がゼロベクトルでない時
        if (moveForward != Vector3.zero)
        {
            playSE.enabled = true;      // SE再生スクリプトを有効化
            // キャラクターの向きを徐々に移動方向に向ける
            transform.forward = Vector3.Slerp(transform.forward, moveForward, rotationSpeed * Time.deltaTime);
        }
        // 移動方向がゼロベクトルの時
        else
        {
            playSE.SetElapsedTime(0);   // SE再生スクリプトの経過時間をリセット
            playSE.SetPlayCnt(0);		// SE再生スクリプトの再生回数をリセット
            playSE.enabled = false;     // SE再生スクリプトを無効化
        }
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
