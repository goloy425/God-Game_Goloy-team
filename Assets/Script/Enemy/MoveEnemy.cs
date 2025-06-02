using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 制作者：ゴロイ

public class MoveEnemy : MonoBehaviour
{
    public float stepSize = 1f;        // 1回の移動距離
    public float moveSpeed = 2f;       // 移動速度
    public bool isStopped = false;     // 移動停止フラグ

    public List<MoveDirection> moveOrder = new List<MoveDirection>(); // 移動順序

    private bool isMoving = false;

    // 各方向に対する Y 回転角（初期向き調整用）
    private Dictionary<Direction, float> rotationYMap = new Dictionary<Direction, float>
    {
        { Direction.Forward, 0f },
        { Direction.Backward, 180f },
        { Direction.Right, -90f },
        { Direction.Left, 90f }
    };

    // ワールド基準での移動方向ベクトル
    private Dictionary<Direction, Vector3> directionMap = new Dictionary<Direction, Vector3>
    {
        { Direction.Forward, Vector3.right },
        { Direction.Backward, Vector3.left },
        { Direction.Right, Vector3.forward },
        { Direction.Left, Vector3.back }
    };

    // ✅ Start をコルーチンに変更して最初の向きをちゃんと適用
    private IEnumerator Start()
    {
        // 最初の向きに一度だけ回転
        if (moveOrder.Count > 0)
        {
            float initialYRotation = rotationYMap[moveOrder[0].direction];
            transform.rotation = Quaternion.Euler(0, initialYRotation, 0);
        }

        // ❗ 1フレーム待ってから移動を開始（回転が反映されるのを待つ）
        yield return null;

        StartCoroutine(Move());
    }

    private void Update()
    {
        if (isStopped)
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator Move()
    {
        isMoving = true;

        while (!isStopped)
        {
            foreach (MoveDirection direction in moveOrder)
            {
                Vector3 moveVec = directionMap[direction.direction];
                float yRotation = rotationYMap[direction.direction];

                // 向きを更新（この方向に回転）
                transform.rotation = Quaternion.Euler(0, yRotation, 0);

                for (int i = 0; i < direction.steps; i++)
                {
                    if (isStopped) yield break;

                    Vector3 startPos = transform.position;
                    Vector3 targetPos = startPos + moveVec * stepSize;

                    float elapsedTime = 0f;

                    while (elapsedTime < 1f / moveSpeed)
                    {
                        transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime * moveSpeed);
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    transform.position = targetPos;
                }
            }
        }

        isMoving = false;
    }
}

// Inspector で設定する移動パターン
[System.Serializable]
public struct MoveDirection
{
    public Direction direction;
    public int steps;
}

// 移動方向の選択肢
public enum Direction
{
    Forward,
    Backward,
    Right,
    Left
}
