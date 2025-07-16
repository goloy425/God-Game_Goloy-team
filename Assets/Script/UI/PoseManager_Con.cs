using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


// ポーズ画面のコントローラー操作

public class PoseManager_Con : MonoBehaviour
{
    [Header("ボタン Resume→Retry→ReturnTitleの順番で設定")]
    public Button[] buttons;

    private Gamepad gamepad;
    private GameInputs inputs;  // GameInputsクラス
    private PoseManager_KeyMou pManager_k;

    // Start is called before the first frame update
    public void Start()
    {
        gamepad = Gamepad.current;
        pManager_k = GetComponent<PoseManager_KeyMou>();

        inputs = new GameInputs();
        inputs.Enable();
    }

    // Update is called once per frame
    public void Update()
    {
        if (gamepad == null) { return; }    // コントローラーが接続されてない場合はスルー

        // 入力の取得
        bool decideKey = inputs.Select.Decide.WasPressedThisFrame();    // 決定（任天堂:A PS:〇 Xbox:B）
        bool selectUpKey = inputs.Select.SelectUp.WasPressedThisFrame();        // 十字キー上
        bool selectDownKey = inputs.Select.SelectDown.WasPressedThisFrame();    // 　 〃 　下

        // 決定ボタンの処理
        if (decideKey)
        {
            if (buttons.Length == 1)
            {
                buttons[pManager_k.buttonIdx].onClick.Invoke();
            }
            else
            {
                if (pManager_k.buttonIdx != 0)
                {
                    buttons[pManager_k.buttonIdx - 1].onClick.Invoke();
                }
            }
        }

        if (buttons.Length > 1)
        {
            // 取得した入力処理でインデックスを切り替える
            if (selectUpKey)    // 上
            {
                if (pManager_k.buttonIdx > 1) { pManager_k.buttonIdx--; }
                else { pManager_k.buttonIdx = 3; }
            }
            if (selectDownKey)  // 下
            {
                if (pManager_k.buttonIdx < buttons.Length) { pManager_k.buttonIdx++; }
                else { pManager_k.buttonIdx = 1; }
            }
        }

        // UIの切り替えはKeyMouの方でやる
    }

    private void OnDestroy()
    {
        inputs?.Dispose();
    }
}