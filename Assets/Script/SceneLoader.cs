using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 制作者　ゴロイヒデキ

public class SceneLoader : MonoBehaviour
{
    public string[] sceneNames; // シーン名をInspectorでリスト化
    private int currentSceneIndex = 0; // 現在のシーンインデックスで保持

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // コントローラーの入力をチェック
        if (Input.GetButtonDown("Submit"))
        {
            LoadNextScene();
        }
    }

    public void LoadNextScene()
    {
        // 次のシーン名を計算して切り替え
        currentSceneIndex = (currentSceneIndex + 1) % sceneNames.Length;
        SceneManager.LoadScene(sceneNames[currentSceneIndex]);

    }

    void Awake()
    {
        if (FindObjectsOfType<SceneLoader>().Length > 1)
        {
            Destroy(gameObject); // 複製を防ぐ
            return;
        }

        DontDestroyOnLoad(this.gameObject); // シーンをまたいで保持
    }
}
