using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PoseManager_KeyMou : MonoBehaviour
{
    [Header("ボタン Resume→Retry→ReturnTitleの順番で設定")]
    public Button[] buttons;

    private PoseUISwitch pUIswitch;

    public int buttonIdx = 0;   // どのコマンドを選択しているか
    private int prevIdx = 0;    // UI切り替え用

    // Start is called before the first frame update
    public void Start()
    {
        GameObject.Find("Pose").TryGetComponent<PoseUISwitch>(out pUIswitch);
    }

    // Update is called once per frame
    public void Update()
    {
        // 決定ボタン(スペースキー・エンターキー)の処理
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (buttons.Length == 1)
            {
                buttons[buttonIdx].onClick.Invoke();
                return;
            }
            else
            {
                buttons[buttonIdx - 1].onClick.Invoke();
                return;
            }
        }

        // buttonsが2個以上ある時
        if (buttons.Length > 1)
        {
            // 取得した入力処理でインデックスを切り替える
            if (Input.GetKeyDown(KeyCode.UpArrow))      // 上
            {
                if (buttonIdx > 1) { buttonIdx--; }
                else if (buttonIdx == 1) { buttonIdx = 3; }
                else { buttonIdx = 1; }
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))    // 下
            {
                if (buttonIdx < buttons.Length) { buttonIdx++; }
                else if (buttonIdx >= buttons.Length) { buttonIdx = 1; }
                else { buttonIdx = 3; }
            }
        }

        if (pUIswitch == null) { return; }

        // コマンドによるUIの切り替え
        switch (buttonIdx)
        {
            case 1:     // Resume
                if (prevIdx == 2)
                {
                    pUIswitch.Switch_RetryNormal();
                }
                else if (prevIdx == 3)
                {
                    pUIswitch.Switch_TitleNormal();
                }
                pUIswitch.Switch_ResumeGlow();
                break;

            case 2:     // Retry
                if (prevIdx == 1)
                {
                    pUIswitch.Switch_ResumeNormal();
                }
                else if (prevIdx == 3)
                {
                    pUIswitch.Switch_TitleNormal();
                }
                pUIswitch.Switch_RetryGlow();
                break;

            case 3:     // ReturnToTitle
                if (prevIdx == 1)
                {
                    pUIswitch.Switch_ResumeNormal();
                }
                else if (prevIdx == 2)
                {
                    pUIswitch.Switch_RetryNormal();
                }
                pUIswitch.Switch_TitleGlow();
                break;
            default:
                break;
        }

        prevIdx = buttonIdx;
        Debug.Log(prevIdx);
    }

    //--- ボタンの処理 ---//
    //--- Retryが押された時 ---//
    public void OnRetry(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //--- StageResetが押された時 ---//
    public void OnStageReset(string sceneName)
    {
        PlayerPrefs.SetInt("CurrentStageNum", 0);   // 現在ステージを1に戻す
        PlayerPrefs.DeleteKey("MagObjPositions");   // 磁石のデータを削除
        PlayerPrefs.SetInt("Deaths", 0);            // 死亡回数をリセット
        PlayerPrefs.SetFloat("PlayTime", 0.0f);     // プレイ時間をリセット
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }

    //--- ReturnToTitleが押された時 ---//
    public void OnReturnTitle(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
