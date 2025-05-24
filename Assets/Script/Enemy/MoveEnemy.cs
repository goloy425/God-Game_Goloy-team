using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 制作者　ゴロイ

// ※敵が最初に動く方向をrotationのｙを変えて正面にしてください。
//
// Forward(前) = 180 , Backward(後ろ) = 0 , Left(左) = -90 , right(右) = 90


public class MoveEnemy : MonoBehaviour
{
    // 敵の移動設定
    public float stepSize = 1f;   // 1回の移動距離
    public float moveSpeed = 2f;  // 移動速度（数値が大きいほど速く移動）
    public bool isStopped = false; // true の場合、敵の移動を停止

    // Inspector で設定可能な移動順
    public List<MoveDirection> moveOrder = new List<MoveDirection>();

    private bool isMoving = false;
    private Quaternion initialRotation; // **オブジェクトの初期回転を保持（正面の基準を決定）**

    void Start()
    {
        // 初期回転を取得（Y軸の向きを基準として保持）
        initialRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);

        // 敵の移動を開始
        StartCoroutine(Move());
    }

    void Update()
    {
        // 移動停止フラグが立っている場合、コルーチンを停止
        if (isStopped)
        {
            StopAllCoroutines();
        }
    }

    // 移動方向に対する回転情報の辞書
    private Dictionary<Direction, Quaternion> rotationMap = new Dictionary<Direction, Quaternion>
    {
        { Direction.Forward, Quaternion.Euler(0, 180, 0) },  // 前向き
        { Direction.Backward, Quaternion.Euler(0, 0, 0) },   // 後ろ向き
        { Direction.Right, Quaternion.Euler(0, 90, 0) },     // 右向き
        { Direction.Left, Quaternion.Euler(0, -90, 0) }      // 左向き
    };

    // 移動方向に対するベクトル情報の辞書
    private Dictionary<Direction, Vector3> directionMap = new Dictionary<Direction, Vector3>
    {
        { Direction.Forward, Vector3.right },  // 前 → 右移動
        { Direction.Backward, Vector3.left },  // 後ろ → 左移動
        { Direction.Right, Vector3.forward },  // 右 → 前移動
        { Direction.Left, Vector3.back }       // 左 → 後ろ移動
    };

    IEnumerator Move()
    {
        isMoving = true;

        // 停止フラグが立つまでループ
        while (!isStopped)
        {
            foreach (MoveDirection direction in moveOrder) // 設定された順に移動
            {
                int steps = direction.steps;

                // 移動方向を辞書から取得
                Vector3 targetDirection = directionMap[direction.direction];

                for (int j = 0; j < steps; j++)
                {
                    // もし途中で停止フラグが立ったら、処理を終了
                    if (isStopped) yield break;

                    // 現在の位置を取得
                    Vector3 startPos = transform.position;

                    // 次の移動位置を計算（現在位置 + 移動ベクトル）
                    Vector3 targetPos = startPos + targetDirection * stepSize;

                    float elapsedTime = 0f;

                    // 滑らかに移動する処理
                    while (elapsedTime < 1f / moveSpeed)
                    {
                        transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime * moveSpeed);
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    // 最終的な位置を確定
                    transform.position = targetPos;
                }

                // オブジェクトの向きを変更（初期回転を基準）
                transform.rotation = initialRotation * rotationMap[direction.direction];
            }
        }

        isMoving = false;
    }
}

// 移動順の設定を管理する構造体
[System.Serializable]
public struct MoveDirection
{
    public Direction direction; // 進む方向
    public int steps; // その方向へ進む歩数
}

// 移動可能な方向の選択肢
public enum Direction
{
    Forward,  // 前
    Backward, // 後ろ
    Right,    // 右
    Left      // 左
}