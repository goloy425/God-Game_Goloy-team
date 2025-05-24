using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    public float stepSize = 1f;   // 一歩の距離
    public float moveSpeed = 2f;  // 移動速度
    public bool isStopped = false; // 停止フラグ

    // **Inspectorで設定できる移動順**
    public List<MoveDirection> moveOrder = new List<MoveDirection>();

    private bool isMoving = false;

    void Start()
    {
        StartCoroutine(Move());
    }

    void Update()
    {
        if (isStopped)
        {
            StopAllCoroutines();
        }
    }

    IEnumerator Move()
    {
        isMoving = true;

        while (!isStopped)
        {
            foreach (MoveDirection direction in moveOrder) // 設定した順で移動
            {
                int steps = direction.steps;
                Vector3 targetDirection = GetDirectionVector(direction.direction);

                for (int j = 0; j < steps; j++)
                {
                    if (isStopped) yield break;

                    Vector3 startPos = transform.position;
                    Vector3 targetPos = startPos + targetDirection * stepSize;

                    float elapsedTime = 0f;
                    while (elapsedTime < 1f / moveSpeed)
                    {
                        transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime * moveSpeed);
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    transform.position = targetPos;
                }

                transform.rotation = Quaternion.LookRotation(targetDirection);
            }
        }

        isMoving = false;
    }

    // Inspector で設定した移動方向をベクトルに変換
    private Vector3 GetDirectionVector(Direction dir)
    {
        switch (dir)
        {
            case Direction.Forward: return Vector3.right; // 前移動
            case Direction.Backward: return Vector3.left; // 後ろ移動
            case Direction.Right: return Vector3.forward; // 右移動
            case Direction.Left: return Vector3.back;     // 左移動
            default: return Vector3.forward;
        }
    }
}

// Inspectorで移動順を設定できるための構造体
[System.Serializable]
public struct MoveDirection
{
    public Direction direction;
    public int steps;
}

// 移動可能な方向の選択肢
public enum Direction
{
    Forward, Backward, Right, Left
}
