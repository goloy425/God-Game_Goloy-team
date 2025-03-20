using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 制作者　ゴロイヒデキ

public class GameManager : MonoBehaviour
{
    public PressurePlates01[] pressurePlates; // すべての感圧板を登録
    private int totalPressed = 0; // 押されている感圧板の数

    public string resultSceneName = "Result"; // 遷移先のシーン名をInspectorで設定

    // Start is called before the first frame update
    void Start()
    {
        foreach (PressurePlates01 plate in pressurePlates)
        {
            plate.OnPressurePlateChanged += OnPlateStateChanged; // イベント登録
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnPlateStateChanged(bool isPressed)
    {
        totalPressed += isPressed ? 1 : -1; // 押されている数を増減

        if (totalPressed == pressurePlates.Length) // すべてが押された場合
        {
            Debug.Log("全ての感圧板が押されています！ゲームクリア！Result画面に移ります");
            // ここにゲームクリア処理を書く
            SceneManager.LoadScene(resultSceneName);
        }
    }
}
